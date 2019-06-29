using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class PlayerControl : Movement
    {
        private PlayerRotationScript playerStats;  //Характеристики игрока

        Rigidbody rb;
        Transform tr;

        private Joystick joystick;                 //Джойстик управления

        private Vector3 moveDirection;
        private float moveSpeed;
        void Start()
        {
            joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<FixedJoystick>();
            rb = GetComponent<Rigidbody>();
            tr = GetComponent<Transform>();
            playerStats = GetComponentInChildren<PlayerRotationScript>();

        }
        void Update()
        {
            Rotate();
        }
        void FixedUpdate()
        {
            Move();
        }

        public override void Rotate()
        {
            moveSpeed = playerStats.speed; //Подгружаем скорость

            moveDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical); //Двигаемся в направлении осей джойстика
        }

        public override void Move()
        {
            rb.MovePosition(rb.position + tr.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime); //Двигаем по физике
        }


    }
}