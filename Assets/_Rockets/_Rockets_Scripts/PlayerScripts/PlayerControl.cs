using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class PlayerControl : MonoBehaviour
    {
        public PlayerRotationScript playerStats;  //Характеристики игрока

        Rigidbody rb;
        Transform tr;

        public Joystick joystick;                 //Джойстик управления

        private Vector3 moveDirection;
        private float moveSpeed;
        void Start()
        {
            joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<FixedJoystick>();
            rb = GetComponent<Rigidbody>();
            tr = GetComponent<Transform>();

        }
        void Update()
        {
            moveSpeed = playerStats.moveSpeed; //Подгружаем скорость

            moveDirection = new Vector3(joystick.Horizontal, 0, joystick.Vertical); //Двигаемся в направлении осей джойстика
        }
        void FixedUpdate()
        {
            rb.MovePosition(rb.position + tr.TransformDirection(moveDirection) * moveSpeed * Time.deltaTime); //Двигаем физику
        }




    }
}