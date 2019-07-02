using UnityEngine;
using UnityEngine.UI;

namespace Planes
{
    public class EnemyGreenPrint : CharacterPrint
    {
        [Header("Speed Settings")]
        [SerializeField] [Tooltip("Стандартная скорость движения")]
        private float standartSpeed = 10;
        [SerializeField] [Tooltip("Скорость движения в режиме отрыва от игрока")]
        private float lowSpeed = 5;
        [SerializeField] [Tooltip("Скорость движения в данный момент. Изменяется через lowSpeed или standartSpeed")]
        private float speed;
        [SerializeField] [Tooltip("Скорость поворота")]
        private float rotationSpeed = 0.3f;
        [SerializeField] [Tooltip("Задержка выстрела после того, как увидит игрока")]
        private float shootDelay = 1f;
        [Header("Breakaway Settings")]
        [SerializeField] [Tooltip("Начинает отдаляться от игрока, как только приблизится на расстояние")]
        private float offsetCloseBegin = 5;
        [SerializeField] [Tooltip("Прекращает отдаляться от игрока только по достижению расстояния")]
        public float offsetCloseEnd = 10;

        [Header("Plane's Components")]
        [SerializeField] [Tooltip("Позиция персонажа")]
        private Transform _enemy; //позиция персонажа
        [SerializeField] [Tooltip("Нос - точка, указывающая перед самолета")] 
        private Transform _nose;
        [SerializeField] [Tooltip("Rigidbody для обнуления постороннего вращения после столновения")]
        private Rigidbody _enemyRigidbody; //для обнуления поворота после столкновения

        private Transform _player; //отслеживаем положение игрока

        float timer; //таймер для подсчета времени между выстрелами/отображением эффектов
        private float shootDelayTimer; //подсчет времени, когда можно стрелять после обнаружения игрока
        private float prevShoot = 0; //время предыдущего выстрела для регулировки частоты выстрелов
        private bool playerInRange = false; //находится ли игрок в досягаемости
        bool breakawayMode = false; //находимся ли мы в режиме отрыва от игрока (чтобы не столкнуться с ним)
        protected float directionY = 0f; //для поиска направления при режиме отрыва от игрока
        private System.Random random; //для рандомного выбора направления ухода от столкновения

        void Awake()
        {
            isPlayer = false; //объект не является игроком
            _sliderHealth = GameObject.FindWithTag("EnemyHealth").GetComponent<Slider>();
            _sliderHealth.maxValue = maxHealth;
            _sliderHealth.value = maxHealth;
            health = _sliderHealth.value;
            speed = standartSpeed;
            random = new System.Random();
            _shootAudio = GetComponent<AudioSource>();
        }

        private void Start()
        {
            _player = GameObject.FindWithTag("Player").GetComponent<Transform>();
        }

        void Update()
        {
            if (takeoffMode || !GameManager.gameIsGoing)
            {
                return;
            }
            timer += Time.deltaTime;
            #region MOVEMENT
            _enemyRigidbody.angularVelocity = new Vector3(0, 0, 0); //обнулить сторонние вращения
            //постоянное движение вперед
            _enemy.position = Vector3.MoveTowards(_enemy.position, _nose.position, speed * Time.deltaTime);
            //направление
            if (Vector3.Distance(_player.position, _enemy.position) < offsetCloseBegin || breakawayMode) //если подбираемся слишком близко к игроку, переходим в режим отрыва
            {
                breakawayMode = true;
                //сбавляем скорость
                speed = lowSpeed;
                
                if (directionY == 0) //если направление поворота еще не выбрано, рандомно выбираем сторону, в которую будем поворачивать, чтобы уйти
                {
                    int dir = random.Next(1, 5);
                    switch (dir)
                    {
                        case 1: directionY = 60f; break;
                        case 2: directionY = -60f; break;
                        case 3: directionY = 120f; break;
                        case 4: directionY = -120f; break;
                    }
                }
                Quaternion neededRotation = Quaternion.Euler(0, directionY, 0); //задаем целевой поворот
                _enemy.rotation = Quaternion.Slerp(_enemy.rotation, neededRotation, Time.deltaTime * rotationSpeed); //медленно поворачиваемся
                if (Vector3.Distance(_player.position, _enemy.position) > offsetCloseEnd) //если отдалились от игрока достаточно, выходим из режима
                {
                    directionY = 0;
                    breakawayMode = false;
                }
            }
            else //с небольшой задержкой поворачиваемся в направлении игрока
            {
                speed = standartSpeed;
                Quaternion neededRotation = Quaternion.LookRotation(_player.position - _enemy.position);
                _enemy.rotation = Quaternion.Slerp(_enemy.rotation, neededRotation, Time.deltaTime * rotationSpeed);
            }
            #endregion

            #region SHOOTING
            foreach (SingleShoot point in shootingPoints)
            {
                if (point.CanDamage() && !playerInRange) //проверяем, появился ли игрок в диапазоне поражения
                {
                    playerInRange = true;
                    shootDelayTimer = shootDelay; //если да, то начинаем отсчет до выстрела
                }
            }
            if (shootDelayTimer > 0) //уменьшаем таймер до выстрела
            {
                shootDelayTimer -= Time.deltaTime;
            }
            else if(shootDelayTimer <= 0 && playerInRange) //когда таймер достигнет нуля, стреляем (при условии соблюдения времени между выстрелами) и обнуляем таймер
            {
                if (timer - prevShoot >= timeBetweenShoots)
                {
                    Shooting();
                    prevShoot = timer;
                }
                playerInRange = false; //чтобы после этого начать искать игрока заново
            }

            if (_shootAudio.isPlaying) //если аудио включено, значит мы стреляем
            {
                audioDurrability += Time.deltaTime; //подсчитываем, как долго было включено аудио
                if (stopAudio && audioDurrability > audioMinDurrability) //если нам нужно как можно скорее отключить аудио, делаем это, как только истечет минимальное время его проигрывания
                {
                    _shootAudio.Stop();
                }
            }
            else
            {
                if (timer - prevShoot >= timeBetweenShoots * effectsDisplayTime) //следим за тем, когда перестать отображать эффекты - доступно только если аудио уже не играет
                {
                    foreach (SingleShoot point in shootingPoints)
                    {
                        point.DisableEffects();
                    }
                }
            }
            #endregion
        }

        override public void Shooting()
        {
            if (!_shootAudio.isPlaying) //если аудио уже играет, не нужно его включать
            {
                audioDurrability = 0;
                _shootAudio.Play();
            }
            foreach (SingleShoot point in shootingPoints) //стрельба из каждой точки
            {
                point.Shoot();
            }
            stopAudio = true; //после каждого выстрела собираемся отключить аудио
        }
        /*
        override protected void Death()
        {
            Debug.Log("Enemy died");
            GameManager.Instance.EndGame(GetComponent<CharacterPrint>()); //отчитываемся GameManager
            gameObject.SetActive(false);
            GameObject explosion = Instantiate(ObjectPoolingManager.instance.Explosion);
            explosion.GetComponent<Transform>().position = gameObject.GetComponent<Transform>().position;
            explosion.GetComponent<ParticleSystem>().Play();
            Destroy(explosion, 7f);
        }*/
    }
}