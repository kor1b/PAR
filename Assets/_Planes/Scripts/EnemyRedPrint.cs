using UnityEngine;
using UnityEngine.UI;

namespace Planes
{
    public class EnemyRedPrint : CharacterPrint
    {
        [Header("Speed Settings")]
        [SerializeField] [Tooltip("Стандартная скорость движения")]
        private float standartSpeed = 10;
        [SerializeField] [Tooltip("Скорость движения в режиме ускорения за игроком")]
        public float maxSpeed = 60;
        [SerializeField] [Tooltip("Скорость движения в режиме отрыва от игрока")]
        private float lowSpeed = 5;
        [SerializeField] [Tooltip("Скорость движения в данный момент. Изменяется через lowSpeed, maxSpeed или standartSpeed")]
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
        [Header("TurboMode Settings")]
        [SerializeField] [Tooltip("Начинает ускоряться, чтобы догнать игрока, на расстояниии")]
        public float offsetFarBegin = 150f;
        [SerializeField] [Tooltip("Выходит из режима ускорения за игроком на расстоянии")]
        public float offsetFarEnd = 70f;
        [Header("Thunderstorm Settings")]
        [SerializeField] [Tooltip("Минимальное время до начала грозы")]
        int minTimeBeforeStorm = 20;
        [SerializeField] [Tooltip("Максимальное время до начала грозы")]
        int maxTimeBeforeStorm = 80;
        [SerializeField] [Tooltip("Позиция этого персонажа")]
        private Transform _enemy; //позиция персонажа
        [SerializeField] [Tooltip("Нос")]
        private Transform _nose;
        [SerializeField]
        private Rigidbody _enemyRigidbody; //для обнуления поворота после столкновения
        [SerializeField] [Tooltip("Скрипт молний")]
        private Thunderstorm _thunderstorm; //скрипт молний

        private Transform _player; //отслеживаем положение игрока

        private float timer; //таймер для подсчета времени между выстрелами/отображением эффектов

        private float shootDelayTimer; //подсчет времени, когда можно стрелять после обнаружения игрока
        private float prevShoot = 0; //время предыдущего выстрела для регулировки частоты выстрелов
        private bool playerInRange = false; //находится ли игрок в досягаемости выстрела

        private bool breakawayMode = false; //находимся ли мы в режиме отрыва от игрока (чтобы не столкнуться с ним)
        private float directionY = 0f; //для поиска направления при режиме отрыва от игрока

        private bool turboMode = false; //находимся ли мы в турбо-режиме (догоняем игрока, чтобы он не улетал далеко)
        
        float timeBeforeStorm; //время, которое осталось до следующей грозы

        private System.Random random;

        //было нужно для погони за бонусами
        /*GameObject _healthBonus;
        GameObject[] avalibleHealthBonuses;
        [SerializeField]
        bool noHealthBonuses = true;
        Transform closestHealthBonus;*/

        void Awake()
        {
            isPlayer = false; //объект не является игроком
            bossNumber = 3; //третий босс
            _sliderHealth = GameObject.FindWithTag("EnemyHealth").GetComponent<Slider>();
            _sliderHealth.maxValue = maxHealth;
            _sliderHealth.value = maxHealth;
            random = new System.Random();
            health = _sliderHealth.value;
            speed = standartSpeed;
            //_healthBonus = GameObject.FindWithTag("Bonuses");
            timeBeforeStorm = random.Next(minTimeBeforeStorm, maxTimeBeforeStorm);
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
            _enemyRigidbody.angularVelocity = new Vector3(0, 0, 0); //убирать посторонние вращения
            //постоянное движение вперед
            _enemy.position = Vector3.MoveTowards(_enemy.position, _nose.position, speed * Time.deltaTime);

            Quaternion neededRotation = Quaternion.Euler(0, 0, 0); //мы повернемся на neededRotation после того, как выберем направление (исправим координаты)

            #region HealthBonusTracker(Unactive)
            /*if (health < maxHealth / 3)
            {
                if (closestHealthBonus == null || !closestHealthBonus.gameObject.activeInHierarchy)
                {
                    avalibleHealthBonuses = GameObject.FindGameObjectsWithTag("HealthBonus");
                    Debug.Log("Health bonuses avalible: " + avalibleHealthBonuses.Length);
                    if (avalibleHealthBonuses.Length == 0)
                    {
                        closestHealthBonus = null;
                        noHealthBonuses = true;
                    }
                    else
                    {
                        noHealthBonuses = false;
                        if (avalibleHealthBonuses.Length == 1)
                        {
                            closestHealthBonus = avalibleHealthBonuses[0].GetComponent<Transform>();
                        }
                        else
                        {
                            float[] distanses = new float[avalibleHealthBonuses.Length];
                            float minDistance = 1000;
                            int minIndex = 0;
                            for (int i = 0; i < distanses.Length; i++)
                            {
                                distanses[i] = Vector3.Distance(_enemy.position, avalibleHealthBonuses[i].GetComponent<Transform>().position);
                                if (minDistance > distanses[i])
                                {
                                    minDistance = distanses[i];
                                    minIndex = i;
                                }
                            }
                            closestHealthBonus = avalibleHealthBonuses[minIndex].GetComponent<Transform>();
                        }
                    }
                }
                if (!noHealthBonuses)
                {
                    speed = standartSpeed;
                    neededRotation = Quaternion.LookRotation(closestHealthBonus.position - _enemy.position);
                    //_enemy.rotation = Quaternion.Slerp(_enemy.rotation, neededRotation, Time.deltaTime * lookDelay);
                }
                
            }*/
            #endregion
            //с небольшой задержкой поворачиваемся в направлении игрока
            if ((Vector3.Distance(_player.position, _enemy.position) < offsetCloseBegin || breakawayMode)) //если подлетел слишком быстро: сбавляет скорость и рандомно меняет направление
            {
                breakawayMode = true;
                speed = lowSpeed;
                if (directionY == 0)
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
                neededRotation = Quaternion.Euler(0, directionY, 0);
                if (Vector3.Distance(_player.position, _enemy.position) > offsetCloseEnd)
                {
                    breakawayMode = false;
                }
            }
            else if (!breakawayMode /*&& noHealthBonuses*/)
            {
                directionY = 0f;
                if (Vector3.Distance(_player.position, _enemy.position) > offsetFarBegin || turboMode) //если мы улетели слишком далеко от врага - начинаем ускорение
                {
                    turboMode = true;
                    speed = maxSpeed;
                    if (Vector3.Distance(_player.position, _enemy.position) < offsetFarEnd)
                    {
                        turboMode = false;
                    }
                }
                else
                {
                    speed = standartSpeed;
                }
                neededRotation = Quaternion.LookRotation(_player.position - _enemy.position);
            }
            _enemy.rotation = Quaternion.Slerp(_enemy.rotation, neededRotation, Time.deltaTime * rotationSpeed); //применяем выбранное направление
            #endregion

            #region SHOOTING
            foreach (SingleShoot point in shootingPoints) //проверяем, находится ли игрок в зоне досягаемости
            {
                if (point.CanDamage() && !playerInRange)
                {
                    playerInRange = true;
                    shootDelayTimer = shootDelay; //если находится, то начинаем отсчет до выстрела
                }
            }
            if (shootDelayTimer > 0) //отсчет до выстрела
            {
                shootDelayTimer -= Time.deltaTime;
            }
            else if (shootDelayTimer <= 0 && playerInRange) //стреляем, когда подойдет время и если игрок был замечен в зоне досягамости
            {
                if (timer - prevShoot >= timeBetweenShoots)
                {
                    Shooting();
                    prevShoot = timer;
                }
                playerInRange = false; //чтобы затем начать искать игрока заново
            }

            if (timer - prevShoot >= timeBetweenShoots * effectsDisplayTime) //убираем эффекты
            {
                foreach (SingleShoot point in shootingPoints)
                {
                    point.DisableEffects();
                }
            }
            #endregion

            //решаем, когда запускать новую грозу
            if (timeBeforeStorm > 0) //если таймер установлен, то ведем отсчет
            {
                timeBeforeStorm -= Time.deltaTime;
            }
            else //если отсчет подошел к конку, начинаем генерацию молний и ставим следующий таймер
            {
                _thunderstorm.StartThunderstorm();
                timeBeforeStorm = random.Next(minTimeBeforeStorm, maxTimeBeforeStorm);
            }
        }

        override public void Shooting() //стрельба
        {
            _shootAudio.Stop();
            _shootAudio.Play();
            foreach (SingleShoot point in shootingPoints)
            {
                point.Shoot();
            }
        }

        override protected void Death()
        {
            Debug.Log("Enemy died");
            GameManager.instance.EndGame(GetComponent<CharacterPrint>());
            gameObject.SetActive(false);
            GameObject explosion = Instantiate(ObjectPoolingManager.instance.Explosion);
            explosion.GetComponent<Transform>().position = gameObject.GetComponent<Transform>().position;
            explosion.GetComponent<ParticleSystem>().Play();
            Destroy(explosion, 7f);
        }
    }
}