using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rockets
{
    public abstract class BlueprintSystem : MonoBehaviour
    {
        

    }

    public abstract class Movement : MonoBehaviour
    {
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
        public GameObject MissilePrefab;

        protected bool MissileEmitterChange;                //Флаг смены ракетного барабана

        protected Transform target;
        protected Transform shootEmitter;
        protected GameObject parent;
        protected GameObject bulletPrefab;
        protected GameObject shieldPrefab;
        protected GameObject missilePrefab;
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

    public abstract class MissileFactory : MonoBehaviour
    {
        [Header("Movement")]
        public float moveSpeed;
        public float rotationSpeed;

        [Header("Damage")]
        public float damage;
        public int shieldDamageBoost;

        [Header("Searching & Behaviour")]
        public float lifeTime;
        public float searchTimeDelay;
        public float searchingTime;
        public bool searchFlag;
        protected float lifeTimer;
        protected float searchTimer;
        protected float searchingTimer;

        protected Vector3 startMoveDirection;
        private Vector3 searchDirection;
        private Vector3 movement;
        private Quaternion searchRotation;


        protected Rigidbody rb;
        protected Transform tr;
        protected GameObject target;

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


    
}