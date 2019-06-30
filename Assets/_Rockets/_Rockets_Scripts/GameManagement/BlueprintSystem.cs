using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rockets
{
    public abstract class Movement : MonoBehaviour
    {
        [HideInInspector]
        public float speed;
        protected Vector3 movement;
        public abstract void Rotate();
        public abstract void Move();
    }

    public abstract class ShipFactory : MonoBehaviour
    {
        [Header("Health and Shields")]
        public float startHealth;
        public float health;
        public float startShield;
        public float shield;

        [Header("Movement")]
        public float startSpeed;
        public float speed;
        public float startRotationSpeed;
        public float rotationSpeed;

        [Header("Shooting")]
        public float startBetweenShots;
        protected float timeBetweenShots;
        public Transform RightMissileEmitter;  //Ракетный барабан правого борта
        public Transform LeftMissileEmitter;   //Ракетный барабан левого борта
        public Transform shootEmitter;
        public GameObject bulletPrefab;
        public GameObject missilePrefab;

        [Header("Прочее")]
        public GameObject parent;

        protected bool MissileEmitterChange;                //Флаг смены ракетного барабана

        protected Transform target;
        
        protected GameObject shieldPrefab;
        protected Slider healthSlider;
        protected Slider shieldSlider;
        protected GameManager gameManager;
        protected Rigidbody rb;
        protected Transform tr;
        protected GameObject gm;

        public abstract void BulletDamage(float BulletDamage, int shieldDamageBoost);
        public abstract void MissileDamage(float MissileDamage, int shieldDamageBoost);
        public abstract void MeteorDamage(float meteorDamage, int shieldDamageBoost);
        public abstract void Shoot();
        public abstract void LaunchMissile();
    }

    public abstract class AmmoFactory : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed;

        [Header("Damage")]
        public float damage;
        public int shieldDamageBoost;

        [Header("Behaviour")]
        public float lifeTime;
        protected float lifeTimer;
        protected Vector3 movement;
        protected Rigidbody rb;
        protected Transform tr;
        protected GameObject target;
        protected string BossTag;
        protected GameManager gameManager;

    }
    public abstract class MissileFactory : AmmoFactory
    {
        [Header("Movement")]
        public float rotationSpeed;

        [Header("Damage")]

        [Header("Behaviour")]
        public float searchTimeDelay;
        public float searchingTime;
        public bool searchFlag;
        protected float searchTimer;
        protected float searchingTimer;

        protected Vector3 startMoveDirection;
        protected Vector3 searchDirection;
        private Quaternion searchRotation;
        public void FindDirection()
        {


            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                gameObject.SetActive(false);
            }

            searchTimer -= Time.deltaTime;

            //movement = startMoveDirection * moveSpeed * Time.deltaTime;

            if (searchTimer <= 0)
            {

                searchRotation = Quaternion.LookRotation(target.transform.position - tr.position, tr.up);
                
                tr.rotation = Quaternion.Slerp(tr.rotation, searchRotation, rotationSpeed * Time.deltaTime);
                movement = tr.forward * moveSpeed * Time.deltaTime;
                searchFlag = true;
            }
        }
            
        public void Pursue()
        {
            if (searchFlag)
            {

                rb.MovePosition(tr.position + movement);

                searchingTimer -= Time.deltaTime;

                if (searchingTimer <= 0)
                {
                    searchTimer = searchTimeDelay;
                    movement = tr.forward * moveSpeed * Time.deltaTime;
                    searchingTimer = searchingTime;
                    searchFlag = false;

                }

                //rb.MovePosition(tr.position + tr.forward);
            }
            else
                rb.MovePosition(tr.position + movement);
        }

    }

    public abstract class BulletFactory : AmmoFactory
    {


        public void MoveAndLive()
        {
            movement = tr.forward * moveSpeed * Time.deltaTime;

            lifeTimer -= Time.deltaTime;
            if (lifeTimer <= 0f)
            {
                gameObject.SetActive(false);
            }
        }

        public void PhysicalMovement()
        {
            rb.MovePosition(tr.position + movement);
        }
    }

    
}