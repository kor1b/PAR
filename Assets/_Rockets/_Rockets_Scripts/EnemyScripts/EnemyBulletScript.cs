using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class EnemyBulletScript : BulletFactory
    {

        public PlayerRotationScript player;

        void Awake()
        {
            gameManager = GameManager.Instance;
            if (gameManager.Win != 2)
            {
                    player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRotationScript>();
            }
        }
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            tr = GetComponent<Transform>();
        }

        private void OnEnable()
        {
            lifeTimer = lifeTime;
        }
        void Update()
        {
            MoveAndLive();
        }

        private void FixedUpdate()
        {
            PhysicalMovement();
        }

        private void OnTriggerEnter(Collider other)
        {
            if ((other.CompareTag("Player")) && (other.gameObject.activeInHierarchy))
            {
                //Debug.Log("Got it!");
                player.BulletDamage(damage, shieldDamageBoost);
                gameObject.SetActive(false);
            }
        }
    }
}
