using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuManager : MonoBehaviour {

	public GameObject playerObject;
	public GameObject[] playerObjectsCar;
	public GameObject[] playerObjectsPlane;
	public GameObject[] playerObjectsRocket;
	public GameObject[] Car;  
	public GameObject[] Plane;  
	public GameObject[] Rocket;  
	public GameObject[] gameMenu;
	public GameObject[] environmentCar;                        //массив targetImages, который будет заменяться на массив окружения выбранной игры 
	public GameObject[] environmentPlane;  
	public GameObject[] environmentRocket;  
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
	public Sprite[] imageBoss0;
	public Sprite[] imageBoss1;
	public Sprite[] imageBoss2;
	// картинки активных боссов
	public Sprite[] imageBossActive0;
	public Sprite[] imageBossActive1;
	public Sprite[] imageBossActive2;
	//
	public Sprite[] imageBanknotes;
	public  static Sprite imageBanknote;
	// кнопки боссов
	public Button[] buttons0;
	public Button[] buttons1;
	public Button[] buttons2;

	//монеты
	public TextMeshProUGUI CoinsText;
	TextMeshProUGUI coinsText;
	//массив стоимости прокачки
	public int[] cost;

	void Start () {
		coinsText = CoinsText.GetComponent<TextMeshProUGUI> ();
		GetCoins ();
		MenuSetActive ();
		SetControl ();
		CheckCost ();
		CheckActiveBoss ();
		//int n = _Cost.cost [0];
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	//запись монет
	public void GetCoins (){
		int coins=PlayerPrefs.GetInt ("Coins", 0);
		coinsText.text = coins.ToString ();
	}
	//переключение активности меню
	public void MenuSetActive(){
		for (int i = 0; i < 3; i++) {
			if (PlayerPrefs.GetInt ("NumberGame", 0) == i) {
				gameMenu [i].SetActive(true);
			} else {
				gameMenu [i].SetActive(false);
			}
		}
	}
	//выбор игры	
	public void ButtonChooseGame(int n){
		PlayerPrefs.SetInt ("NumberGame", n);
		//playerObject = playerObjects [n];
		MenuSetActive ();
		CheckCost ();
	}
	//меняем местами 0 и 1
	public int SetBool(int i){
		if (i == 0)
			return 1;
		else
			return 0;
	}
	//Устанавливаем  кнопки управления
	public void SetControl(){
		Left.interactable=!Convert.ToBoolean(PlayerPrefs.GetInt("SetControl",0));
		Right.interactable=Convert.ToBoolean(PlayerPrefs.GetInt("SetControl",0));
	}

	//меняем местами кнопки управления
	public void ChangeControl(){
		
		PlayerPrefs.SetInt ("SetControl", SetBool(PlayerPrefs.GetInt("SetControl",0)));
		SetControl ();
	}
	//кнопка прокачки
	public void ButtonUpgrade(){
		switch (PlayerPrefs.GetInt("NumberGame", 0))
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
		}


	}
	//проверка стоимости для кнопки прокачки
	public void CheckCost(){
		switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
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
		}
	}
	//установка активности кнопок
	public void CheckActiveBoss(){
		
		switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
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
		}
	}
	//изменение выбраного босса для машин
	public void ButtonChangeBossCar(int n){
		buttons0[PlayerPrefs.GetInt ("BossCar", 0)].gameObject.GetComponent<Image> ().overrideSprite = imageBoss0[PlayerPrefs.GetInt ("BossCar", 0)];
		PlayerPrefs.SetInt ("BossCar", n);
		buttons0 [PlayerPrefs.GetInt ("BossCar", 0)].gameObject.GetComponent<Image> ().overrideSprite = imageBossActive0[PlayerPrefs.GetInt ("BossCar", 0)];
	}
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

	}
	public void SetBanknotes(){
		switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
		case 0:
			imageBanknote = imageBanknotes [PlayerPrefs.GetInt ("BossCar", 0)];
			break;
		case 1:
			imageBanknote = imageBanknotes [PlayerPrefs.GetInt ("BossPlane", 0)];
			break;
		case 2:
			imageBanknote = imageBanknotes [PlayerPrefs.GetInt ("BossRocket", 0)];
			break;
		}
	}
	public void ButtonPlay(){
		//SceneManager.sceneLoaded += OnSceneFinishedLoading;
		SceneManager.LoadScene(PlayerPrefs.GetInt("NumberGame", 0));
	}
	void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Racing")
			SpawnCar();
		if (scene.name == "Planes")
			SpawnPlane();
		if (scene.name == "Rockets")
			SpawnRocket();
	}
	void SpawnCar()
	{
		selectedEnvironment = Instantiate(environmentCar[PlayerPrefs.GetInt ("BossCar", 0)], Vector3.zero, Quaternion.identity);
		//создаем машину в выбранном окружении
		playerObject= Instantiate(playerObjectsCar[PlayerPrefs.GetInt ("PlayerCar", 0)], Vector3.zero, Quaternion.identity) as GameObject;

	}
	void SpawnPlane()
	{
		selectedEnvironment = Instantiate(environmentPlane[PlayerPrefs.GetInt ("BossPlane", 0)], Vector3.zero, Quaternion.identity);
		//создаем машину в выбранном окружении
		playerObject= Instantiate(playerObjectsPlane[PlayerPrefs.GetInt ("PlayerPlane", 0)], Vector3.zero, Quaternion.identity) as GameObject;

	}
	void SpawnRocket()
	{
		selectedEnvironment = Instantiate(environmentRocket[PlayerPrefs.GetInt ("BossRocket", 0)], Vector3.zero, Quaternion.identity);
		//создаем машину в выбранном окружении
		playerObject= Instantiate(playerObjectsRocket[PlayerPrefs.GetInt ("PlayerRocket", 0)], Vector3.zero, Quaternion.identity) as GameObject;

	}
	public void SetCoins(int n){
		PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins", 0) + n);
		GetCoins ();
	}
	//это методы для проверок работы кнопок(можно удалить потом)

	public void SetCoins(){
		PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins", 0) + 100);
		GetCoins ();
	}

	public void SetCoins0(){
		PlayerPrefs.SetInt ("Coins", PlayerPrefs.GetInt ("Coins", 0) - 100);
		GetCoins ();
	}
	public void SetMBoss(){
		switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
		case 0:
			PlayerPrefs.SetInt ("BossMaxOpenCar", PlayerPrefs.GetInt ("BossMaxOpenCar", 0) + 1);
			break;
		case 1:
			PlayerPrefs.SetInt ("BossMaxOpenPlane", PlayerPrefs.GetInt ("BossMaxOpenPlane", 0) + 1);
			break;
		case 2:
			PlayerPrefs.SetInt ("BossMaxOpenRocket", PlayerPrefs.GetInt ("BossMaxOpenRocket", 0) + 1);
			break;

		}
		CheckActiveBoss ();
	}
	public void SetMBoss0(){
		switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
		case 0:
			PlayerPrefs.SetInt ("BossMaxOpenCar",0);
			PlayerPrefs.SetInt ("BossOpenCar",0);
			PlayerPrefs.SetInt ("BossCar", 0);
			break;
		case 1:
			PlayerPrefs.SetInt ("BossMaxOpenPlane",0);
			PlayerPrefs.SetInt ("BossOpenPlane",0);
			PlayerPrefs.SetInt ("BossPlane", 0);
			break;
		case 2:
			PlayerPrefs.SetInt ("BossMaxOpenRocket",0);
			PlayerPrefs.SetInt ("BossOpenRocket",0);
			PlayerPrefs.SetInt ("BossRocket", 0);
			break;

		}
		CheckActiveBoss ();
	}

	public void SetBoss(){
		switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
		case 0:
			PlayerPrefs.SetInt ("BossOpenCar", PlayerPrefs.GetInt ("BossOpenCar", 0) + 1);
			break;
		case 1:
			PlayerPrefs.SetInt ("BossOpenPlane", PlayerPrefs.GetInt ("BossOpenPlane", 0) + 1);
			break;
		case 2:
			PlayerPrefs.SetInt ("BossOpenRocket", PlayerPrefs.GetInt ("BossOpenRocket", 0) + 1);
			break;

		}
		CheckActiveBoss ();
	}
	public void SetBoss0(){
		switch (PlayerPrefs.GetInt ("NumberGame", 0)) {
		case 0:
			PlayerPrefs.SetInt ("BossOpenCar", 0);
			break;
		case 1:
			PlayerPrefs.SetInt ("BossOpenPlane",0);
			break;
		case 2:
			PlayerPrefs.SetInt ("BossOpenRocket",0);
			break;

		}
		CheckActiveBoss ();

	}
	public void SetLvl(){
		PlayerPrefs.SetInt ("PlayerCar", PlayerPrefs.GetInt ("PlayerCar", 0) + 1);

	}
	public void SetLvl0(){
		PlayerPrefs.SetInt ("PlayerCar", PlayerPrefs.GetInt ("PlayerCar", 0) *0);
		Upgrade.gameObject.GetComponent<Image> ().color = Color.blue;
	}
	public void SetColor(){
		Upgrade.gameObject.GetComponent<Image> ().color = Color.blue;
	}
}
