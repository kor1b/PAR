using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planes
{
    public class ObjectPoolingManager : MonoBehaviour
    {
        public static ObjectPoolingManager instance;
        [SerializeField] [Tooltip("Префаб молнии")]
        private GameObject Lightning;
        [Tooltip("Префаб взрыва (для хранения, не для пула)")]
        public GameObject Explosion; //не пулится, просто хранится здесь

        //изначальные размеры очереди
        private int healthBonusAmount = 3;
        private int lightningsAmount = 7;
        //очереди объектов на пул
        private Queue<GameObject> healthBonuses;
        private Queue<GameObject> lightnings;
        //родители объектов на пул
        private Transform healthBonusesParent;
        private Transform lightningsParent;
        
        void Awake()
        {
            #region Singleton
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            #endregion

            healthBonuses = new Queue<GameObject>(healthBonusAmount);
            healthBonusesParent = GameObject.FindWithTag("Bonuses").GetComponent<Transform>();
            lightnings = new Queue<GameObject>(lightningsAmount);
            lightningsParent = GameObject.FindWithTag("Lightnings").GetComponent<Transform>();
        }

        public GameObject GetHealthBonus(GameObject HealthBonus)
        {
            foreach (GameObject healthBonus in healthBonuses)
            {
                if (!healthBonus.activeInHierarchy)
                {
                    healthBonus.SetActive(true);
                    return healthBonus; //если найден объект, который можно переиспользовать
                }
            }
            //если объект для переиспользования не найден - просто создаем новый и добавляем его в очередь
            GameObject prefabInstance = Instantiate(HealthBonus);
            prefabInstance.transform.SetParent(healthBonusesParent);
            healthBonuses.Enqueue(prefabInstance);
            return prefabInstance;
        }

        public GameObject GetLinghning()
        {
            foreach (GameObject lightning in lightnings)
            {
                if (!lightning.activeInHierarchy)
                {
                    lightning.SetActive(true);
                    return lightning; //если найден объект, который можно переиспользовать
                }
            }
            //если объект для переиспользования не найден - просто создаем новый и добавляем его в очередь
            GameObject prefabInstance = Instantiate(Lightning);
            prefabInstance.transform.SetParent(lightningsParent);
            lightnings.Enqueue(prefabInstance);
            return prefabInstance;
        }
    }
}