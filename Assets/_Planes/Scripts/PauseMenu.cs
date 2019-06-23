using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace Planes
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance;
        public GameObject targetLostScreen;
        public Image banknote;

        private void Awake()
        {
            #region Singleton
            if (Instance != null)
                return;
            Instance = this;
            #endregion
        }

        public void PauseOn()
        {
            Time.timeScale = 0;
            GameManager.instance.StopGame();
        }

        public void PauseOff()
        {
            Time.timeScale = 1;
            StartCoroutine(GameManager.instance.StartGame());
        }
    }
}