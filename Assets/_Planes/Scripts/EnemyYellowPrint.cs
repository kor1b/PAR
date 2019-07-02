using UnityEngine;
using UnityEngine.UI;

namespace Planes
{
    public class EnemyYellowPrint : CharacterPrint
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
        [Header("SandStorm Settings")]
        [SerializeField] [Tooltip("Минимальное время между началами песчаных бурь")]
        public int minTimeBeforeNextSandStorm = 30;
        [SerializeField] [Tooltip("Максимальное время между началами песчаных бурь")]
        public int maxTimeBeforeNextSandStorm = 160;

        [Header("Components")]
        [SerializeField]
        private Transform _enemy; //позиция персонажа
        [SerializeField]
        private Transform _nose;
        [SerializeField]
        private Rigidbody _enemyRigidbody; //для обнуления поворота после столкновения
        [SerializeField]
        private SandStorm sandStorm; //скрипт песчаной бури
        private Transform _player; //отслеживаем положение игрока
        private float timer; //таймер для подсчета времени между выстрелами/отображением эффектов

        private float shootDelayTimer; //подсчет времени, когда можно стрелять после обнаружения игрока
        private float prevShoot = 0; //время предыдущего выстрела для регулировки частоты выстрелов
        private bool playerInRange = false; //находится ли игрок в досягаемости выстрела
        
        private bool breakawayMode = false; //находимся ли мы в режиме отрыва от игрока (чтобы не столкнуться с ним)
        private float directionY = 0f; //для поиска направления при режиме отрыва от игрока

        private bool turboMode = false; //находимся ли мы в турбо-режиме (догоняем игрока, чтобы он не улетал далеко)

        private float timeBeforeNextSandStorm; //время до следующей песчаной бури
        private float timeSinceLastSandStorm = 0; //время последней песчаной бури

        private System.Random random;

        void Awake()
        {
            isPlayer = false; //объект не является игроком
            _sliderHealth = GameObject.FindWithTag("EnemyHealth").GetComponent<Slider>();
            _sliderHealth.maxValue = maxHealth;
            _sliderHealth.value = maxHealth;
            random = new System.Random();
            timeBeforeNextSandStorm = random.Next(minTimeBeforeNextSandStorm, maxTimeBeforeNextSandStorm);
            health = _sliderHealth.value;
            speed = standartSpeed;
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
            _enemyRigidbody.angularVelocity = new Vector3(0, 0, 0); //убрать постороннее вращение
            //постоянное движение вперед
            _enemy.position = Vector3.MoveTowards(_enemy.position, _nose.position, speed * Time.deltaTime);
            //с небольшой задержкой поворачиваемся в направлении игрока
            if (Vector3.Distance(_player.position, _enemy.position) < offsetCloseBegin || breakawayMode) //если подлетел слишком близко: сбавляет скорость и рандомно меняет направление
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
                Quaternion neededRotation = Quaternion.Euler(0, directionY, 0);
                _enemy.rotation = Quaternion.Slerp(_enemy.rotation, neededRotation, Time.deltaTime * rotationSpeed);
                if (Vector3.Distance(_player.position, _enemy.position) > offsetCloseEnd)
                {
                    directionY = 0f;
                    breakawayMode = false;
                }
            }
            else
            {
                if (Vector3.Distance(_player.position, _enemy.position) > offsetFarBegin || turboMode) //если мы улетели слишком далеко от игрока - начинаем ускорение
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
                Quaternion neededRotation = Quaternion.LookRotation(_player.position - _enemy.position);
                _enemy.rotation = Quaternion.Slerp(_enemy.rotation, neededRotation, Time.deltaTime * rotationSpeed);
            }
            #endregion

            #region SHOOTING
            if (!playerInRange) //если игрок в зоне досягаемости не замечен, проверяем, не появился ли он там
            {
                foreach (SingleShoot point in shootingPoints) //для каждого пулемета
                {
                    if (point.CanDamage())
                    {
                        playerInRange = true;
                        shootDelayTimer = shootDelay; //если игрок в зоне досягаемости, начинаем отсчет до выстрела
                    }
                }
            }
            if (shootDelayTimer > 0) //уменьшаем отсчет до выстрела до тех пор, пока он больше нуля
            {
                shootDelayTimer -= Time.deltaTime;
            }
            else if (shootDelayTimer <= 0 && playerInRange) //если отсчет достиг нуля и мы зафиксировали игрока в зоне досягаемости
            {
                if (timer - prevShoot >= timeBetweenShoots) //проверяем частоту выстрелов
                {
                    Shooting();
                    prevShoot = timer;
                }
                playerInRange = false; //после каждого выстрела нужно будет заново проверить, в диапазоне ли игрок
            }
            if (_shootAudio.isPlaying) //если аудио включено, значит мы стреляем
            {
                audioDurrability += Time.deltaTime; //подсчитываем, как долго было включено аудио
                if (stopAudio && audioDurrability > audioMinDurrability) //если нам нужно как можно скорее отключить аудио, делаем это, как только истечет минимальное время его проигрывания
                {
                    _shootAudio.Stop();
                }
            }
            if (timer - prevShoot >= timeBetweenShoots * effectsDisplayTime) //убираем отображение эффектов
            {
                foreach (SingleShoot point in shootingPoints)
                {
                    point.DisableEffects();
                }
            }
            #endregion

            //контроль и запуск песчаной бури
            if (timer - timeSinceLastSandStorm >= timeBeforeNextSandStorm) 
            {
                Debug.Log("Starting SandStorm");
                timeSinceLastSandStorm = timer;
                sandStorm.StartSandStorm();
                timeBeforeNextSandStorm = random.Next(minTimeBeforeNextSandStorm, maxTimeBeforeNextSandStorm); //выбираем следующий момент запуска бури
            }
        }

        override public void Shooting() //стрельба
        {
            if (!_shootAudio.isPlaying) //если аудио уже играет, не нужно его включать
            {
                audioDurrability = 0;
                _shootAudio.Play();
            }
            foreach (SingleShoot point in shootingPoints)
            {
                point.Shoot();
            }
            stopAudio = true; //после каждого выстрела собираемся отключить аудио
        }

        override protected void Death()
        {
            Debug.Log("Enemy died");
            GameManager.Instance.EndGame(GetComponent<CharacterPrint>());
            gameObject.SetActive(false);
            GameObject explosion = Instantiate(ObjectPoolingManager.instance.Explosion);
            explosion.GetComponent<Transform>().position = gameObject.GetComponent<Transform>().position;
            explosion.GetComponent<ParticleSystem>().Play();
            Destroy(explosion, 7f);
        }
    }
}