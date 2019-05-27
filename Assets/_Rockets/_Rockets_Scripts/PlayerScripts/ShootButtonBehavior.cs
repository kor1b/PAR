using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Rockets {
    public class ShootButtonBehavior : MonoBehaviour
    {
        public PlayerRotationScript Player;
        public Button shootButton;
        void Start()
        {  
            Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRotationScript>();
            if (Player != null)
            shootButton.onClick.AddListener(Player.Shoot);
        }

    }
}