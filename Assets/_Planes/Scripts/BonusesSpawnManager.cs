using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planes {
    public class BonusesSpawnManager : MonoBehaviour
    {
        public static BonusesSpawnManager instance;
        [SerializeField] [Tooltip("Префаб бонуса здоровья")]
        private GameObject healthBonusPrefab;
        [SerializeField] [Tooltip("Минимальное время до спавна")]
        private int minTime = 20;
        [SerializeField] [Tooltip("Максимальное время до спавна")]
        private int maxTime = 60;
        private float timeToNextBonus = 10;
        private float timer;
        private System.Random random;
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

            random = new System.Random();
            timeToNextBonus = random.Next(minTime, maxTime); //рандомно выбираем первое время
        }

        void FixedUpdate()
        {
            if (!GameManager.gameIsGoing) //если игра не запущена, не отсчитываем время до бонуса
            {
                return;
            }
            timer += Time.deltaTime;
            if (timer >= timeToNextBonus)
            {
                SpawnHealthBonus();
                timer = 0;
            }
        }

        void SpawnHealthBonus()
        {
            GameObject bonus = ObjectPoolingManager.instance.GetHealthBonus(healthBonusPrefab); //достаем из пула
            Vector3 pos = new Vector3();
            do
            {
                pos.x = random.Next(0, 150);
                pos.y = random.Next(-150, 150);
                pos.z = random.Next(-150, 150);
            } while (Vector3.Distance(new Vector3(0, 0, 0), pos) < 150); //выбираем позицию в допустимой зону
            bonus.GetComponent<Transform>().position = pos; //устанавливаем позицию
            timeToNextBonus = random.Next(minTime, maxTime); //выбираем следующее время бонуса
        } 
    }
}