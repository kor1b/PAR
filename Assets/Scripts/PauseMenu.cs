using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
	public static PauseMenu Instance;

	#region Singleton
	private void Awake()
	{
		if (Instance != null)
			return;
		Instance = this;
	}
	#endregion

	public void PauseOn()
	{
		Time.timeScale = 0;

		Racing.GameManager.Instance.StopGame();
	}

	public void PauseOff()
	{
		Time.timeScale = 1;

		StartCoroutine(Racing.GameManager.Instance.StartCountdown());
	}
}
