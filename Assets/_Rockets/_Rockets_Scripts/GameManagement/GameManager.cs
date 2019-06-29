using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;

namespace Rockets
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;

        public GameObject Player;
        public GameObject Enemy;

        public int NumberOfLevel;

        public Slider PlayerHealth;
        public Slider PlayerShield;
        public Slider EnemyHealth;
        public Slider EnemyShield;

        public GameObject PlayerBullet;
        public GameObject PlayerMissile;

        public GameObject FirstBossBullet;
        public GameObject SecondBossBullet;
        public GameObject ThirdBossBullet;

        public GameObject SecondBossMissile;
        public GameObject ThirdBossMissile;

        public FrostEffectBehaviour FrostAnimationEffect;   //Связь со скриптом умения 2-го босса -- "Заморозка"
        public PetrifiedEffectBehaviour PetrifiedAnimationEffect; //Связь со скриптом умения 3-го босса -- "Каменные Оковы"

        

        public int Win;  //Переменная определяет победителя

        public int Coins;
        private int currentCoins;

        private bool coinsCalculated = false;

        private PlayerRotationScript PlayerScript;
        private EnemyRotationScript EnemyScript;

        public static bool gameIsGoing = false;
        public static bool countdownGameStarted = false;                //начался ли обратный отсчеt

        public TextMeshProUGUI timerText;
        public Animator timerTextAnim;
        public float startGameTime;
        private float timer;

        private float tempOriginPlayerSpeed;
        private float tempOriginEnemySpeed;

        public Button ShootButton;
        public Button MissileButton;
        public RectTransform FireSystem;
        public RectTransform joystick;
        public Slider reloadSlider;

        public Button LeftButton;
        public Button RightButton;

        private bool OneTimeFlag1 = false;
        private bool OneTimeFlag2 = false;

        public float GameTime;

        public float LooseTime;
        private float looseTimer;

        private float SafetyExitTime = 0f;
        void Awake()
        {

            //PlayerPrefs.SetInt("SetControl", 0);
            Time.timeScale = 1;
            StartCoroutine(StartGame());

            Player = GameObject.FindGameObjectWithTag("Player");
            Player.SetActive(true);
            PlayerScript = Player.GetComponent<PlayerRotationScript>();

            if (PlayerPrefs.HasKey("Coins"))
                Coins = PlayerPrefs.GetInt("Coins");

            

            #region Singleton;
            if (Instance != null)
            {
                return;
            }
            else
            {
                Instance = this;
            }
            #endregion
        }

        
        void Start()
        {
            
            if (NumberOfLevel == 0)
            {
                Enemy = GameObject.FindWithTag("FirstBoss");
                Enemy.SetActive(true);
                EnemyScript = Enemy.GetComponent<EnemyRotationScript>();
                
            }
            else if (NumberOfLevel == 1)
            {
                Enemy = GameObject.FindWithTag("SecondBoss");
                Enemy.SetActive(true);
                EnemyScript = Enemy.GetComponent<EnemyRotationScript>();
            }
            else if (NumberOfLevel == 2)
            {
                Enemy = GameObject.FindWithTag("ThirdBoss");
                Enemy.SetActive(true);
                EnemyScript = Enemy.GetComponent<EnemyRotationScript>();
            }

            FrostAnimationEffect = GameObject.FindGameObjectWithTag("FrostEffect").GetComponent<FrostEffectBehaviour>();
            PetrifiedAnimationEffect = GameObject.FindGameObjectWithTag("PetrifiedEffect").GetComponent<PetrifiedEffectBehaviour>();

            Win = 0;

            timer = startGameTime;
            looseTimer = LooseTime;

            PlayerHealth = GameObject.FindGameObjectWithTag("PlayerHealthBar").GetComponent<Slider>();
            PlayerShield = GameObject.FindGameObjectWithTag("PlayerShieldBar").GetComponent<Slider>();
            EnemyHealth = GameObject.FindGameObjectWithTag("EnemyHealthBar").GetComponent<Slider>();
            EnemyShield = GameObject.FindGameObjectWithTag("EnemyShieldBar").GetComponent<Slider>();


            SetControl();
            SwapButtons();
        }

        private void Update()
        {

            ChangeStatsForStartStop();

            if (gameIsGoing)
            {
                GameTime += Time.deltaTime;
                looseTimer -= Time.deltaTime;
            }

            SafetyExitFunc();

            if (PlayerHealth.value == 0f)
            {
                Player.SetActive(false);

                if (NumberOfLevel == 1)
                    FrostAnimationEffect.FrostFadeOut();
                else if (NumberOfLevel == 2)
                    PetrifiedAnimationEffect.PetrifiedFadeOut();

                Win = 2;

                if (!coinsCalculated)
                {
                    CountCoins();
                }
                ShootButton.interactable = false;
                MissileButton.interactable = false;
                //enable gameover menu
                //gameOverMenu.SetActive(true);
                //set gameover board values
                //GameOverManager.Instance.SetValues(win, gameTime, 0);
                //GameOverManager.Instance.SetCarPrefs(win);
            }
            else if (EnemyHealth.value == 0f)
            {
                Enemy.SetActive(false);

                Win = 1;
                if (!coinsCalculated)
                {
                    CountCoins();
                }
                //enable gameover menu
                //gameOverMenu.SetActive(true);
                //set gameover board values
                //GameOverManager.Instance.SetValues(win, gameTime, 0);
                //GameOverManager.Instance.SetCarPrefs(win);
                FrostAnimationEffect.FrostFadeOut();
            }
            else
            {
                //enable gameover menu
                //gameOverMenu.SetActive(true);
                //set gameover board values
                //GameOverManager.Instance.SetValues(win, gameTime, 0);
                //GameOverManager.Instance.SetCarPrefs(win);
            }

            if (looseTimer <= 0f)
            {
                Win = 2;
                Debug.Log("GameOver");
            }
        }

        public void SafetyExitFunc()
        {
            if (gameIsGoing)
            {
                if (Win == 0)
                {
                    Win = -1;
                }
            }
            //Вызов ГэймОверМенеджера

        }

        public void ChangeStatsForStartStop()
        {
            if (!gameIsGoing)
            {
                if (!OneTimeFlag1)
                {
                    tempOriginPlayerSpeed = Player.GetComponent<PlayerRotationScript>().speed;
                    tempOriginEnemySpeed = EnemyScript.speed;

                    Player.GetComponent<PlayerRotationScript>().speed = 0f;
                    EnemyScript.speed = 0f;

                    EnemyScript.ShootActive = false;
                    Player.GetComponent<PlayerRotationScript>().freezeRotation = true;

                    ShootButton.interactable = false;
                    MissileButton.interactable = false;

                    OneTimeFlag1 = true;
                    OneTimeFlag2 = false;
                }

        }
            else
            {
                if (!OneTimeFlag2)
                {
                    Player.GetComponent<PlayerRotationScript>().speed = tempOriginPlayerSpeed;
                    EnemyScript.speed = tempOriginEnemySpeed;
                    EnemyScript.ShootActive = true;
                    Player.GetComponent<PlayerRotationScript>().freezeRotation = false;
                    ShootButton.interactable = true;
                    MissileButton.interactable = true;
                    OneTimeFlag1 = false;
                    OneTimeFlag2 = true;
                }
            }
        }

        //нужно вызвать для переключения положения кнопок
        public void SwapButtons()
        {
            //меняем местами кнопки
            if (PlayerPrefs.GetInt("SetControl", 0) == 0)
            {
                float xTemp = joystick.position.x;
                joystick.position = new Vector3(FireSystem.position.x, joystick.position.y, 0);
                FireSystem.position = new Vector3(xTemp, FireSystem.position.y, 0);
            }
        }

        public void SetControl()
        {
            LeftButton.interactable = !Convert.ToBoolean(PlayerPrefs.GetInt("SetControl", 0));
            RightButton.interactable = Convert.ToBoolean(PlayerPrefs.GetInt("SetControl", 0));
            
        }

        public void ChangeControl()
        {
            
            PlayerPrefs.SetInt("SetControl", SetBool(PlayerPrefs.GetInt("SetControl", 0)));
            SetControl();
            SwapButtons();

        }

        public int SetBool(int i)
        {
            if (i == 0)
                return 1;
            else
                return 0;
        }
        //Функция подсчёта монет
        public void CountCoins()
        {
            if (Win == 2)
            {
                currentCoins = (int)(EnemyHealth.maxValue - EnemyHealth.value);
            }
            else if (Win == 1)
            {
                currentCoins = (int)(EnemyHealth.maxValue + PlayerHealth.value);
            }
            else
            {
                currentCoins = 0;
            }

            Coins += currentCoins;

            if (PlayerPrefs.HasKey("Coins"))
                PlayerPrefs.SetInt("Coins", Coins);

            coinsCalculated = true;
        }



        public IEnumerator StartGame()
        {

            //start countdown check
            countdownGameStarted = true;

            yield return StartCoroutine(StartCountdown());

            //flag that game is started
            gameIsGoing = true;
            countdownGameStarted = false;
            //start moving of cars
        }


        //use if game is paused or stopped
        public void StopGame()
    {
        //turn off the game
        gameIsGoing = false;

        //turn off the countdownTimer
        countdownGameStarted = false;
    }

    public IEnumerator StartCountdown()
    {
        //back to standart size of timer
        timerTextAnim.SetBool("GameStarted", false);

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            timerText.text = Mathf.Ceil(timer).ToString();
            yield return null;
        }

        timerText.text = "GO!";
        //start anim
        timerTextAnim.SetBool("GameStarted", true);
        //set standart value for timer
        timer = startGameTime;

            
        }

   }
}
    
