using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Rockets
{
    public class PlayerRotationScript : ShipFactory
    {
        [HideInInspector]
        public Joystick joystick;

        private Slider reloadSlider;

        [HideInInspector]
        public FrostEffectBehaviour FrostAnimationEffect;   //Связь со скриптом умения 2-го босса -- "Заморозка"
        [HideInInspector]
        public PetrifiedEffectBehaviour PetrifiedAnimationEffect; //Связь со скриптом умения 3-го босса -- "Каменные Оковы"

        private Vector3 currentRotateDirection;
        private Vector3 previousRotateDirection;
        private Quaternion slerpRotation;

        private bool DamageInputFlag;                     //Флаг получения урона
        private float ShieldRecoveryTime;                 //Вспомогательный таймер восстановления щитов

        private float speedForFreeze;                     //Скорость при заморозке
        private float freezeDamagePerFrame;               //Урон от заморозки
        public bool freezeRotation;                      //Флаг остановки поворота          

        private float petrifyDamagePerFrame;
        private bool petrifyRotation;

        [HideInInspector]
        public int Win;                                   //Опреелитель победы, поражения, ничьи или выхода из боя

        private float ShieldRecoveryDelay = 5f;                 //Время которое игрок должен продержаться без получения урона, для восстановления щитов
        private float ShieldRecoveryValue = 0.1f;                 //Значение, на которое восполняется щит за каждый фрейм.

        private bool Reloaded = true;


        private int currentMissileAmount;
        [HideInInspector]
        public int startMissileAmount = 3;
        void Awake()
        {
            gameManager = GameManager.Instance;
            if (gameManager == null)
                Debug.Log("Fuck!!");
            #region Привязка всех объектов
            joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<FixedJoystick>();
            bulletPrefab = gameManager.PlayerBullet;
            missilePrefab = gameManager.PlayerMissile;
            shieldPrefab = GameObject.FindGameObjectWithTag("PlayerShield");
            healthSlider = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<Slider>();
            shieldSlider = GameObject.FindGameObjectWithTag("PlayerShieldBar").GetComponent<Slider>();
            reloadSlider = GameObject.FindGameObjectWithTag("ReloadSlider").GetComponent<Slider>();
            shootEmitter = GameObject.FindGameObjectWithTag("playerEmitter").GetComponent<Transform>();
            FrostAnimationEffect = GameObject.FindGameObjectWithTag("FrostEffect").GetComponent<FrostEffectBehaviour>();
            PetrifiedAnimationEffect = GameObject.FindGameObjectWithTag("PetrifiedEffect").GetComponent<PetrifiedEffectBehaviour>();
            #endregion
        }
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            tr = GetComponent<Transform>();
            gm = GetComponent<GameObject>();

            #region Устанавливаем все стартовые параметры
            healthSlider.value = startHealth;
            shieldSlider.value = startShield;
            health = startHealth;
            shield = startShield;
            speed = startSpeed;

            speedForFreeze = speed;

            freezeRotation = false;
            petrifyRotation = false;


            shieldPrefab.SetActive(true);             //Включаем щиты

            currentMissileAmount = startMissileAmount;

            DamageInputFlag = false;                  //Индикатор входящего урона

            ShieldRecoveryTime = ShieldRecoveryDelay; //Устанавливаем задержку перед началом восстановления щитов

            timeBetweenShots = startBetweenShots;

            previousRotateDirection = tr.localEulerAngles; //Вектор для слерпа

            //Устанавливаем параметры для передачи вне сцены
            Win = 0;
            #endregion
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
            if ((DamageInputFlag) && (shieldSlider.value < 100))  //Если был урон, и щиты не полные
            {
                //Debug.Log("ShiedTime!");
                if (ShieldRecoveryTime <= 0)
                {
                    //Восстанавливаем щиты, если продержались достаточно(И возвращаем их, если они были уничтожены)
                    if (shieldPrefab.activeInHierarchy)
                    {
                        shield += ShieldRecoveryValue;
                        shieldSlider.value = shield;
                    }
                    else
                    {

                        shieldPrefab.SetActive(true);
                        shield += ShieldRecoveryValue;
                        shieldSlider.value = shield;
                    }
                }
                else
                {
                    ShieldRecoveryTime -= Time.deltaTime;
                }
            }
        }


    public override void BulletDamage(float enemyBulletDamage, int shieldDamageBoost)
    {

            if (shieldPrefab != null && shieldPrefab.activeInHierarchy)
            {
                //Debug.Log("PlayerShieldDamaged!");
                shield -= (enemyBulletDamage * shieldDamageBoost);
                shieldSlider.value = shield;
                if (shield <= 0f)
                {
                    shieldSlider.value = 0f;
                    shieldPrefab.SetActive(false);
                }
            }
            else if (gameObject != null && gameObject.activeInHierarchy)
            {
                health -= enemyBulletDamage;
                healthSlider.value = health;

            }

            if (health <= 0f)
            {
                healthSlider.value = 0f;
                gameObject.SetActive(false);

            }
            else
            {
                DamageInputFlag = true;
                ShieldRecoveryTime = ShieldRecoveryDelay;
            }

        }

        public override void MissileDamage(float enemyMissileDamage, int shieldDamageBoost)
        {

            if (shieldPrefab != null && shieldPrefab.activeInHierarchy)
            {
                //Debug.Log("PlayerShieldDamaged!");
                shield -= (enemyMissileDamage * shieldDamageBoost);
                shieldSlider.value = shield;
                if (shield <= 0f)
                {
                    shieldSlider.value = 0f;
                    shieldPrefab.SetActive(false);
                }
            }
            else if (gameObject != null && gameObject.activeInHierarchy)
            {
                health -= enemyMissileDamage;
                healthSlider.value = health;

            }

            if (health <= 0f)
            {
                healthSlider.value = 0f;
                gameObject.SetActive(false);

            }
            else
            {
                DamageInputFlag = true;
                ShieldRecoveryTime = ShieldRecoveryDelay;
            }

        }
        public override void MeteorDamage(float meteorDamage, int shieldDamageBoost)
        {
            if (shieldPrefab != null && shieldPrefab.activeInHierarchy)
            {

                shield -= (meteorDamage * shieldDamageBoost);

                shieldSlider.value = shield;

                if (shield <= 0f)
                {
                    //playerShieldBar.value = 0f;
                    shieldPrefab.SetActive(false);
                }
                else
                {
                    shieldSlider.value = shield;
                }
            }
            else if (gameObject != null && gameObject.activeInHierarchy)
            {

                health -= meteorDamage;
                healthSlider.value = health;

            }

            if (health <= 0f)
            {
                healthSlider.value = 0f;
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
                speed = 0.2f;                       //Замедлем игрока
                freezeRotation = true;                  //Замораживаем поворот
                freezeDamagePerFrame = damagePerFrame;  //Обновляем урон

                FrostAnimationEffect.FrostFadeIn();     //Запускаем анимацию замерзания экрана

                yield return new WaitForSeconds(duration);

                speed = speedForFreeze;             //Возвращаем сохраненную скорость
                freezeRotation = false;
                FrostAnimationEffect.FrostFadeOut();    //Запускаем анимацию отмерзания экрана    
            }
        }

        /* Корутин влияния окаменения на игрока */
        IEnumerator PetrifyDuration(float duration, float damagePerFrame)
        {
            if (GameManager.gameIsGoing && !GameManager.countdownGameStarted)
            {
            speed = 0.2f;                       //Замедлем игрока
            petrifyRotation = true;                  //Замораживаем поворот
            petrifyDamagePerFrame = damagePerFrame;  //Обновляем урон

            PetrifiedAnimationEffect.PetrifiedFadeIn();     //Запускаем анимацию окаменения экрана

            yield return new WaitForSeconds(duration);

            speed = speedForFreeze;             //Возвращаем сохраненную скорость
            petrifyRotation = false;
            PetrifiedAnimationEffect.PetrifiedFadeOut();    //Запускаем анимацию откаменения хдд)) экрана    
            }
        }

        /* Основное поведение игрока и его реакция на еффекты извне*/
        void PlayerBehaviour()
        {
            if (freezeRotation)
            {
                health -= freezeDamagePerFrame; //Наносим урон от заморозки
                healthSlider.value = health;

                if (health <= 0f)
                {
                    healthSlider.value = 0f;
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
        }

        //Функция стрельбы
        public override void Shoot()
        {
            if (Reloaded)
            {
                if (gameObject.activeInHierarchy)
                {
                    GameObject bulletObject = ObjectPoolingManager.Instance.GetPlayerBullet(bulletPrefab);
                    bulletObject.transform.position = shootEmitter.position;
                    //bulletObject.transform.up = ShootEmitter.up;
                    bulletObject.transform.forward = shootEmitter.forward;
                }
                Reloaded = false;
                reloadSlider.value = 0f;
            }
            else
            {

            }
        }

        //Функция запуска ракеты
        public override void LaunchMissile()
        {
            if (currentMissileAmount > 0)
            {
                if (gameObject.activeInHierarchy)
                {
                    GameObject missileObject = ObjectPoolingManager.Instance.GetPlayerMissile(missilePrefab);


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
                currentMissileAmount--;
            }
        }
    }
}