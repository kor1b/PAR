using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Rockets
{
    public class PlayerRotationScript : MonoBehaviour
    {

        Rigidbody rb;
        Transform tr;
        GameObject gm;

        public Joystick joystick;

        public Transform ShootEmitter;         //Место для стрельбы пулями

        public Transform RightMissileEmitter;  //Ракетный барабан правого борта
        public Transform LeftMissileEmitter;   //Ракетный барабан левого борта

        public GameObject BulletPrefab;
        public GameObject ShieldPrefab;
        public GameObject MissilePrefab;

        public GameObject Enemy;

        private Slider playerHealthBar;
        private Slider playerShieldBar;
        private Slider reloadSlider;

        public FrostEffectBehaviour FrostAnimationEffect;   //Связь со скриптом умения 2-го босса -- "Заморозка"
        public PetrifiedEffectBehaviour PetrifiedAnimationEffect; //Связь со скриптом умения 3-го босса -- "Каменные Оковы"

        public GameManager gameController;

        private Vector3 currentRotateDirection;
        private Vector3 previousRotateDirection;
        private Quaternion slerpRotation;

        private bool DamageInputFlag;                     //Флаг получения урона
        private bool MissileEmitterChange;                //Флаг смены ракетного барабана
        private float ShieldRecoveryTime;                 //Вспомогательный таймер восстановления щитов

        private float speedForFreeze;                     //Скорость при заморозке
        private float freezeDamagePerFrame;               //Урон от заморозки
        public bool freezeRotation;                      //Флаг остановки поворота          

        private float petrifyDamagePerFrame;
        private bool petrifyRotation;

        public int Win;                                   //Опреелитель победы, поражения, ничьи или выхода из боя

        private float playerHealth;
        private float playerShield;

        public float moveSpeed;                           //Скорость движения

        public int MissilesStorage;
        public int currentMissileAmount;

        public float playerStartHealth;
        public float playerStartShield;

        public float ShieldRecoveryDelay;                 //Время которое игрок должен продержаться без получения урона, для восстановления щитов
        public float ShieldRecoveryValue;                 //Значение, на которое восполняется щит за каждый фрейм.

        private float timeBetweenShots;
        public float startBetweenShots;
        private bool Reloaded = true;
        void Awake()
        {
            //gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();

            gameController = GameManager.Instance;

            if (gameController == null)
                Debug.Log("Fuck!!");

            joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<FixedJoystick>();
            BulletPrefab = gameController.PlayerBullet;
            MissilePrefab = gameController.PlayerMissile;
            playerHealthBar = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<Slider>();
            playerShieldBar = GameObject.FindGameObjectWithTag("PlayerShieldBar").GetComponent<Slider>();
            reloadSlider = GameObject.FindGameObjectWithTag("ReloadSlider").GetComponent<Slider>();
            FrostAnimationEffect = GameObject.FindGameObjectWithTag("FrostEffect").GetComponent<FrostEffectBehaviour>();
            PetrifiedAnimationEffect = GameObject.FindGameObjectWithTag("PetrifiedEffect").GetComponent<PetrifiedEffectBehaviour>();

            //PlayerPrefs.SetInt("MissileStorage", 3);
            //MissilesStorage = PlayerPrefs.GetInt("MissileStorage");

            currentMissileAmount = MissilesStorage;
        }
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            tr = GetComponent<Transform>();
            gm = GetComponent<GameObject>();

            playerHealthBar.value = playerStartHealth;
            playerShieldBar.value = playerStartShield;
            playerHealth = playerStartHealth;
            playerShield = playerStartShield;

            speedForFreeze = moveSpeed;

            freezeRotation = false;
            petrifyRotation = false;


            ShieldPrefab.SetActive(true);             //Включаем щиты

            DamageInputFlag = false;                  //Индикатор входящего урона

            MissileEmitterChange = false;

            ShieldRecoveryTime = ShieldRecoveryDelay; //Устанавливаем задержку перед началом восстановления щитов

            timeBetweenShots = startBetweenShots;

            previousRotateDirection = tr.localEulerAngles; //Вектор для слерпа

            //Устанавливаем параметры для передачи вне сцены
            Win = 0;
        }

        void Update()
        {
            ShieldRecovery();                                               //Всегда хотим попробовать восстановить щиты

            /* Стрельба раз в определённый период времени*/
            if (timeBetweenShots <= 0)
            {
                Reloaded = true;
                timeBetweenShots = startBetweenShots;
            }
            else
            {
                timeBetweenShots -= Time.deltaTime;
            }
        }

        void FixedUpdate()
        {

            PlayerBehaviour();  //Поведение игрока(В основном движение и поворот, и эффекты накладываемые на них).

        }

        /* Функция восстановления щитов */
        void ShieldRecovery()
        {
            if ((DamageInputFlag) && (playerShieldBar.value < 100))  //Если был урон, и щиты не полные
            {
                //Debug.Log("ShiedTime!");
                if (ShieldRecoveryTime <= 0)
                {
                    //Восстанавливаем щиты, если продержались достаточно(И возвращаем их, если они были уничтожены)
                    if (ShieldPrefab.activeInHierarchy)
                    {
                        playerShield += ShieldRecoveryValue;
                        playerShieldBar.value = playerShield;
                    }
                    else
                    {

                        ShieldPrefab.SetActive(true);
                        playerShield += ShieldRecoveryValue;
                        playerShieldBar.value = playerShield;
                    }
                }
                else
                {
                    ShieldRecoveryTime -= Time.deltaTime;
                }
            }
        }


        public void BulletDamage(float enemyBulletDamage, int shieldDamageBoost)
        {

            if (ShieldPrefab != null && ShieldPrefab.activeInHierarchy)
            {
                //Debug.Log("PlayerShieldDamaged!");
                playerShield -= (enemyBulletDamage * shieldDamageBoost);
                playerShieldBar.value = playerShield;
                if (playerShield <= 0f)
                {
                    playerShieldBar.value = 0f;
                    ShieldPrefab.SetActive(false);
                }
            }
            else if (gameObject != null && gameObject.activeInHierarchy)
            {
                playerHealth -= enemyBulletDamage;
                playerHealthBar.value = playerHealth;

            }

            if (playerHealth <= 0f)
            {
                playerHealthBar.value = 0f;
                gameObject.SetActive(false);

            }
            else
            {
                DamageInputFlag = true;
                ShieldRecoveryTime = ShieldRecoveryDelay;
            }

        }

        public void MissileDamage(float enemyMissileDamage, int shieldDamageBoost)
        {

            if (ShieldPrefab != null && ShieldPrefab.activeInHierarchy)
            {
                //Debug.Log("PlayerShieldDamaged!");
                playerShield -= (enemyMissileDamage * shieldDamageBoost);
                playerShieldBar.value = playerShield;
                if (playerShield <= 0f)
                {
                    playerShieldBar.value = 0f;
                    ShieldPrefab.SetActive(false);
                }
            }
            else if (gameObject != null && gameObject.activeInHierarchy)
            {
                playerHealth -= enemyMissileDamage;
                playerHealthBar.value = playerHealth;

            }

            if (playerHealth <= 0f)
            {
                playerHealthBar.value = 0f;
                gameObject.SetActive(false);

            }
            else
            {
                DamageInputFlag = true;
                ShieldRecoveryTime = ShieldRecoveryDelay;
            }

        }
        public void MeteorDamage(float meteorDamage, int shieldDamageBoost)
        {
            if (ShieldPrefab != null && ShieldPrefab.activeInHierarchy)
            {

                playerShield -= (meteorDamage * shieldDamageBoost);

                playerShieldBar.value = playerShield;

                if (playerShield <= 0f)
                {
                    //playerShieldBar.value = 0f;
                    ShieldPrefab.SetActive(false);
                }
                else
                {
                    playerShieldBar.value = playerShield;
                }
            }
            else if (gameObject != null && gameObject.activeInHierarchy)
            {

                playerHealth -= meteorDamage;
                playerHealthBar.value = playerHealth;

            }

            if (playerHealth <= 0f)
            {
                playerHealthBar.value = 0f;
                gameObject.SetActive(false);

            }
            else
            {
                DamageInputFlag = true;
                ShieldRecoveryTime = ShieldRecoveryDelay;
            }
        }

        /* Функция влияния заморозки на игрока */
        public void FreezeCast(float duration, float damagePerFrame)
        {

            if (gameObject.activeInHierarchy && GameManager.gameIsGoing && !GameManager.countdownGameStarted)
                StartCoroutine(FreezeDuration(duration, damagePerFrame));

        }

        public void PetrifyCast(float duration, float damagePerFrame)
        {

            if (gameObject.activeInHierarchy && GameManager.gameIsGoing && !GameManager.countdownGameStarted)
                StartCoroutine(PetrifyDuration(duration, damagePerFrame));

        }

        /* Корутин влияния заморозки на игрока */
        IEnumerator FreezeDuration(float duration, float damagePerFrame)
        {
            if (GameManager.gameIsGoing && !GameManager.countdownGameStarted)
            {
                moveSpeed = 0.2f;                       //Замедлем игрока
                freezeRotation = true;                  //Замораживаем поворот
                freezeDamagePerFrame = damagePerFrame;  //Обновляем урон

                FrostAnimationEffect.FrostFadeIn();     //Запускаем анимацию замерзания экрана

                yield return new WaitForSeconds(duration);

                moveSpeed = speedForFreeze;             //Возвращаем сохраненную скорость
                freezeRotation = false;
                FrostAnimationEffect.FrostFadeOut();    //Запускаем анимацию отмерзания экрана    
            }
        }

        /* Корутин влияния окаменения на игрока */
        IEnumerator PetrifyDuration(float duration, float damagePerFrame)
        {
            if (GameManager.gameIsGoing && !GameManager.countdownGameStarted)
            {
                moveSpeed = 0.2f;                       //Замедлем игрока
            petrifyRotation = true;                  //Замораживаем поворот
            petrifyDamagePerFrame = damagePerFrame;  //Обновляем урон

            PetrifiedAnimationEffect.PetrifiedFadeIn();     //Запускаем анимацию окаменения экрана

            yield return new WaitForSeconds(duration);

            moveSpeed = speedForFreeze;             //Возвращаем сохраненную скорость
            petrifyRotation = false;
            PetrifiedAnimationEffect.PetrifiedFadeOut();    //Запускаем анимацию откаменения хдд)) экрана    
            }
        }

        /* Основное поведение игрока и его реакция на еффекты извне*/
        void PlayerBehaviour()
        {
            if (freezeRotation)
            {
                playerHealth -= freezeDamagePerFrame; //Наносим урон от заморозки
                playerHealthBar.value = playerHealth;

                if (playerHealth <= 0f)
                {
                    playerHealthBar.value = 0f;
                    gameObject.SetActive(false);
                    /*
                    if (!Enemy.activeInHierarchy)
                    {
                        Win = 3;
                        PlayerPrefs.SetInt("Win", Win);
                        PlayerPrefs.Save();
                    }
                    else
                    {
                        Win = 2;
                        PlayerPrefs.SetInt("Win", Win);
                        PlayerPrefs.Save();
                    }
                    */

                }
                else
                {
                    DamageInputFlag = true;
                    ShieldRecoveryTime = ShieldRecoveryDelay;
                }
            }
            //else
            //{
                float h1 = joystick.Horizontal; // set as your inputs 
                float v1 = joystick.Vertical;

                if (h1 == 0f && v1 == 0f)
                { // this statement allows it to recenter once the inputs are at zero 
                    Vector3 curRot = tr.localEulerAngles; // the object you are rotating
                    Vector3 homeRot;
                    if (curRot.y > 180f)
                    { // this section determines the direction it returns home 

                        homeRot = new Vector3(0f, 359.999f, 0f); //it doesnt return to perfect zero 
                    }
                    else
                    {                                                                      // otherwise it rotates wrong direction 
                        homeRot = Vector3.zero;
                    }
                    //tr.localEulerAngles = Vector3.Slerp(curRot, curRot, Time.deltaTime * 2);

                }
                else
                {

                    currentRotateDirection = new Vector3(0f, Mathf.Atan2(h1, v1) * 180 / Mathf.PI, 0f);

                    slerpRotation = Quaternion.LookRotation(currentRotateDirection, tr.up);
                    //Vector3 lerpQuaternion = Quaternion.Lerp(Quaternion.LookRotation(previousRotateDirection), slerpRotation, Time.deltaTime * 2).eulerAngles;

                    tr.localEulerAngles = currentRotateDirection;
                    //tr.rotation = Quaternion.Euler(0, lerpQuaternion.y, 0);


                }
            //}
        }

        //Функция стрельбы
        public void Shoot()
        {
            if (Reloaded)
            {
                if (gameObject.activeInHierarchy)
                {
                    GameObject bulletObject = ObjectPoolingManager.Instance.GetPlayerBullet(BulletPrefab);
                    bulletObject.transform.position = ShootEmitter.position;
                    //bulletObject.transform.up = ShootEmitter.up;
                    bulletObject.transform.forward = ShootEmitter.forward;
                }
                Reloaded = false;
                reloadSlider.value = 0f;
            }
            else
            {

            }
        }

        //Функция запуска ракеты
        public void LaunchMissile()
        {
            if (gameObject.activeInHierarchy && currentMissileAmount > 0)
            {
                GameObject missileObject = ObjectPoolingManager.Instance.GetPlayerMissile(MissilePrefab);

                currentMissileAmount--;
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