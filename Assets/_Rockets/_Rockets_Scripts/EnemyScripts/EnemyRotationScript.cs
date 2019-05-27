using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Rockets
{
    public class EnemyRotationScript : MonoBehaviour
    {

        Transform tr;

        public GameObject player;
        public Transform ShootEmitter;

        public Transform RightMissileEmitter;  //Ракетный барабан правого борта
        public Transform LeftMissileEmitter;   //Ракетный барабан левого борта
        public GameObject MissilePrefab;
        private bool MissileEmitterChange;                //Флаг смены ракетного барабана

        public Transform enemyparent;   //Пустой объект который выполняет движение по планете и хранит в себе объект врага

        public int freezeProbabilityInPercent;  // Вероятность кастования умения в процентном соотношении
        public float freezeDuration;      //Длительность умения "Заморозка"
        public float freezeDamage;        //Урон заморозки за каждый фрэйм(примерно 2 милисекунды)

        public int missileProbabilityInPercent;  //Вероятность запуска ракеты

        public float enemyHealth;                  
        public float enemyShield;

        public GameObject BulletPrefab;
        private Slider enemyHealthBar;   //Полоса здоровья
        private Slider enemyShieldBar;   //Полоса щитов
        public GameObject ShieldPrefab;  //Щит врага



        private float timeBetweenShots;    //Вспомогательный таймер для скорости стрельбы
        public float startBetweenShots;    //Скорость(период) стрельбы 

        public bool castFreeze;            //Сигнал кастования умения "Заморозка"
        public bool launchMissile;         //Сигнал запуска ракеты
        public bool launchDeathStorm;      //Muhahahahahahah

        public int NumberOfLevel;          //Номер планеты 

        private PlayerRotationScript playerStats;  //Связь со скриптом игрока
        private EnemyPursuit enemyPursuit;         //Родитель

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

        [HideInInspector]
        public bool ShootActive;

        
        public float speed;

        public float rotationSpeed;
        public float stoppingDistance;
        public float retreatingDistance;

        void Awake()
        {

            player = GameObject.FindGameObjectWithTag("Player");
            playerStats = player.GetComponent<PlayerRotationScript>();

        }
        void Start()
        {
            //NumberOfLevel = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>().NumberOfLevel;

            tr = GetComponent<Transform>();


            enemyHealthBar = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponent<Slider>();
            enemyShieldBar = GameObject.FindGameObjectWithTag("EnemyShieldBar").GetComponent<Slider>();

            enemyPursuit = enemyparent.GetComponent<EnemyPursuit>();

            enemyPursuit.stoppingDistance = stoppingDistance;
            enemyPursuit.retreatingDistance = retreatingDistance;
            enemyPursuit.speed = speed;
            enemyPursuit.rotationSpeed = rotationSpeed;


            originStoppingDistance = stoppingDistance;     //Сохраняем изначальную дистанцию остановки
            originRetreatingDistance = retreatingDistance; //И дистанцию отступления
            originSpeed = speed;
            originRotationSpeed = rotationSpeed;
            originMissileProbability = missileProbabilityInPercent;
            originFreezeProbability = freezeProbabilityInPercent;

            ShieldPrefab.SetActive(true); //Активируем щиты в начале

            timeBetweenShots = startBetweenShots;
            //timeBetweenLaunches = startBetweenLaunches;

            tr.rotation = enemyparent.rotation; //На всякий случай переприсваиваем напраление вращения родителя

            castFreeze = false;                 //В начале не кастуем умений

            RandomDigit1 = 102;                  //В начале задаём число не входящее в наш промежуток
            RandomDigit2 = 102;

            StartCoroutine(RandomPerSec());  //Задаёт сигнал для кастования умения один раз в заданный период времени(5 сек по стандарту)
        }

        void Update()
        {
            /* Здесь можно подключить тот или иной общий стиль поведения :*/
            if (gameObject.CompareTag("FirstBoss"))
            {

                Enemy1Behaviour();
            }
            if (gameObject.CompareTag("SecondBoss"))
            {
                Enemy2Behaviour();
            }
            if (gameObject.CompareTag("ThirdBoss"))
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
            if (enemyHealth <= 50)
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
            if (enemyHealth <= 50)
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
            if (enemyHealth <= 80 && enemyHealth > 50)
            {
                startBetweenShots = 0.5f;
                speed = originSpeed * 1.5f;
                rotationSpeed = originRotationSpeed * 1.5f;
                stoppingDistance = originStoppingDistance / 2 + 0.2f;
                retreatingDistance = originRetreatingDistance / 2 + 0.2f;
                freezeProbabilityInPercent = (int)(originFreezeProbability * 1.2f);
                missileProbabilityInPercent = (int)(originMissileProbability * 1.2f);
            }
            else if (enemyHealth <= 50 && enemyHealth > 10)
            {
                startBetweenShots = 0.3f;
                speed = originSpeed * 1.5f;
                rotationSpeed = originRotationSpeed * 1.5f;
                stoppingDistance = originStoppingDistance / 2 + 0.2f;
                retreatingDistance = originRetreatingDistance / 2 + 0.2f;
                freezeProbabilityInPercent = (int)(originFreezeProbability * 1.4f);
                missileProbabilityInPercent = (int)(originMissileProbability * 1.4f);
            }
            else if (enemyHealth <= 10)
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

        public void Shoot()
        {
            GameObject bulletObject = ObjectPoolingManager.Instance.GetEnemyBullet(BulletPrefab);  //Вызываем пулю из пула:)
            bulletObject.transform.position = ShootEmitter.position;
            //bulletObject.transform.up = ShootEmitter.up;                                              //Ставим её на позицию выстрелов
            bulletObject.transform.forward = ShootEmitter.forward;                                    //Направляем по направлению движения врага

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
        public void BulletDamage(float playerBulletDamage, int shieldDamageBoost)
        {

            if (ShieldPrefab.activeInHierarchy)
            {
                //Debug.Log("EnemyShieldDamaged!");
                enemyShield -= (playerBulletDamage * shieldDamageBoost);  //Если щиты активны то перераспределяем урон на них
                enemyShieldBar.value = enemyShield;
                if (enemyShield <= 0f)
                {
                    enemyShieldBar.value = 0f;
                    ShieldPrefab.SetActive(false);                        //Если щит исчерпался - выключаем его
                }
            }
            else if (gameObject.activeInHierarchy)
            {
                enemyHealth -= playerBulletDamage;                        //Если нет щитов и объект ещё жив - отнимаем здоровье
                enemyHealthBar.value = enemyHealth;

                if (enemyHealth <= 0f)
                {
                    enemyHealthBar.value = 0f;
                    gameObject.SetActive(false);                          //Уничтожение объекта
                }

            }



        }


        /* Функция получения урона от метеорита */
        public void MeteorDamage(float meteorDamage, int shieldDamageBoost)
        {
            if (ShieldPrefab.activeInHierarchy)
            {
                enemyShield -= (meteorDamage * shieldDamageBoost);
                enemyShieldBar.value = enemyShield;
                if (enemyShield <= 0f)
                {
                    enemyShieldBar.value = 0f;
                    ShieldPrefab.SetActive(false);
                }
            }
            else if (gameObject.activeInHierarchy)
            {
                enemyHealth -= meteorDamage;
                enemyHealthBar.value = enemyHealth;

                if (enemyHealth <= 0f)
                {
                    enemyHealthBar.value = 0f;
                    gameObject.SetActive(false);                          //Уничтожение объекта
                }
            }
        }


        /* Функция получения урона от ракеты игрока */
        public void MissileDamage(float playerMissileDamage, int shieldDamageBoost)
        {

            if (ShieldPrefab.activeInHierarchy)
            {
                enemyShield -= (playerMissileDamage * shieldDamageBoost);
                enemyShieldBar.value = enemyShield;
                if (enemyShield <= 0f)
                {
                    enemyShieldBar.value = 0f;
                    ShieldPrefab.SetActive(false);
                }
            }
            else if (gameObject.activeInHierarchy)
            {
                enemyHealth -= playerMissileDamage;
                enemyHealthBar.value = enemyHealth;

                if (enemyHealth <= 0f)
                {
                    enemyHealthBar.value = 0f;
                    gameObject.SetActive(false);                          //Уничтожение объекта
                }
            }
        }

        //Функция запуска ракеты
        public void LaunchMissile()
        {
            if (gameObject.activeInHierarchy)
            {
                GameObject missileObject = ObjectPoolingManager.Instance.GetBossMissile(MissilePrefab);


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