using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System;

namespace Planes
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance = null;

        [Header("For Pause Function")]
        //таймер
        [SerializeField] [Tooltip("Анимация текста таймера")]
        private Animator timerTextAnim;
        [SerializeField] [Tooltip("Текст таймера")]
        private TextMeshProUGUI timerText;
        
        [Header("Setting Menu Components")]
        [SerializeField] [Tooltip("положение кнопкок: 0 - джойстик слева, 1 - джойстик справа")]
        private int buttonPosition = 0; //положение кнопкок: 0 - джойстик слева, 1 - джойстик справа
        [SerializeField] [Tooltip("Джойстик (для того, чтобы менять кнопки местами)")]
        private Transform _joystick; //доступ к джойстику
        [SerializeField] [Tooltip("Кнопка огонь")]
        private Transform _fireSystem; //доступ к кнопке огонь
        [SerializeField] [Tooltip("Выделенное положение кнопок")]
        private Button left;
        [SerializeField] [Tooltip("Выделенное положение кнопок")]
        private Button right;
        [SerializeField] [Tooltip("Окошко с завершением игры")]
        private GameObject gameOverMenu;

        private float startGameTime = 3f; //время, которое запускается игра (время, пока отсчитывается таймер)
        public static bool gameIsGoing = false; //указывает, идет ли игра в данный момент
        public static bool countdownGameStarted = false; //начался ли обратный отсчет
               
        private float timer; //таймер для отсчета таймера начала игры

        //итоги игры
        [HideInInspector]
        public bool gameEnded = false; //закончилась ли игра
        private int win; //результат игры: 0 - вышел в начале, 1 - победа, 2 - поражение? -1 - вышел с потерей прогресса
        public float timeOfGame = 0f; //длительность игры
        
        //Подсчет монеток (не реализован до конца)
        private CharacterPrint enemyInfo;
        private CharacterPrint playerInfo;
        // private int replay; //получить из PlayerPrefs
        // private float lastHighScore; //из PlayerPrefs
                
        private void Awake()
        {
            #region Singleton
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
			#endregion
            
            enemyInfo = GameObject.FindWithTag("Enemy").GetComponent<CharacterPrint>();
            playerInfo = GameObject.FindWithTag("Player").GetComponent<CharacterPrint>();

            Time.timeScale = 1;
         //   StartCoroutine(StartGame());

            SetControl();
            SwapButtons();
            timerText.text = "";
        }

        private void Update()
        {
            if (gameIsGoing) //считает длительность игры
            {
                if (win == 0)
                {
                    win = -1;
                }
                timeOfGame += Time.deltaTime;
            }
        }
        
        public void SwapButtons()
        {
            Debug.Log("SwapButtons");
            //меняем местами кнопки
            if (PlayerPrefs.GetInt("SetControl", 0) != buttonPosition)
            {
                float xTemp = _joystick.position.x;
                _joystick.position = new Vector3(_fireSystem.position.x, _joystick.position.y, 0);
                _fireSystem.position = new Vector3(xTemp, _fireSystem.position.y, 0);
                buttonPosition = SetBool(buttonPosition);
            }
        }

        public int SetBool(int i)
        {
            if (i == 0)
                return 1;
            else
                return 0;
        }

        public void SetControl() //устанавливает interactible кнопок в соответствии с их позицией
        {
            left.interactable = Convert.ToBoolean(PlayerPrefs.GetInt("SetControl", 0));
            right.interactable = !Convert.ToBoolean(PlayerPrefs.GetInt("SetControl", 0));
        }


        public void ChangeControl() //меняет значение в префс на противоположное
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
                Debug.Log("You lost!");
            }
            else if (!deadPlane.isPlayer) //если убит враг
            {
                win = 1;
                Debug.Log("You won!");
            }
            gameEnded = true;
            //GameOverManager.Instance.SetPlanesPrefs(win);
            Debug.Log("Your Score: " + CountCoins());
         //   StartCoroutine(CallGameOverScreen());
            Invoke("CallGameOverScreen", 3f);
        }


        void CallGameOverScreen()
        {
      //      yield return new WaitForSeconds(3f);
            gameOverMenu.SetActive(true);
            GameOverManager.Instance.SetValues(win, timeOfGame, CountCoins());
        }

        private int CountCoins () //подсчет монеток за уровень; не реализовано
        {
            int coins = 0;
            float coefficient = 1f;
            coins = Convert.ToInt32(
             Math.Round(
                Convert.ToDouble(
                    (enemyInfo.maxHealth - enemyInfo.health + playerInfo.health) * coefficient)
                )
            );
            if (enemyInfo.level - playerInfo.level >= 0)
            {
                coins *= enemyInfo.level - playerInfo.level + 1;
            }

            #region ОЧЕНЬ_СЛОЖНО
            /* 
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
            #endregion
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
        }

        public void ExitGame() //завершает игру, если игрок вышел
        {
            Debug.Log("Player exit");
            //enable gameover menu
            gameOverMenu.SetActive(true);
            //set gameover board values
            GameOverManager.Instance.SetValues(win, timeOfGame, 0);
            //GameOverManager.Instance.SetPlanePrefs(win);
        }
    }
}