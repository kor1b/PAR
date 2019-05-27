using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	GameOverManager gameOverManager;

	private void Awake()
	{
		gameOverManager = GameOverManager.Instance;
	}

	public void LoadMenu()
	{
		if (SceneManager.GetActiveScene().name == "Racing")
			gameOverManager.SetCarPrefs(Racing.GameManager.Instance.win);

		else if (SceneManager.GetActiveScene().name == "Planes")
			gameOverManager.SetCarPrefs(Racing.GameManager.Instance.win);

		else if (SceneManager.GetActiveScene().name == "Rockets")
			gameOverManager.SetCarPrefs(Racing.GameManager.Instance.win);

		SceneManager.LoadScene("Menu");
	}
}
