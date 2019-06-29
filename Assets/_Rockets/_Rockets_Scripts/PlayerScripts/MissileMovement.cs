using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class MissileMovement : MissileFactory
    {
        private EnemyRotationScript BossTarget;

        public string BossTag;

        private int NumberOfLevel;
        void Start()
        {
            BossTagging();
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
            if (gameObject.CompareTag("PlayerMissile") && other.gameObject != null && other.gameObject.activeInHierarchy && other.CompareTag(BossTag))
            {
                BossTarget.MissileDamage(damage, shieldDamageBoost);
                gameObject.SetActive(false);

            }

        }

        private void BossTagging()
        {
            NumberOfLevel = GameManager.Instance.NumberOfLevel;
                if (NumberOfLevel == 0)
                {
                    target = GameObject.FindWithTag("FirstBoss");
                    if (target.activeInHierarchy)
                        BossTarget = target.GetComponent<EnemyRotationScript>();
                    //BulletTag = "FirstBossBullet";

                    BossTag = "FirstBoss";
                }
                else if (NumberOfLevel == 1)
                {
                target = GameObject.FindWithTag("SecondBoss");
                    if (target.activeInHierarchy)
                        BossTarget = target.GetComponent<EnemyRotationScript>();
                    BossTag = "SecondBoss";
                }
                else if (NumberOfLevel == 2)
                {
                target = GameObject.FindWithTag("ThirdBoss");
                    if (target.activeInHierarchy)
                        BossTarget = target.GetComponent<EnemyRotationScript>();
                    BossTag = "ThirdBoss";
                }
        }
    }
}
