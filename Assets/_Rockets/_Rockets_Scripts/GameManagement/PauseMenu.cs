using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class PauseMenu : MonoBehaviour
    {
        public static PauseMenu Instance;

        public GameObject SettingMenu;

        #region Singleton
        private void Awake()
        {
            SettingMenu.SetActive(false);

            if (Instance != null)
                return;
            Instance = this;
        }
        #endregion

        public void PauseOn()
        {
            Time.timeScale = 0;
            if (SettingMenu != null)
            SettingMenu.SetActive(true);
            GameManager.Instance.StopGame();
            
        }

        public void PauseOff()
        {
            Time.timeScale = 1;
            SettingMenu.SetActive(false);
            StartCoroutine(GameManager.Instance.StartGame());
        }
    }
}