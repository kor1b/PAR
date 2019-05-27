using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planes
{
    public class Takeoff : MonoBehaviour
    {
        //изначальные координаты игрока
        public Vector3 PlayerFirstCoordinates;
        public Vector3 PlayerFirstRotation;

        //изначальные координаты врага
        public Vector3 EnemyFirstCoordinates;
        public Vector3 EnemyFirstRotation;

        //изначальные координаты острова
        public Vector3 IslandFirstCoordinates;
        public Vector3 IslandFirstRotation;

        private Animator animator;

        private void Awake()
        {
            animator = GetComponentInParent<Animator>();
        }

        private void Update()
        {
            if (!GameManager.gameIsGoing)
            {
                animator.enabled = false;
            }
            else
            {
                animator.enabled = true;
            }
        }

        public void TakeoffEnd()
        {
            SceneDifficultyManager.instance.TakeoffEnd(GetComponentInChildren<CharacterPrint>().isPlayer);
        }
    }
}