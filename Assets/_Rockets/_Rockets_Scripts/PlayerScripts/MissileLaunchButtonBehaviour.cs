using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rockets
{
    public class MissileLaunchButtonBehaviour : MonoBehaviour
    {
        public PlayerRotationScript Player;
        public Button missileLaunchButton;
        void Start()
        {
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRotationScript>();
            if (Player != null)
                missileLaunchButton.onClick.AddListener(Player.LaunchMissile);
        }
    }
}