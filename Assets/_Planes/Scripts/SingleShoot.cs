using UnityEngine;

//единичный выстрел из точки ShootPoint
//не контролирует частоту выстрелов и заряд!
namespace Planes
{
    public class SingleShoot : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Урон от одного выстрела")]
        private int damagePerShot = 5; //урон
        [SerializeField]
        [Tooltip("Радиус выстрела")]
        private float range = 50f; //радиус

        //просчет выстрела
        private Transform _shootPoint; //точка, из которой стреляем
        private Ray _shootRay; //луч-направление выстрела
        private RaycastHit shootHit; //для сбора информации при попадании
        private int shootableMask; //слой, чтобы определить объекты, в которые можно стрелять

        //графика
        private ParticleSystem _shootParticles; //анимация выстрела
        private LineRenderer _shootLine; //видимый луч выстрела
        private Light _shootLight; //свет при выстреле

        //автонаведение
        private Transform _enemy; //для отслеживания положения врага
        private float aimRange; //допустимый поворот
        private Vector3 rotation; //поворот, необходимый для наведения
        private CharacterPrint thisCharacterPrint; //для определения персонажа и его врага

        void Awake()
        {
            shootableMask = LayerMask.GetMask("Shootable");
            _shootPoint = GetComponent<Transform>();
            _shootRay = new Ray();
            _shootParticles = GetComponent<ParticleSystem>();
            _shootLine = GetComponent<LineRenderer>();
            _shootLight = GetComponent<Light>();
            thisCharacterPrint = GetComponentInParent<CharacterPrint>();
            aimRange = thisCharacterPrint.aimRange;
        }

        private void Start()
        {
            if (thisCharacterPrint.isPlayer)
            {
                _enemy = GameObject.FindWithTag("Enemy").GetComponent<Transform>();
            }
            else if (!thisCharacterPrint.isPlayer)
            {
                _enemy = GameObject.FindWithTag("Player").GetComponent<Transform>();
            }
        }

        public void DisableEffects() //отключаем эффекты
        {
            _shootLine.enabled = false;
            _shootLight.enabled = false;
        }

        private void Update()
        {
            _shootPoint.LookAt(_enemy.position); //выясняем местонахождение врага и настраиваемся на него
            rotation = _shootPoint.localEulerAngles;
            #region ClampRotation //вовремя обнуляет параметры, чтобы объект не "закручивался" в огромные углы
            while (!(rotation.x > -180 && rotation.x < 180))
            {
                if (rotation.x > 180)
                {
                    rotation.x -= 360;
                }
                if(rotation.x < -180)
                {
                    rotation.x += 360;
                }
            }
            while (!(rotation.y > -180 && rotation.y < 180))
            {
                if (rotation.y > 180)
                {
                    rotation.y -= 360;
                }
                if (rotation.y < -180)
                {
                    rotation.y += 360;
                }
            }
            while (!(rotation.z > -180 && rotation.z < 180))
            {
                if (rotation.z > 180)
                {
                    rotation.z -= 360;
                }
                if (rotation.z < -180)
                {
                    rotation.z += 360;
                }
            }
            #endregion
            //если враг не в зоне автонаведения, обнуляем параметры
            if (rotation.x < 0 - aimRange || rotation.x > 0 + aimRange)
            {
                rotation.x = 0;
            }
            if (rotation.y < 0 - aimRange || rotation.y > 0 + aimRange)
            {
                rotation.y = 0;
            }
            if (rotation.z < 0 - aimRange || rotation.z > 0 + aimRange)
            {
                rotation.z = 0;
            }
            _shootPoint.localEulerAngles = rotation; //применяем получившейся поворот
        }


        public void Shoot()
        {
            //включение эффектов
            _shootLight.enabled = true;
            _shootParticles.Stop();
            _shootParticles.Play();
            _shootLine.enabled = true;
            _shootLine.SetPosition(0, _shootPoint.position); //ставим направление выстрела
            //направление луча выстрела
            _shootRay.origin = _shootPoint.position;
            _shootRay.direction = _shootPoint.forward;

            if (Physics.Raycast(_shootRay, out shootHit, range, shootableMask)) //если попали по объекту на слое Shootable
            {
                if (shootHit.collider != null)
                {
                   // Debug.Log("Hit smth");
                }
                CharacterPrint obj = shootHit.collider.GetComponentInParent<CharacterPrint>();
                if (obj != null)
                {
                 //   Debug.Log("Ouch");
                    obj.TakeDamage(damagePerShot); //отнимаем здоровье
                }
                _shootLine.SetPosition(1, shootHit.point); //прорисовываем луч выстрела до той точки, куда попали

            }
            else
            {
                _shootLine.SetPosition(1, _shootRay.origin + _shootRay.direction * range); //если ни в кого не попали, прорисовываем выстрел на максимальный радиус
            }
        }

        public bool CanDamage() //показывает, сможет ли персонаж кого-то ранить, если выстрелит прямо сейчас. Используется для врага!
        {
            //запуск луча
            _shootRay.origin = _shootPoint.position;
            _shootRay.direction = _shootPoint.forward;
            if(Physics.Raycast(_shootRay, out shootHit, range, shootableMask)) //считывание результатов
            {
                CharacterPrint obj = shootHit.collider.GetComponentInParent<CharacterPrint>();
                if (obj != null)
                {
                    if (obj.isPlayer) //если впереди игрок, возвращаем true
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}