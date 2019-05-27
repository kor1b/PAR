using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Planes
{
    public class FireButtonScript : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
    {
        bool shooting = false;
        bool stopShooting = false;
        PlayerBluePrint _playerShooting;
        float timer;
        [SerializeField]
        float minTimeShooting = 0.2f;

        private void Start()
        {
            _playerShooting = GameObject.FindWithTag("Player").GetComponent<PlayerBluePrint>();
        }
        
        private void Update()
        {
            timer += Time.deltaTime;
            if (shooting)
            { 
                _playerShooting.Shooting();
            }
            if (stopShooting && timer > minTimeShooting)
            {
                _playerShooting.FireButtonReleased();
                shooting = false;
                stopShooting = false;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            shooting = true;
            timer = 0;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            shooting = false;
            if (timer < minTimeShooting)
            {
                stopShooting = true;
            }
            else
            {
                _playerShooting.FireButtonReleased();
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            shooting = true;
        }
    }
}