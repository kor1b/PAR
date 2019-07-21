using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using Newtonsoft.Json;
using System.IO;
public class NewBehaviourScript : MonoBehaviour
{


    public class MenuManager : MonoBehaviour
    {

        public GameObject playerObject;
        //playerObjects
        [SerializeField] PlayerObjects[] playerObjects;
        /*public GameObject[] playerObjectsCar;
        public GameObject[] playerObjectsPlane;
        public GameObject[] playerObjectsRocket;*/
        //player
        [SerializeField] Player[] player;
        /*public GameObject[] Car;
        public GameObject[] Plane;
        public GameObject[] Rocket;*/
        public GameObject[] gameMenu;
        //environment
        [SerializeField] Environment[] environment;
        /*public GameObject[] environmentCar;                        //массив targetImages, который будет заменяться на массив окружения выбранной игры 
        public GameObject[] environmentPlane;
        public GameObject[] environmentRocket;*/
        public GameObject selectedEnvironment;            //выбранное окружение, которое будет загружаться
                                                          //кнопки переключения управления
        public Button Left;
        public Button Right;
        //кнопка прокачки
        public Button Upgrade;
        //картинки активности боссов
        public Sprite imageMax;
        public Sprite imageUpgradeActive;
        public Sprite imageUpgradeDisabled;
        public Sprite imageCloseBoss;
        // картинки боссов
        [SerializeField] ImageBoss[] imageBoss;
        /*public Sprite[] imageBoss0;
        public Sprite[] imageBoss1;
        public Sprite[] imageBoss2;*/
        // картинки активных боссов
        [SerializeField] ImageBossActive[] imageBossActive;
        /*public Sprite[] imageBossActive0;
        public Sprite[] imageBossActive1;
        public Sprite[] imageBossActive2;*/
        //
        public Sprite[] imageBanknotes;
        public static Sprite imageBanknote;
        // кнопки боссов
        [SerializeField] Buttons[] buttons;
        /*public Button[] buttons0;
        public Button[] buttons1;
        public Button[] buttons2;*/

        //монеты
        public TextMeshProUGUI CoinsText;
        TextMeshProUGUI coinsText;
        //массив стоимости прокачки
        public int[] cost;
        public SaveData sd;
        void Start()
        {
            /*sd = File.Exists("savedata.json") ? JsonConvert.DeserializeObject<SaveData>(File.ReadAllText("savedata.json")) : new SaveData()
            {
               sCoins=0,

            };*/
            if (File.Exists("savedata.json"))
            {
                JsonConvert.DeserializeObject<SaveData>(File.ReadAllText("savedata.json"));
            }
            else
            {
                sd.sCoins = 0;
                sd.sNumberGame = 0;
                for (int i = 0; i < 3; i++)
                {
                    sd.sPlayer[i] = 0;
                    sd.sBoss[i] = 0;
                    sd.sBossOpen[i] = 0;
                    sd.sBossMaxOpen[i] = 0;
                }
            }
            coinsText = CoinsText.GetComponent<TextMeshProUGUI>();
            GetCoins();
            MenuSetActive();
            SetControl();
            CheckCost();
            CheckActiveBoss();
            //int n = _Cost.cost [0];

        }

        // Update is called once per frame
        void Update()
        {

        }
        //запись монет
        public void GetCoins()
        {
            //int coins = PlayerPrefs.GetInt("Coins", 0);
            int coins = sd.sCoins;
            coinsText.text = coins.ToString();
        }
        //переключение активности меню
        public void MenuSetActive()
        {
            for (int i = 0; i < 3; i++)
            {
                if (/*PlayerPrefs.GetInt("NumberGame", 0)*/sd.sNumberGame == i)
                {
                    gameMenu[i].SetActive(true);
                }
                else
                {
                    gameMenu[i].SetActive(false);
                }
            }
        }
        //выбор игры	
        public void ButtonChooseGame(int n)
        {
            //PlayerPrefs.SetInt("NumberGame", n);
            sd.sNumberGame = n;
            File.WriteAllText("savedata", JsonConvert.SerializeObject(sd));
            //playerObject = playerObjects [n];
            MenuSetActive();
            CheckCost();
        }
        //меняем местами 0 и 1
        public int SetBool(int i)
        {
            if (i == 0)
                return 1;
            else
                return 0;
        }
        //Устанавливаем  кнопки управления
        public void SetControl()
        {
            Left.interactable = !Convert.ToBoolean(PlayerPrefs.GetInt("SetControl", 0));
            Right.interactable = Convert.ToBoolean(PlayerPrefs.GetInt("SetControl", 0));
        }

        //меняем местами кнопки управления
        public void ChangeControl()
        {

            PlayerPrefs.SetInt("SetControl", SetBool(PlayerPrefs.GetInt("SetControl", 0)));
            SetControl();
        }
        //кнопка прокачки
        public void ButtonUpgrade()
        {
            /*switch (PlayerPrefs.GetInt("NumberGame", 0))
            {
            case 0:
                SetCoins (-cost [PlayerPrefs.GetInt ("PlayerCar", 0)]);
                PlayerPrefs.SetInt ("PlayerCar", PlayerPrefs.GetInt ("PlayerCar", 0) + 1);
                CheckCost ();
                break;
            case 1:
                SetCoins (-cost [PlayerPrefs.GetInt ("PlayerPlane", 0)]);
                PlayerPrefs.SetInt ("PlayerPlane", PlayerPrefs.GetInt ("PlayerPlane", 0) + 1);
                CheckCost ();
                break;
            case 2:
                SetCoins (-cost [PlayerPrefs.GetInt ("PlayerRocket", 0)]);
                PlayerPrefs.SetInt ("PlayerRocket", PlayerPrefs.GetInt ("PlayerRocket", 0) + 1);
                CheckCost ();
                break;
            }*/
            SetCoins(-cost[sd.sPlayer[sd.sNumberGame]]);
            //PlayerPrefs.SetInt(GetPlayer(PlayerPrefs.GetInt("NumberGame", 0)), PlayerPrefs.GetInt(GetPlayer(PlayerPrefs.GetInt("NumberGame", 0)), 0) + 1);
            sd.sPlayer[sd.sNumberGame]++;
            File.WriteAllText("savedata", JsonConvert.SerializeObject(sd));
            CheckCost();

        }
        //проверка стоимости для кнопки прокачки
        public void CheckCost()
        {
            /*switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
            case 0:
                if (PlayerPrefs.GetInt ("PlayerCar", 0) >= 2) {
                    Upgrade.interactable = false;
                    Upgrade.gameObject.GetComponent<Image> ().overrideSprite = imageMax;

                } else if (PlayerPrefs.GetInt ("Coins", 0) < cost [PlayerPrefs.GetInt ("PlayerCar", 0)]) {
                    Upgrade.interactable = false;
                    Upgrade.gameObject.GetComponent<Image> ().overrideSprite = imageUpgradeDisabled;
                } else {
                    Upgrade.interactable = true;
                    Upgrade.gameObject.GetComponent<Image> ().overrideSprite = imageUpgradeActive;
                }
                break;
            case 1:
                if (PlayerPrefs.GetInt ("PlayerPlane", 0) >= 2) {
                    Upgrade.interactable = false;
                    Upgrade.gameObject.GetComponent<Image> ().overrideSprite = imageMax;

                } else if (PlayerPrefs.GetInt ("Coins", 0) < cost [PlayerPrefs.GetInt ("PlayerPlane", 0)]) {
                    Upgrade.interactable = false;
                    Upgrade.gameObject.GetComponent<Image> ().overrideSprite = imageUpgradeDisabled;
                } else {
                    Upgrade.interactable = true;
                    Upgrade.gameObject.GetComponent<Image> ().overrideSprite = imageUpgradeActive;
                }
                break;
            case 2:
                if (PlayerPrefs.GetInt ("PlayerRocket", 0) >= 2) {
                    Upgrade.interactable = false;
                    Upgrade.gameObject.GetComponent<Image> ().overrideSprite = imageMax;

                } else if (PlayerPrefs.GetInt ("Coins", 0) < cost [PlayerPrefs.GetInt ("PlayerRocket", 0)]) {
                    Upgrade.interactable = false;
                    Upgrade.gameObject.GetComponent<Image> ().overrideSprite = imageUpgradeDisabled;
                } else {
                    Upgrade.interactable = true;
                    Upgrade.gameObject.GetComponent<Image> ().overrideSprite = imageUpgradeActive;
                }
                break;
            }*/
            //if (PlayerPrefs.GetInt(GetPlayer(PlayerPrefs.GetInt("NumberGame", 0)), 0) >= 2)
            if (sd.sPlayer[sd.sNumberGame] >= 2)
            {
                Upgrade.interactable = false;
                Upgrade.gameObject.GetComponent<Image>().overrideSprite = imageMax;

            }
            else if (sd.sCoins < cost[sd.sPlayer[sd.sNumberGame]])
            {
                Upgrade.interactable = false;
                Upgrade.gameObject.GetComponent<Image>().overrideSprite = imageUpgradeDisabled;
            }
            else
            {
                Upgrade.interactable = true;
                Upgrade.gameObject.GetComponent<Image>().overrideSprite = imageUpgradeActive;
            }
        }
        //установка активности кнопок
        public void CheckActiveBoss()
        {

            /*switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
            case 0:
                for(int i=0; i<3;i++){
                    Debug.Log (i+" ");
                    if (i > PlayerPrefs.GetInt ("BossMaxOpenCar", 0)) {
                        buttons0[i].interactable = false;
                        buttons0[i].gameObject.GetComponent<Image> ().overrideSprite = imageCloseBoss;
                        Debug.Log ("1");
                    } else if (i > PlayerPrefs.GetInt ("BossOpenCar", 0)) {
                        buttons0[i].interactable = false;
                        buttons0[i].gameObject.GetComponent<Image> ().overrideSprite = imageBoss0[i];
                        buttons0[i].gameObject.GetComponent<Image> ().color =Color.grey;
                        Debug.Log ("2");
                    } else if (i != PlayerPrefs.GetInt ("BossCar", 0)) {
                        buttons0[i].interactable = true;
                        buttons0[i].gameObject.GetComponent<Image> ().overrideSprite = imageBoss0[i];
                        buttons0[i].gameObject.GetComponent<Image> ().color =new Color(255,255,255,255);
                        Debug.Log ("3");
                    } else {
                        buttons0[i].interactable = true;
                        buttons0[i].gameObject.GetComponent<Image> ().overrideSprite = imageBoss0[i];
                        buttons0[i].gameObject.GetComponent<Image> ().overrideSprite = imageBossActive0[i];
                        Debug.Log ("4");
                    }
                }
                break;
            case 1:
                for(int i=0; i<3;i++){
                    if (i > PlayerPrefs.GetInt ("BossMaxOpenPlane", 0)) {
                        buttons1[i].interactable = false;
                        buttons1[i].gameObject.GetComponent<Image> ().overrideSprite = imageCloseBoss;
                    } else if (i > PlayerPrefs.GetInt ("BossOpenPlane", 0)) {
                        buttons1[i].interactable = false;
                        buttons1[i].gameObject.GetComponent<Image> ().overrideSprite = imageBoss1[i];
                        buttons1[i].gameObject.GetComponent<Image> ().color =Color.grey;
                    } else if (i != PlayerPrefs.GetInt ("BossPlane", 0)) {
                        buttons1[i].interactable = true;
                        buttons1[i].gameObject.GetComponent<Image> ().overrideSprite = imageBoss1[i];
                        buttons1[i].gameObject.GetComponent<Image> ().color =new Color(255,255,255,255);
                    } else {
                        buttons1[i].interactable = true;
                        buttons1[i].gameObject.GetComponent<Image> ().overrideSprite = imageBoss1[i];
                        buttons1[i].gameObject.GetComponent<Image> ().overrideSprite = imageBossActive1[i];
                    }
                }
                break;
            case 2:
                for(int i=0; i<3;i++){
                    if (i > PlayerPrefs.GetInt ("BossMaxOpenRocket", 0)) {
                        buttons2[i].interactable = false;
                        buttons2[i].gameObject.GetComponent<Image> ().overrideSprite = imageCloseBoss;
                    } else if (i > PlayerPrefs.GetInt ("BossOpenRocket", 0)) {
                        buttons2[i].interactable = false;
                        buttons2[i].gameObject.GetComponent<Image> ().overrideSprite = imageBoss2[i];
                        buttons2[i].gameObject.GetComponent<Image> ().color =Color.grey;
                    } else if (i != PlayerPrefs.GetInt ("BossRocket", 0)) {
                        buttons2[i].interactable = true;
                        buttons2[i].gameObject.GetComponent<Image> ().overrideSprite = imageBoss2[i];
                        buttons2[i].gameObject.GetComponent<Image> ().color =new Color(255,255,255,255);
                    } else {
                        buttons2[i].interactable = true;
                        buttons2[i].gameObject.GetComponent<Image> ().overrideSprite = imageBoss2[i];
                        buttons2[i].gameObject.GetComponent<Image> ().overrideSprite = imageBossActive2[i];
                    }
                }
                break;
            }*/
            /*for (int i = 0; i < 3; i++)
            {
                if (i > PlayerPrefs.GetInt(GetBossMaxOpen(PlayerPrefs.GetInt("NumberGame", 0)), 0))
                {
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].interactable = false;
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].gameObject.GetComponent<Image>().overrideSprite = imageCloseBoss;
                }
                else if (i > PlayerPrefs.GetInt(GetBossOpen(PlayerPrefs.GetInt("NumberGame", 0)), 0))
                {
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].interactable = false;
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].gameObject.GetComponent<Image>().overrideSprite = imageBoss[PlayerPrefs.GetInt("NumberGame", 0)].image[i];
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].gameObject.GetComponent<Image>().color = Color.grey;
                }
                else if (i != PlayerPrefs.GetInt(GetBoss(PlayerPrefs.GetInt("NumberGame", 0)), 0))
                {
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].interactable = true;
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].gameObject.GetComponent<Image>().overrideSprite = imageBoss[PlayerPrefs.GetInt("NumberGame", 0)].image[i];
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                }
                else
                {
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].interactable = true;
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].gameObject.GetComponent<Image>().overrideSprite = imageBoss[PlayerPrefs.GetInt("NumberGame", 0)].image[i];
                    buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[i].gameObject.GetComponent<Image>().overrideSprite = imageBossActive[PlayerPrefs.GetInt("NumberGame", 0)].image[i];
                }
            }*/
            for (int i = 0; i < 3; i++)
            {
                if (i > sd.sBossMaxOpen[sd.sNumberGame])
                {
                    buttons[sd.sNumberGame].button[i].interactable = false;
                    buttons[sd.sNumberGame].button[i].gameObject.GetComponent<Image>().overrideSprite = imageCloseBoss;
                }
                else if (i > sd.sBossOpen[sd.sNumberGame])
                {
                    buttons[sd.sNumberGame].button[i].interactable = false;
                    buttons[sd.sNumberGame].button[i].gameObject.GetComponent<Image>().overrideSprite = imageBoss[sd.sNumberGame].image[i];
                    buttons[sd.sNumberGame].button[i].gameObject.GetComponent<Image>().color = Color.grey;
                }
                else if (i != sd.sBoss[sd.sNumberGame])
                {
                    buttons[sd.sNumberGame].button[i].interactable = true;
                    buttons[sd.sNumberGame].button[i].gameObject.GetComponent<Image>().overrideSprite = imageBoss[sd.sNumberGame].image[i];
                    buttons[sd.sNumberGame].button[i].gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                }
                else
                {
                    buttons[sd.sNumberGame].button[i].interactable = true;
                    buttons[sd.sNumberGame].button[i].gameObject.GetComponent<Image>().overrideSprite = imageBoss[sd.sNumberGame].image[i];
                    buttons[sd.sNumberGame].button[i].gameObject.GetComponent<Image>().overrideSprite = imageBossActive[sd.sNumberGame].image[i];
                }
            }
        }
        //изменение выбраного босса для машин
        public void ButtonChangeBoss(int n)
        {
            //buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[PlayerPrefs.GetInt(GetBoss(PlayerPrefs.GetInt("NumberGame", 0)), 0)].gameObject.GetComponent<Image>().overrideSprite = imageBoss0[PlayerPrefs.GetInt(GetBoss(PlayerPrefs.GetInt("NumberGame", 0)), 0)];
            buttons[sd.sNumberGame].button[sd.sBoss[sd.sNumberGame]].gameObject.GetComponent<Image>().overrideSprite = imageBoss[sd.sNumberGame].image[sd.sBoss[sd.sNumberGame]];
            //PlayerPrefs.SetInt(GetBoss(PlayerPrefs.GetInt("NumberGame", 0)), n);
            sd.sBoss[sd.sNumberGame] = n;
            File.WriteAllText("savedata", JsonConvert.SerializeObject(sd));
            //buttons[PlayerPrefs.GetInt("NumberGame", 0)].button[PlayerPrefs.GetInt(GetBoss(PlayerPrefs.GetInt("NumberGame", 0)), 0)].gameObject.GetComponent<Image>().overrideSprite = imageBossActive0[PlayerPrefs.GetInt(GetBoss(PlayerPrefs.GetInt("NumberGame", 0)), 0)];
            buttons[sd.sNumberGame].button[sd.sBoss[sd.sNumberGame]].gameObject.GetComponent<Image>().overrideSprite = imageBossActive[sd.sNumberGame].image[sd.sBoss[sd.sNumberGame]];

        }
        /*
        //изменение выбраного босса для самолетов
        public void ButtonChangeBossPlane(int n){
            buttons1[PlayerPrefs.GetInt ("BossPlane", 0)].gameObject.GetComponent<Image> ().overrideSprite = imageBoss1[PlayerPrefs.GetInt ("BossPlane", 0)];
            PlayerPrefs.SetInt ("BossPlane", n);
            buttons1[PlayerPrefs.GetInt ("BossPlane", 0)].gameObject.GetComponent<Image> ().overrideSprite = imageBossActive0[PlayerPrefs.GetInt ("BossPlane", 0)];

        }
        //изменение выбраного босса для ракет
        public void ButtonChangeBossRocket(int n){
            buttons2[PlayerPrefs.GetInt ("BossRocket", 0)].gameObject.GetComponent<Image> ().overrideSprite = imageBoss2[PlayerPrefs.GetInt ("BossRocket", 0)];
            PlayerPrefs.SetInt ("BossRocket", n);
            buttons2[PlayerPrefs.GetInt ("BossRocket", 0)].gameObject.GetComponent<Image> ().overrideSprite = imageBossActive0[PlayerPrefs.GetInt ("BossRocket", 0)];

        }*/
        public void SetBanknotes()
        {
            /*switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
            case 0:
                imageBanknote = imageBanknotes [PlayerPrefs.GetInt ("BossCar", 0)];
                break;
            case 1:
                imageBanknote = imageBanknotes [PlayerPrefs.GetInt ("BossPlane", 0)];
                break;
            case 2:
                imageBanknote = imageBanknotes [PlayerPrefs.GetInt ("BossRocket", 0)];
                break;
            }*/
            imageBanknote = imageBanknotes[sd.sBoss[sd.sNumberGame]];
        }
        public void ButtonPlay()
        {
            SceneManager.sceneLoaded += OnSceneFinishedLoading;
        }
        void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
        {
            /*if (scene.name == "Racing")
                Spawn();
            if (scene.name == "Planes")
                Spawn();
            if (scene.name == "Rockets")
                Spawn();*/
            Spawn();
        }
        void Spawn()
        {
            selectedEnvironment = Instantiate(environment[sd.sNumberGame].gameObjects[sd.sBoss[sd.sNumberGame]], Vector3.zero, Quaternion.identity);
            //создаем машину в выбранном окружении
            playerObject = Instantiate(playerObjects[sd.sNumberGame].gameObjects[sd.sPlayer[sd.sNumberGame]], Vector3.zero, Quaternion.identity) as GameObject;

        }
        /*void SpawnCar()
        {
            selectedEnvironment = Instantiate(environmentCar[PlayerPrefs.GetInt("BossCar", 0)], Vector3.zero, Quaternion.identity);
            //создаем машину в выбранном окружении
            playerObject = Instantiate(playerObjectsCar[PlayerPrefs.GetInt("PlayerCar", 0)], Vector3.zero, Quaternion.identity) as GameObject;

        }
        void SpawnPlane()
        {
            selectedEnvironment = Instantiate(environmentPlane[PlayerPrefs.GetInt("BossPlane", 0)], Vector3.zero, Quaternion.identity);
            //создаем машину в выбранном окружении
            playerObject = Instantiate(playerObjectsPlane[PlayerPrefs.GetInt("PlayerPlane", 0)], Vector3.zero, Quaternion.identity) as GameObject;

        }
        void SpawnRocket()
        {
            selectedEnvironment = Instantiate(environmentRocket[PlayerPrefs.GetInt("BossRocket", 0)], Vector3.zero, Quaternion.identity);
            //создаем машину в выбранном окружении
            playerObject = Instantiate(playerObjectsRocket[PlayerPrefs.GetInt("PlayerRocket", 0)], Vector3.zero, Quaternion.identity) as GameObject;

        }*/
        public void SetCoins(int n)
        {
            sd.sCoins = n;
            File.WriteAllText("savedata", JsonConvert.SerializeObject(sd));
            //PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + n);
            GetCoins();
        }
        //Методы которые уже не актуальны!!!
        //PlayerPrefs to String
        //это методы для проверок работы кнопок(можно удалить потом)

        public void SetCoins()
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + 100);
            GetCoins();
        }

        public void SetCoins0()
        {
            PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) - 100);
            GetCoins();
        }
        public void SetMBoss()
        {
            switch (PlayerPrefs.GetInt("NumberGame", 0))
            {
                case 0:
                    PlayerPrefs.SetInt("BossMaxOpenCar", PlayerPrefs.GetInt("BossMaxOpenCar", 0) + 1);
                    break;
                case 1:
                    PlayerPrefs.SetInt("BossMaxOpenPlane", PlayerPrefs.GetInt("BossMaxOpenPlane", 0) + 1);
                    break;
                case 2:
                    PlayerPrefs.SetInt("BossMaxOpenRocket", PlayerPrefs.GetInt("BossMaxOpenRocket", 0) + 1);
                    break;

            }
            CheckActiveBoss();
        }
        public void SetMBoss0()
        {
            switch (PlayerPrefs.GetInt("NumberGame", 0))
            {
                case 0:
                    PlayerPrefs.SetInt("BossMaxOpenCar", 0);
                    PlayerPrefs.SetInt("BossOpenCar", 0);
                    PlayerPrefs.SetInt("BossCar", 0);
                    break;
                case 1:
                    PlayerPrefs.SetInt("BossMaxOpenPlane", 0);
                    PlayerPrefs.SetInt("BossOpenPlane", 0);
                    PlayerPrefs.SetInt("BossPlane", 0);
                    break;
                case 2:
                    PlayerPrefs.SetInt("BossMaxOpenRocket", 0);
                    PlayerPrefs.SetInt("BossOpenRocket", 0);
                    PlayerPrefs.SetInt("BossRocket", 0);
                    break;

            }
            CheckActiveBoss();
        }

        public void SetBoss()
        {
            switch (PlayerPrefs.GetInt("NumberGame", 0))
            {
                case 0:
                    PlayerPrefs.SetInt("BossOpenCar", PlayerPrefs.GetInt("BossOpenCar", 0) + 1);
                    break;
                case 1:
                    PlayerPrefs.SetInt("BossOpenPlane", PlayerPrefs.GetInt("BossOpenPlane", 0) + 1);
                    break;
                case 2:
                    PlayerPrefs.SetInt("BossOpenRocket", PlayerPrefs.GetInt("BossOpenRocket", 0) + 1);
                    break;

            }
            CheckActiveBoss();
        }
        public void SetBoss0()
        {
            switch (PlayerPrefs.GetInt("NumberGame", 0))
            {
                case 0:
                    PlayerPrefs.SetInt("BossOpenCar", 0);
                    break;
                case 1:
                    PlayerPrefs.SetInt("BossOpenPlane", 0);
                    break;
                case 2:
                    PlayerPrefs.SetInt("BossOpenRocket", 0);
                    break;

            }
            CheckActiveBoss();

        }
        public void SetLvl()
        {
            PlayerPrefs.SetInt("PlayerCar", PlayerPrefs.GetInt("PlayerCar", 0) + 1);

        }
        public void SetLvl0()
        {
            PlayerPrefs.SetInt("PlayerCar", PlayerPrefs.GetInt("PlayerCar", 0) * 0);
            Upgrade.gameObject.GetComponent<Image>().color = Color.blue;
        }
        public void SetColor()
        {
            Upgrade.gameObject.GetComponent<Image>().color = Color.blue;
        }
    }
    [Serializable]
    class ImageBoss
    {
        [SerializeField] public Sprite[] image;
    }
    [Serializable]
    class ImageBossActive
    {
        [SerializeField] public Sprite[] image;
    }

    class PlayerObjects
    {
        [SerializeField] public GameObject[] gameObjects;
    }
    class Player
    {
        [SerializeField] public GameObject[] gameObjects;
    }
    class Environment
    {
        [SerializeField] public GameObject[] gameObjects;
    }
    class Buttons
    {
        [SerializeField] public Button[] button;
    }
    public class SaveData
    {
        public int sCoins;
        public int sNumberGame;
        public int[] sPlayer;
        public int[] sBoss;
        public int[] sBossOpen;
        public int[] sBossMaxOpen;

    }

}

