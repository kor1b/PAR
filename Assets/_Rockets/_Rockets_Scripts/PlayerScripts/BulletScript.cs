using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

namespace Rockets
{
    public class BulletScript : BulletFactory
    { 
        [HideInInspector]
        public EnemyRotationScript enemy;
        void Awake()
        {
            gameManager = GameManager.Instance;

            if (gameManager.Win != 1 && gameManager.LooseTime > 0)
            {
                enemy = GameObject.FindGameObjectWithTag("Enemy").GetComponent<EnemyRotationScript>();
            }
        }
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            tr = GetComponent<Transform>();
        }
        void OnEnable()
        {
            lifeTimer = lifeTime;
        }


        void Update()
        {
            MoveAndLive();
        }

        void FixedUpdate()
        {
            PhysicalMovement();
        }

        void OnTriggerEnter(Collider other)
        {
            if ((other.CompareTag("Enemy")) && (other.gameObject.activeInHierarchy))
            {
                enemy.BulletDamage(damage, shieldDamageBoost);
                gameObject.SetActive(false);
            }
        }

    }
}