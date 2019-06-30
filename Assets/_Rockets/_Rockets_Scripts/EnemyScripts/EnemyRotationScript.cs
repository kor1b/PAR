using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Rockets
{
    public class EnemyRotationScript : ShipFactory
    {
        public int freezeProbabilityInPercent;  // Вероятность кастования умения в процентном соотношении
        public float freezeDuration;      //Длительность умения "Заморозка"
        public float freezeDamage;        //Урон заморозки за каждый фрэйм(примерно 2 милисекунды)

        public int missileProbabilityInPercent;  //Вероятность запуска ракеты            

        public bool castFreeze;            //Сигнал кастования умения "Заморозка"
        public bool launchMissile;         //Сигнал запуска ракеты
        public bool launchDeathStorm;      //Muhahahahahahah

        private EnemyPursuit enemyPursuit;         //Скрипт Бати
        private PlayerRotationScript playerStats;

        public int RandomDigit1;                    //Случайные числа для иллюзии вероятности
        public int RandomDigit2;
        public int RandomDigit3;

        //Переменные для хранения изначальных характеристик
        private float originStoppingDistance;
        private float originRetreatingDistance;
        private float originSpeed;
        private float originRotationSpeed;
        private float originMissileProbability;
        private float originFreezeProbability;

        public bool ShootActive;

        public float stoppingDistance;
        public float retreatingDistance;

        void Start()
        {
            tr = GetComponent<Transform>();

            gameManager = GameManager.Instance;

            healthSlider = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponent<Slider>();
            shieldSlider = GameObject.FindGameObjectWithTag("EnemyShieldBar").GetComponent<Slider>();
            shieldPrefab = GameObject.FindGameObjectWithTag("EnemyShield");

            target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
            playerStats = target.GetComponent<PlayerRotationScript>();
            enemyPursuit = parent.GetComponent<EnemyPursuit>();

            enemyPursuit.stoppingDistance = stoppingDistance;
            enemyPursuit.retreatingDistance = retreatingDistance;
            enemyPursuit.speed = speed;
            enemyPursuit.rotationSpeed = rotationSpeed;

            healthSlider.value = startHealth;
            shieldSlider.value = startShield;
            health = startHealth;
            shield = startShield;
            speed = startSpeed;
            rotationSpeed = startRotationSpeed;

            originStoppingDistance = stoppingDistance;     //Сохраняем изначальную дистанцию остановки
            originRetreatingDistance = retreatingDistance; //И дистанцию отступления
            originSpeed = speed;
            originRotationSpeed = rotationSpeed;
            originMissileProbability = missileProbabilityInPercent;
            originFreezeProbability = freezeProbabilityInPercent;

            shieldPrefab.SetActive(true); //Активируем щиты в начале

            timeBetweenShots = startBetweenShots;
            //timeBetweenLaunches = startBetweenLaunches;

            tr.rotation = parent.transform.rotation; //На всякий случай переприсваиваем напраление вращения родителя

            castFreeze = false;                 //В начале не кастуем умений

            RandomDigit1 = 102;                  //В начале задаём число не входящее в наш промежуток
            RandomDigit2 = 102;

            StartCoroutine(RandomPerSec());  //Задаёт сигнал для кастования умения один раз в заданный период времени(5 сек по стандарту)
        }

        void Update()
        {
            /* Здесь можно подключить тот или иной общий стиль поведения :*/
            if (gameManager.NumberOfLevel == 0)
            {
                Enemy1Behaviour();
            }
            if (gameManager.NumberOfLevel == 1)
            {
                Enemy2Behaviour();
            }
            if (gameManager.NumberOfLevel == 2)
            {
                Enemy3Behaviour();
            }
            /* Здесь можно было подключить тот или иной общий стиль поведения.*/

            enemyPursuit.stoppingDistance = stoppingDistance;
            enemyPursuit.retreatingDistance = retreatingDistance;
            enemyPursuit.speed = speed;
            enemyPursuit.rotationSpeed = rotationSpeed;

            /* Стрельба раз в определённый период времени*/
            if (ShootActive)
            {
                if (timeBetweenShots <= 0)
                {
                    Shoot();
                    timeBetweenShots = startBetweenShots;
                }
                else
                {
                    timeBetweenShots -= Time.deltaTime;
                }
            }
        }
        void FixedUpdate()
        {


        }


        public void Enemy1Behaviour()
        {
            if (health <= 50)
            {
                startBetweenShots = 1.5f;
                speed = originSpeed * 1.5f;
                rotationSpeed = originRotationSpeed * 1.5f;
                stoppingDistance = originStoppingDistance / 2 + 0.4f;
                retreatingDistance = originRetreatingDistance / 2 + 0.4f;
            }
        }

        public void Enemy2Behaviour()
        {
            /* Добавляем агрессии в поведение противника */
            if (health <= 50)
            {
                startBetweenShots = 0.5f;
                speed = originSpeed * 1.5f;
                rotationSpeed = originRotationSpeed * 1.5f;
                stoppingDistance = originStoppingDistance / 2 + 0.2f;
                retreatingDistance = originRetreatingDistance / 2 + 0.2f;
            }

            if (castFreeze && GameManager.gameIsGoing && !GameManager.countdownGameStarted)
            {
                playerStats.FreezeCast(freezeDuration, freezeDamage);  //Кастуем заморозку
                castFreeze = false;
            }

            if (launchMissile && GameManager.gameIsGoing && !GameManager.countdownGameStarted)
            {
                LaunchMissile();
                launchMissile = false;
            }

        }

        public void Enemy3Behaviour()
        {
            if (health <= 80 && health > 50)
            {
                startBetweenShots = 0.5f;
                speed = originSpeed * 1.5f;
                rotationSpeed = originRotationSpeed * 1.5f;
                stoppingDistance = originStoppingDistance / 2 + 0.2f;
                retreatingDistance = originRetreatingDistance / 2 + 0.2f;
                freezeProbabilityInPercent = (int)(originFreezeProbability * 1.2f);
                missileProbabilityInPercent = (int)(originMissileProbability * 1.2f);
            }
            else if (health <= 50 && health > 10)
            {
                startBetweenShots = 0.3f;
                speed = originSpeed * 1.5f;
                rotationSpeed = originRotationSpeed * 1.5f;
                stoppingDistance = originStoppingDistance / 2 + 0.2f;
                retreatingDistance = originRetreatingDistance / 2 + 0.2f;
                freezeProbabilityInPercent = (int)(originFreezeProbability * 1.4f);
                missileProbabilityInPercent = (int)(originMissileProbability * 1.4f);
            }
            else if (health <= 10)
            {
                startBetweenShots = 0.1f;
                speed = originSpeed * 1.5f;
                rotationSpeed = originRotationSpeed * 1.5f;
                stoppingDistance = originStoppingDistance / 2 + 0.2f;
                retreatingDistance = originRetreatingDistance / 2 + 0.2f;
                freezeProbabilityInPercent = (int)(originFreezeProbability * 1.8f);
                missileProbabilityInPercent = (int)(originMissileProbability * 1.8f);
            }

            if (castFreeze && GameManager.gameIsGoing && !GameManager.countdownGameStarted)
            {
                playerStats.PetrifyCast(freezeDuration, freezeDamage);  //Кастуем окаменение
                castFreeze = false;
            }

            if (launchMissile && GameManager.gameIsGoing && !GameManager.countdownGameStarted)
            {
                LaunchMissile();
                launchMissile = false;
            }
        }

        public override void Shoot()
        {
            GameObject bulletObject = ObjectPoolingManager.Instance.GetEnemyBullet(bulletPrefab);  //Вызываем пулю из пула:)
            bulletObject.transform.position = shootEmitter.position;
            //bulletObject.transform.up = ShootEmitter.up;                                              //Ставим её на позицию выстрелов
            bulletObject.transform.forward = shootEmitter.forward;                                    //Направляем по направлению движения врага

        }

        /* задаём каст умений с определённой вероятностью в каждый заданный период */
        IEnumerator RandomPerSec()
        {

            yield return new WaitForSeconds(5f);

            RandomDigit1 = Random.Range(0, 101);
            RandomDigit2 = Random.Range(0, 101);

            if (RandomDigit2 < missileProbabilityInPercent)
            {
                launchMissile = true;
            }
            else
            {
                launchMissile = false;
            }

            if (RandomDigit1 < freezeProbabilityInPercent)
            {
                castFreeze = true;
            }
            else
            {
                castFreeze = false;
            }

            StartCoroutine(RandomPerSec());
        }

        /* Функция получения урона от пули игрока */
        public override void BulletDamage(float playerBulletDamage, int shieldDamageBoost)
        {

            if (shieldPrefab != null && shieldPrefab.activeInHierarchy)
            {
                //Debug.Log("EnemyShieldDamaged!");
                shield -= (playerBulletDamage * shieldDamageBoost);  //Если щиты активны то перераспределяем урон на них
                shieldSlider.value = shield;
                if (shield <= 0f)
                {
                    shieldSlider.value = 0f;
                    shieldPrefab.SetActive(false);                        //Если щит исчерпался - выключаем его
                }
            }
            else if (gameObject != null && gameObject.activeInHierarchy)
            {
                health -= playerBulletDamage;                        //Если нет щитов и объект ещё жив - отнимаем здоровье
                healthSlider.value = health;

                if (health <= 0f)
                {
                    healthSlider.value = 0f;
                    gameObject.SetActive(false);                          //Уничтожение объекта
                }

            }



        }


        /* Функция получения урона от метеорита */
        public override void MeteorDamage(float meteorDamage, int shieldDamageBoost)
        {
            if (shieldPrefab.activeInHierarchy)
            {
                shield -= (meteorDamage * shieldDamageBoost);
                shieldSlider.value = shield;
                if (shield <= 0f)
                {
                    shieldSlider.value = 0f;
                    shieldPrefab.SetActive(false);
                }
            }
            else if (gameObject.activeInHierarchy)
            {
                health -= meteorDamage;
                healthSlider.value = health;

                if (health <= 0f)
                {
                    healthSlider.value = 0f;
                    gameObject.SetActive(false);                          //Уничтожение объекта
                }
            }
        }


        /* Функция получения урона от ракеты игрока */
        public override void MissileDamage(float playerMissileDamage, int shieldDamageBoost)
        {

            if (shieldPrefab.activeInHierarchy)
            {
                shield -= (playerMissileDamage * shieldDamageBoost);
                shieldSlider.value = shield;
                if (shield <= 0f)
                {
                    shieldSlider.value = 0f;
                    shieldPrefab.SetActive(false);
                }
            }
            else if (gameObject.activeInHierarchy)
            {
                health -= playerMissileDamage;
                healthSlider.value = health;

                if (health <= 0f)
                {
                    healthSlider.value = 0f;
                    gameObject.SetActive(false);                          //Уничтожение объекта
                }
            }
        }

        public override void LaunchMissile()
        {
            if (gameObject.activeInHierarchy)
            {
                GameObject missileObject = ObjectPoolingManager.Instance.GetBossMissile(missilePrefab);


                //Меняем борт запуска
                if (MissileEmitterChange)
                {
                    missileObject.transform.position = RightMissileEmitter.position;
                    MissileEmitterChange = false;
                }
                else
                {
                    missileObject.transform.position = LeftMissileEmitter.position;
                    MissileEmitterChange = true;
                }
                missileObject.transform.forward = transform.forward;
            }
        }

    }
}