using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planes {
    public class Thunderstorm : MonoBehaviour
    {
        bool isStorming = false;
        [SerializeField]
        float stormDurabillity = 10f;
        float timer = 0;

        System.Random random;
        float timeSinceLast = 0;
        float timeBeforeNext;

        [Header("[a, b) - range of time between lightnings")]
        [SerializeField]
        float a = 0f;
        [SerializeField]
        float b = 2f;
        [SerializeField] [Tooltip("Звук грома")]
        AudioSource _thunder;
        
        void Awake()
        {
            random = new System.Random();
            timeBeforeNext = (float)random.NextDouble() * (b - a) + a;
        }
        
        void FixedUpdate()
        {
            if (!isStorming)
            {
                return;
            }
            if (timer > stormDurabillity)
            {
                if (_thunder.volume > 0.02)
                {
                    _thunder.volume = Mathf.Lerp(_thunder.volume, 0f, 0.5f * Time.deltaTime);
                }
                else
                {
                    timer = 0;
                    isStorming = false;
                    _thunder.Stop();
                    timeSinceLast = 0;
                    timeBeforeNext = (float)random.NextDouble() * (b - a) + a;
                }
                return;
            }
            timer += Time.deltaTime;
            if (timer - timeSinceLast >= timeBeforeNext)
            {
                Lightning lightning = ObjectPoolingManager.instance.GetLinghning().GetComponent<Lightning>();
                lightning.GenerateNewArr();
                timeSinceLast = timer;
                timeBeforeNext = (float)random.NextDouble() * (b - a) + a;
            }
        }

        public void StartThunderstorm()
        {
            isStorming = true;
            _thunder.volume = 0.7f;
            _thunder.Play();
        }
    }
}