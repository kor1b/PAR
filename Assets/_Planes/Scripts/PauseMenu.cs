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
        Racing.GameManager.Instance?.StopGame();
        Planes.GameManager.Instance?.StopGame();
        Rockets.GameManager.Instance?.StopGame();
    }

    public void PauseOff()
    {
        Time.timeScale = 1;

        if (Racing.GameManager.Instance != null)
            StartCoroutine(Racing.GameManager.Instance.StartCountdown());
        else if (Planes.GameManager.Instance != null)
            StartCoroutine(Planes.GameManager.Instance.StartGame());
        else if (Rockets.GameManager.Instance != null)
            StartCoroutine(Rockets.GameManager.Instance.StartGame());
    }
}
