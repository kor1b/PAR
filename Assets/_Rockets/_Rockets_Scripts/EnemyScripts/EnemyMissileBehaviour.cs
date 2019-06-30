using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class EnemyMissileBehaviour : MissileFactory
    {
        public PlayerRotationScript PlayerTarget;

        void Start()
        {
            target = GameObject.FindWithTag("Player");

            if (target != null && target.activeInHierarchy)
                PlayerTarget = target.GetComponent<PlayerRotationScript>();
            else Debug.Log("Interesting FuckUp");
        }

        public void OnEnable()
        {
            lifeTimer = lifeTime;
            searchTimer = searchTimeDelay;
            searchingTimer = searchingTime;
            rb = GetComponent<Rigidbody>();
            tr = GetComponent<Transform>();

            startMoveDirection = tr.forward;
            searchFlag = false;
        }

        // Update is called once per frame
        public void Update()
        {
            FindDirection();
        }

        public void FixedUpdate()
        {
            Pursue();
        }

        public void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && other.gameObject.activeInHierarchy)
            {
                PlayerTarget.MissileDamage(damage, shieldDamageBoost);
                gameObject.SetActive(false);
            }
        }
    }
}