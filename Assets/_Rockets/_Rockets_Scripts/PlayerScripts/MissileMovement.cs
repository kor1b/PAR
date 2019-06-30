using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class MissileMovement : MissileFactory
    {
        private EnemyRotationScript BossTarget;

        void Start()
        {
            target = GameObject.FindGameObjectWithTag("Enemy");
            if (target != null && target.activeInHierarchy)
                BossTarget = target.GetComponent<EnemyRotationScript>();
            else Debug.Log("Interesting FuckUp");
        }

       
        void OnEnable()
        {
            lifeTimer = lifeTime;
            searchTimer = searchTimeDelay;
            searchingTimer = searchingTime;

            rb = GetComponent<Rigidbody>();
            tr = GetComponent<Transform>();

            startMoveDirection = tr.forward;
            searchFlag = false;
        }


        void Update()
        {
            FindDirection();
        }

        void FixedUpdate()
        {
            Pursue();
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy") && other.gameObject.activeInHierarchy)
            {
                BossTarget.MissileDamage(damage, shieldDamageBoost);
                gameObject.SetActive(false);

            }

        }

    }
}
