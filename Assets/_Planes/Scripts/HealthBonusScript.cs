using UnityEngine;

namespace Planes
{
    public class HealthBonusScript : MonoBehaviour
    {
        [SerializeField] [Tooltip("Прибавка к здоровью")]
        private float healthBonus = 15f;
        [SerializeField] [Tooltip("Время до исчезновения бонуса")]
        private float timeBeforeDisappear = 15f;

        private float timer; //считаем время до исчезновения

        private void Update()
        {
            if (gameObject.activeInHierarchy) //если бонус активен, считаем время до исчезновения
            {
                timer += Time.deltaTime;
            }
            if (timer >= timeBeforeDisappear) //выключаем бонус, когда придет время, и обнуляем таймер
            {
                gameObject.SetActive(false);
                timer = 0;
            }
        }
        
        private void OnTriggerEnter(Collider other) //добавляем здоровье, когда бонус кто-то подберет
        {
            CharacterPrint enteredObj = other.GetComponentInParent<CharacterPrint>();
            if (enteredObj != null) //если столкнулись с самолетом
            {
                enteredObj.TakeDamage(-healthBonus);  //наносим отрицательный урон == прибавляем здоровье
                gameObject.SetActive(false); //выключаем бонус
                timer = 0; //обнуяляем таймер
            }
        }
    }
}