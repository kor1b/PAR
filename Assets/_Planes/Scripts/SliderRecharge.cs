using UnityEngine;
using UnityEngine.UI;

namespace Planes
{
    public class SliderRecharge : MonoBehaviour
    {
        [SerializeField] [Tooltip("Скорость восстановления")]
        private float rechargeDelay;
        private Slider _slider;

        void Awake()
        {
            _slider = GetComponent<Slider>();
        }

        void FixedUpdate()
        {
            if (_slider.value >= _slider.maxValue)
            {
                return;
            }
            _slider.value += Mathf.Lerp(0.01f, _slider.value, rechargeDelay * Time.deltaTime); //прибавляется значение в диапазоне от 0.01 до текущего значения, чтобы реализовать медленное заполнение в начале и быстрое в конце
        }
    }
}