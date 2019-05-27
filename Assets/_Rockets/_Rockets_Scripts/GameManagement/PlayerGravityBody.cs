using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class PlayerGravityBody : MonoBehaviour
    {

        public PlanetScript attractorPlanet;  //Тело к которому будет питягиваться наш объект

        public GameManager gameController;

        public int NumberOfLevel;

        private Transform playerTransform;

        void Awake()
        {
            gameController = GameManager.Instance;

           

            
        }
        void Start()
        {
            GetComponent<Rigidbody>().useGravity = false;   //Мы создаём свою гравитацию
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;  //Со своим вращением

            playerTransform = transform;
        }

        void FixedUpdate()
        {
            if (attractorPlanet == null)
            {
                /* Подвязываем планету к игроку в зависимости от уровня */
                NumberOfLevel = gameController.NumberOfLevel;
                //if (gameObject.CompareTag("PlayerParent") || gameObject.CompareTag("PlayerMissile"))
                //{
                if (NumberOfLevel == 0)
                {
                    attractorPlanet = GameObject.FindGameObjectWithTag("FirstPlanet").GetComponent<PlanetScript>();
                }
                else if (NumberOfLevel == 1)
                {
                    attractorPlanet = GameObject.FindGameObjectWithTag("SecondPlanet").GetComponent<PlanetScript>();
                }
                else if (NumberOfLevel == 2)
                {
                    attractorPlanet = GameObject.FindGameObjectWithTag("ThirdPlanet").GetComponent<PlanetScript>();
                }
                //}
            }

            if (attractorPlanet)
            {
                attractorPlanet.Attract(playerTransform);
            }
        }
    }
}