using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{

	public static GameOverManager Instance;
	public GameObject gameManagerObject;

	public Sprite victoryBoardSprite;
	public Sprite defeatBoardSprite;

	public Image backgroundImage;                   //depends on victory or defeat;
	public TextMeshProUGUI gameOverText;			//victory or defeat text
	public TextMeshProUGUI gameTimeText;
	public TextMeshProUGUI highScoreText;
	public TextMeshProUGUI coinsText;

	private void Awake()
	{
		if (Instance != null)
			return;
		Instance = this;
	}

	public void SetValues(int win, float gameTime, int coins)
	{
		//if (win == 0)
		//	LoadMenu();

		//else if (win == -1)
		//{
		//	LoadMenu();
		//}

		//win
		if (win == 1)
		{
			backgroundImage.sprite = victoryBoardSprite;
			gameOverText.text = "VICTORY";
		}

		else if (win == 2)
		{
			backgroundImage.sprite = defeatBoardSprite;
			gameOverText.text = "DEFEAT";
		}

		gameTimeText.text = string.Format("{0:00.00}", gameTime);

		//PlayerPrefs.SetFloat("HighScore", Mathf.Infinity);

		if (win == 1 && gameTime < PlayerPrefs.GetFloat("HighScore", Mathf.Infinity))
			PlayerPrefs.SetFloat("HighScore", gameTime);

		highScoreText.text = "HIGH SCORE: " + string.Format("{0:00.00}", PlayerPrefs.GetFloat("HighScore"));

		coinsText.text = "+" + coins.ToString();
	}

	public void SetCarPrefs(int win)
	{
		//exit before timer
		if (win == 0)
			return;

		//win
		if (win == 1)
		{
			if (PlayerPrefs.GetInt("BossCar") < 2 && PlayerPrefs.GetInt("BossCar") == PlayerPrefs.GetInt("BossOpenCar"))
			{
				PlayerPrefs.SetInt("BossOpenCar", PlayerPrefs.GetInt("BossOpenCar") + 1);
			}

			if (PlayerPrefs.GetInt("BossOpenCar") > PlayerPrefs.GetInt("BossMaxOpenCar"))
			{
				PlayerPrefs.SetInt("BossMaxOpenCar", PlayerPrefs.GetInt("BossOpenCar"));
			}
		}

		//defeat or exit after timer
		if (win == -1 || win == 2)
		{
			PlayerPrefs.SetInt("BossMaxOpenCar", 0);
			PlayerPrefs.SetInt("BossOpenCar", 0);
		}
	}

	void SetPlanePrefs()
	{

	}

	void SetRocketPrefs()
	{

	}


}
