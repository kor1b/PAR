using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rockets
{
    public class ReloadSlider : MonoBehaviour
    {
        private Slider _slider;
        private PlayerRotationScript playerScript;
        public float reloadDelay;
        void Awake()
        {
            playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerRotationScript>();
            _slider = GetComponent<Slider>();
            reloadDelay = 10.5f - playerScript.startBetweenShots * 2f;
        }

        void FixedUpdate()
        {
            if (_slider.value >= _slider.maxValue)
            {
                return;
            }
            _slider.value += Mathf.Lerp(0.1f, _slider.value, reloadDelay * Time.deltaTime);
        }
    }
}