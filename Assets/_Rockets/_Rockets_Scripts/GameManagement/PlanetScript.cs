using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class PlanetScript : MonoBehaviour
    {

        public float gravity = -12;  //Сила, с которой планета будет притягивать объекты с весом
        GameManager gameController;
        void Awake()
        {
            gameController = GameManager.Instance;

            if (gameObject.CompareTag("FirstPlanet"))
            {
                gameController.NumberOfLevel = 0;
            }
            else if (gameObject.CompareTag("SecondPlanet"))
            {
                gameController.NumberOfLevel = 1;
            }
            else if (gameObject.CompareTag("ThirdPlanet"))
            {
                gameController.NumberOfLevel = 2;
            }
        }
        public void Attract(Transform playerTransform)
        { 
            Vector3 gravityUp = (playerTransform.position - transform.position).normalized;  //Вектор направления силы
            Vector3 localUp = playerTransform.up;
            playerTransform.GetComponent<Rigidbody>().AddForce(gravityUp * gravity);  //Применение силы в заданном направлении

            Quaternion targetRotation = Quaternion.FromToRotation(localUp, gravityUp) * playerTransform.rotation; //Поворот по шару

            playerTransform.rotation = Quaternion.Slerp(playerTransform.rotation, targetRotation, 10f * Time.deltaTime); //Плавность поворота
        }
    }
}