using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class PlayerGravityBody : MonoBehaviour
    {
        [HideInInspector]
        public PlanetScript attractorPlanet;  //Тело к которому будет питягиваться наш объект

        private Transform playerTransform;
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
                attractorPlanet = GameObject.FindGameObjectWithTag("Planet").GetComponent<PlanetScript>();
            }

            if (attractorPlanet)
            {
                attractorPlanet.Attract(playerTransform);
            }
        }
    }
}