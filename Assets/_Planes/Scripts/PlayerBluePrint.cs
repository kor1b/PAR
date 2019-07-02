using UnityEngine;
using UnityEngine.UI;

namespace Planes
{
    public class PlayerBluePrint : CharacterPrint
    {
        private float timer; //таймер для подсчета времени между выстрелами/отображением эффектов
        private Slider _gunCharge; //для проверки оставшегося заряда
        [SerializeField] [Tooltip("Максимальный заряд")]
        private float maxCharge;
        
        void Awake()
        {
            isPlayer = true; //объект является игроком
            _gunCharge = GameObject.FindWithTag("FireSystem").GetComponent<Slider>();
            _gunCharge.maxValue = maxCharge;
            _gunCharge.value = maxCharge;

            _sliderHealth = GameObject.FindWithTag("PlayerHealth").GetComponent<Slider>();
            _sliderHealth.maxValue = maxHealth;
            _sliderHealth.value = maxHealth;
            health = _sliderHealth.value;
        }

        void Update()
        {
            if (takeoffMode || !GameManager.gameIsGoing)
            {
                return;
            }
            timer += Time.deltaTime;
            if(_shootAudio.isPlaying) //если аудио включено, значит мы стреляем
            {
                audioDurrability += Time.deltaTime; //подсчитываем, как долго было включено аудио
                if (stopAudio && audioDurrability > audioMinDurrability) //если нам нужно как можно скорее отключить аудио, делаем это, как только истечет минимальное время его проигрывания
                {
                    _shootAudio.Stop();
                }
            }
            else
            {
                if (timer >= timeBetweenShoots * effectsDisplayTime) //следим за тем, когда перестать отображать эффекты - доступно только если аудио уже не играет
                {
                    foreach (SingleShoot point in shootingPoints)
                    {
                        point.DisableEffects();
                    }
                }
            }
        }

        override public void Shooting()
        {
            if (timer < timeBetweenShoots) //не позволять игроку стрелять слишком быстро
            {
                Debug.Log("Cannot shoot now");
                return;
            }
            if (_gunCharge.value < 1) //не позволяем стрелять, если нет заряда
            {
                //Debug.Log("No charge!");
                _shootAudio.Stop();
                return;
            }
            //совершаем по выстрелу из каждой точки для стрелбы
            foreach (SingleShoot point in shootingPoints)
            {
                point.Shoot();
            }
            if (!_shootAudio.isPlaying)
            {
                _shootAudio.Play();
            }
            _gunCharge.value -= 1; //отнимаем заряд
            timer = 0f; //обнуляем таймер
        }

        public void FireButtonReleased() //контролирует аудио в зависимости от нажатия кнопки огонь
        {
            if (audioDurrability > audioMinDurrability)
            {
                _shootAudio.Stop();
                stopAudio = false;
            }
            else
            {
                stopAudio = true;
            }
        }
        /*
        override protected void Death() //если игрок умер
        {
            Debug.Log("Player died");
            gameObject.SetActive(false); //делаем объект неактивным
            GameObject explosion = Instantiate(ObjectPoolingManager.instance.Explosion);
            explosion.GetComponent<Transform>().position = gameObject.GetComponent<Transform>().position;
            Destroy(explosion, 7f);
            GameManager.Instance.EndGame(GetComponent<CharacterPrint>()); //отправляем данные GameManager
        }
        */
        private void OnTriggerEnter(Collider other) //столкновение с другим самолетом
        {
            if (takeoffMode)
            {
                return;
            }
            CharacterPrint obj = other.gameObject.GetComponent<CharacterPrint>();
            if (obj != null)
            {
                Debug.Log("Collision with enemy");
                TakeDamage(health / 20);
                obj.TakeDamage(health / 20);
            }
        }
       
    }
}