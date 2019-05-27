using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

namespace Planes
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] [Tooltip("Коэффициент, на который умножаются очки при победе")]
        private int buttonPosition = 0; //положение кнопкок: 0 - джойстик слева, 1 - джойстик справа
        
        public static GameManager instance = null;

        private bool paused = false; //указывает, нажата ли пауза
        private GameObject countdown;
        private Animator timerTextAnim;
        private TextMeshProUGUI timerText;
        private float startGameTime = 3f;
        public static bool gameIsGoing = false;
        public static bool countdownGameStarted = false;                //начался ли обратный отсчеt
        //private GameObject _pauseScreen; //меню паузы
        
        private Transform _joystick; //доступ к джойстику
        private Transform _fireSystem; //доступ к кнопке огонь
        private float timer;

        //итоги игры
        public bool gameEnded = false;
        private int win; //результат игры: 0 - вышел в начале, 1 - победа, 2 - поражение
        //public float score; //количество заработанных монет

        public float timeOfGame = 0f;
        
        public Button Left;
        public Button Right;

        CharacterPrint enemyInfo;
        CharacterPrint playerInfo;
        public int replay; //получить из PlayerPrefs
        public float score;
        public float lastHighScore; //из PlayerPrefs


        private void Awake()
        {
            #region Singleton
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
			#endregion

			//_pauseScreen = GameObject.FindWithTag("PauseScreen");
			//_pauseScreen.SetActive(false);
			Time.timeScale = 1;
            countdown = GameObject.FindWithTag("Timer");
            timerTextAnim = countdown.GetComponent<Animator>();
            timerText = countdown.GetComponent<TextMeshProUGUI>();
            _joystick = GameObject.FindWithTag("Joystick").GetComponent<RectTransform>();
            _fireSystem = GameObject.FindWithTag("FireSystem").GetComponent<RectTransform>();
            timer = startGameTime;
            if (PlayerPrefs.GetInt("SetControl") != buttonPosition)
            {
                //меняем местами кнопки
                float xTemp = _joystick.position.x;
                _joystick.position = new Vector3(_fireSystem.position.x, _joystick.position.y, 0);
                _fireSystem.position = new Vector3(xTemp, _fireSystem.position.y, 0);
            }
            buttonPosition = PlayerPrefs.GetInt("SetControl");
            SetControl();
            
            enemyInfo = GameObject.FindWithTag("Enemy").GetComponent<CharacterPrint>();
            playerInfo = GameObject.FindWithTag("Player").GetComponent<CharacterPrint>();

            Time.timeScale = 1;
            paused = false;
            StartCoroutine(StartGame());
        }

        private void Update()
        {
            if(gameIsGoing)
            {
                if (win == 0)
                {
                    win = -1;
                }
                timeOfGame += Time.deltaTime;
            }
        }

        //ставит/убирает паузу при нажатии на кнопку
        public void PauseButtonPressed()
        {
            if (paused) //если пауза нажата, а мы нажимаем кнопку, то отжимаем паузу
            {
                Debug.Log("Pause unpressed");
                paused = false; //установка нового состояния паузы
                PauseMenu.Instance.PauseOff(); //запускаем ход времени
                return;
            }
            if (!paused)
            {
                Debug.Log("Pause pressed");
                paused = true; //установка нового состояния паузы
                PauseMenu.Instance.PauseOn(); //останавливаем ход времени
                return;
            }
        }
        
        
        public void SwapButtons()
        {
            ChangeControl();
            if (PlayerPrefs.HasKey("SetControl"))
            {
                if (PlayerPrefs.GetInt("SetControl") != buttonPosition)
                {
                    Debug.Log("SwapButtons");
                    //меняем местами кнопки
                    float xTemp = _joystick.position.x;
                    _joystick.position = new Vector3(_fireSystem.position.x, _joystick.position.y, 0);
                    _fireSystem.position = new Vector3(xTemp, _fireSystem.position.y, 0);
                    //обновляем состояние переменной
                    buttonPosition = SetBool(buttonPosition);
                }
            }
        }

        public int SetBool(int i)
        {
            if (i == 0)
                return 1;
            else
                return 0;
        }

        public void SetControl()
        {
            Left.interactable = Convert.ToBoolean(PlayerPrefs.GetInt("SetControl", 0));
            Right.interactable = !Convert.ToBoolean(PlayerPrefs.GetInt("SetControl", 0));
        }


        public void ChangeControl()
        {
            Debug.Log("ChangeControl");
            PlayerPrefs.SetInt("SetControl", SetBool(PlayerPrefs.GetInt("SetControl", 0)));
            SetControl();
        }

        public void EndGame(CharacterPrint deadPlane) //конец игры: подсчитывает результаты
        {
            if (deadPlane.isPlayer) //если убит игрок
            {
                win = 2;
               /* GameObject enemyObj = GameObject.FindWithTag("Enemy");
                CharacterPrint enemy = enemyObj.GetComponent<CharacterPrint>();*/
                Debug.Log("You lost!");
            }
            else if (!deadPlane.isPlayer) //если убит враг
            {
                win = 1;
                //CharacterPrint player = GameObject.FindWithTag("Player").GetComponent<CharacterPrint>();
               // score = (player.health + deadPlane.maxHealth) * coefficient;
                Debug.Log("You won!");
            }
            gameEnded = true;
            score = timeOfGame;
            //gameOverMenu.SetActive(true);
            //GameOverManager.Instance.SetValues(win, gameTime, CountCoins());
            //GameOverManager.Instance.SetPlanesPrefs(win);
            Debug.Log("Your Score: " + CountCoins());
        }

        private int CountCoins ()
        {
            int coins;
            float coefficient, lastHighScore;
            int replay, playerLevel, openBossNumber;
            coefficient = 0.2f;
            replay = 1; //PlayerPrefs
            playerLevel = 1; //PlayerPrefs
            lastHighScore = 0; //PlayerPrefs
            openBossNumber = 1; //PLayerPrefs
            coins = Convert.ToInt32(
            Math.Round(
               Convert.ToDouble(
                   (enemyInfo.maxHealth - enemyInfo.health + playerInfo.health) * coefficient * Math.Pow(2, -(replay - 1))
                   )
               )
           );
            if (enemyInfo.bossNumber - playerLevel >= 0)
            {
                coins *= enemyInfo.bossNumber - playerLevel + 1;
            }
            if (lastHighScore != 0 && timeOfGame < lastHighScore && playerInfo.health > 0)
            {
                coins += Convert.ToInt32(Math.Round(lastHighScore - timeOfGame));
                if (enemyInfo.bossNumber - playerLevel >= 0)
                {
                    coins *= enemyInfo.bossNumber - playerLevel + 1;
                }
            }
            if (openBossNumber == 3 && replay > 1)
            {
                coins = 1;
            }
            /*int coins = Convert.ToInt32(
                Math.Round(
                    Convert.ToDouble(
                        (enemyInfo.maxHealth - enemyInfo.health + playerInfo.health) * coefficient * Math.Pow(2, replay - 1) * (enemyInfo.bossNumber - PlayerPrefs.GetInt("PlayerPlane") + 1) 
                        )
                    )
                );
            if (score > lastHighScore)
            {
                coins += Convert.ToInt32(Math.Round((score - lastHighScore) * (enemyInfo.bossNumber - PlayerPrefs.GetInt("PlayerPlane") + 1)));
            }
            if (enemyInfo.bossNumber == 3 && replay > 1)
            {
                coins = 1;
            }*/
            return coins;
        }


        public IEnumerator StartCountdown()
        {
            Debug.Log("StartCountdown()");
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
        }

        public IEnumerator StartGame()
        {
            //start countdown check
            Debug.Log("Start Game func");
            countdownGameStarted = true;

            timer = startGameTime;

            //_pauseScreen.SetActive(false);

            yield return StartCoroutine(StartCountdown());

            //flag that game is started
            gameIsGoing = true;

        }

        //use if game is paused or stopped
        public void StopGame()
        {
            Debug.Log("StopGame()");
            //turn off the game
            gameIsGoing = false;

            //turn off the countdownTimer
            countdownGameStarted = false;

            //_pauseScreen.SetActive(true);
        }

        public void ExitGame()
        {
            Debug.Log("Player exit");
            /* if (win == -1)
             {
                 //обнуление прогресса
                 //выход в меню
             }
             else if (win == 0)
             {
                 //выход в меню
             }*/

            //
            //enable gameover menu
            //gameOverMenu.SetActive(true);
            //set gameover board values
            //GameOverManager.Instance.SetValues(win, gameTime, 0);
            //GameOverManager.Instance.SetPlanesPrefs(win);
        }


    }
}