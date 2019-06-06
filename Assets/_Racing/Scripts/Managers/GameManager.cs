using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Racing
{
	public class GameManager : MonoBehaviour
	{
		public static GameManager Instance;

		public GameObject pauseMenu;

		CarBlueprint player;
		CarBlueprint enemy;
		[HideInInspector]
		public CarBlueprint[] cars;                                     //link to enemy and player
		CarSystem[] carsCarSystem;                                      //getcomponent of cars[]	
		bool foundAllCars;                                              //check did we find player and enemy

		CarBlueprint gameWinner;
		[Tooltip("Необходимое количество кругов для победы")]
		public int winScore = 3;
		[HideInInspector]
		public int win = 0;                                             //0 - exit, 1 - victory, 2 - defeat, -1 - player exit before start game

		public static bool gameIsGoing = false;                         //game started?
		public static bool countdownGameStarted = false;                //начался ли обратный отсчет

		public float gameTime;                                          //time from the start of game

		public int coins;

		public GameObject gameOverMenu;

		#region Singleton
		private void Awake()
		{
			if (Instance != null)
				return;
			Instance = this;
		}
		#endregion

		private void Update()
		{
			if (gameIsGoing)
			{
				if (win == 0)
					win = -1;

				gameTime += Time.deltaTime;
			}
		}

		public void Start()
		{
			//find the player and the enemy by tags
			try
			{
				player = GameObject.FindWithTag("Player").GetComponent<CarBlueprint>();
				enemy = GameObject.FindWithTag("Enemy").GetComponent<CarBlueprint>();

				//set ui score
				player.scoreText = GameObject.Find("UICanvas/Score/PlayerScore").GetComponent<TextMeshProUGUI>();
				enemy.scoreText = GameObject.Find("UICanvas/Score/EnemyScore").GetComponent<TextMeshProUGUI>();

				foundAllCars = true;
			}
			catch (Exception e)
			{
				Debug.Log($"<color=red>{e.ToString()}</color>");
			}
			//set an array of cars
			cars[0] = player;
			cars[1] = enemy;

			carsCarSystem = new CarSystem[cars.Length];

			for (int i = 0; i < cars.Length; i++)
			{
				carsCarSystem[i] = cars[i].GetComponent<CarSystem>();
			}
		}

		public void CallReload(GameObject reloadGO)
		{
			StartCoroutine(ReloadPlayer(reloadGO));
		}

		public IEnumerator ReloadPlayer(GameObject reloadGO)
		{

			yield return new WaitForSeconds(1);

			reloadGO.SetActive(true);

			reloadGO.GetComponent<CarSystem>().BackToTrack();
		}

		public IEnumerator StartCountdown()
		{
			if (foundAllCars)
			{
				//start countdown check
				countdownGameStarted = true;

				pauseMenu.SetActive(false);

				yield return StartCoroutine(UIManager.Instance.StartCountdown());

				//flag that game is started
				gameIsGoing = true;

				//start moving of cars
				for (int i = 0; i < cars.Length; i++)
				{
					carsCarSystem[i].enabled = true;
				}
			}
		}

		//use if game is paused or stopped
		public void StopGame()
		{
			if (foundAllCars)
			{
				//turn off the game
				gameIsGoing = false;//

				//stop moving of cars
				for (int i = 0; i < cars.Length; i++)
				{
					carsCarSystem[i].enabled = false;
				}
			}
		}

		//check for game winner
		public void CheckGameWinner()
		{
			for (int i = 0; i < cars.Length; i++)
			{
				//if car win the racing
				if (cars[i].score == winScore)
				{
					//set game winner
					if (gameWinner == null)
						gameWinner = cars[i];

					//set laps time to check time difference
					cars[i].lapsTime = carsCarSystem[i].lapsTime;

					//stop moving
					carsCarSystem[i].enabled = false;

					//set coins to player
					if (cars[i] == player)
					{
						//SetCoins();
						//player win
						if (win == 0 || win == -1)
							win = 1;

						//enable gameover menu
						gameOverMenu.SetActive(true);
						//set gameover board values
						GameOverManager.Instance.SetValues(win, gameTime, CountCoins());
						GameOverManager.Instance.SetCarPrefs(win);
					}

					else
					{
						//enemy win
						if (win == 0 || win == -1)
							win = 2;
					}

				}
			}
		}

		//return a car that completed more laps now	
		public CarBlueprint GetGameLeader()
		{
			if (cars[0].score > cars[1].score)
				return cars[0];

			else if (cars[1].score > cars[0].score)
				return cars[1];

			return null;
		}

		public int CountCoins()
		{
			//if player wins the game
			if (gameWinner == player)
			{
				//set coins by difference of laps time
				coins = (int)(100 + (CarSystem.normalLapsTime * winScore - player.lapsTime) * enemy.enemyCoinsCoef);
			}

			//if enemy wins the game
			else if (gameWinner == enemy)
			{
				//if time between laps < 10 seconds
				if (player.lapsTime - enemy.lapsTime <= 10)
					//set coins by difference of laps time
					coins = (int)(10 + (10 - (player.lapsTime - enemy.lapsTime)));
				else
					//set 10 coins by default
					coins = 10;
				//increase coins
				coins += (int)enemy.enemyCoinsCoef;
			}

			Debug.Log(player.lapsTime);
			Debug.Log(enemy.lapsTime);

			Debug.Log("Coins+ " + coins);

			return coins;

			//PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + coins);

		}
	}

}