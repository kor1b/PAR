using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Planes
{
    public abstract class CharacterPrint : MonoBehaviour
    {
        [Tooltip("Уровень персонажа")]
        public int level;
        [Header("Health")] [Tooltip("Максимальное здоровье")]
        public float maxHealth = 100f;
        [Tooltip("Здоровье в данный момент")]
        public float health; //здоровье
        [Header("General Shooting")]
        [SerializeField] [Tooltip("Минимальное время между выстрелами")]
        protected float timeBetweenShoots = 0.3f; //время между выстрелами
        [Tooltip("Диапазон самонаведения")]
        public float aimRange = 10f;
        [SerializeField] [Tooltip("Точки, откуда самолет стреляет")]
        protected SingleShoot[] shootingPoints;
        [SerializeField] [Tooltip("Минимальная длительность аудио стрельбы")]
        protected float audioMinDurrability = 0.1f; //для контроля аудио
        [SerializeField] [Tooltip("Компонент источник аудио для выстрела")]
        protected AudioSource _shootAudio;

        [HideInInspector]
        public bool isPlayer; //является ли наследник класса игроком (true) или врагом (false)
        [HideInInspector]
        public bool takeoffMode = true; //когда влючено, функции Update не выполняются (перехват контроля анимацией взлета)
        protected float effectsDisplayTime = 0.2f; //время отображения эффектов стрельбы
        protected Slider _sliderHealth; //слайдер, показывающий здоровье
        protected float audioDurrability; //как долго аудио проигрывается на данный момент
        protected bool stopAudio = false; //указание остановить аудио как только это станет возможно

        public static float CRUSH = -1; //крушение: мгновенно убивает персонажа
        
        public abstract void Shooting(); //стрельба персонажа

        virtual public void TakeDamage(float damage) //отнять у персонажа здоровье
        {
            if(GameManager.Instance.gameEnded)
            {
                return;
            }
            if (damage == CRUSH) //мгновенная смерть
            {
                health = 0;
            }
            else
            {
                health -= damage; //отнимаем урон
            }
            
            if (health <= 0) //если игрок умер
            {
                health = 0;
                Death();
            }
            if (health > maxHealth) //не даем превышать максимум здоровья
            {
                health = maxHealth;
            }
            _sliderHealth.value = health; //обновляем слайдер
        }

        void Death()
        {
            gameObject.SetActive(false);
            GameObject explosion = Instantiate(ObjectPoolingManager.instance.Explosion);
            explosion.GetComponent<Transform>().position = gameObject.GetComponent<Transform>().position;
            explosion.GetComponent<ParticleSystem>().Play();
            Destroy(explosion, 7f);
            GameManager.Instance.EndGame(GetComponent<CharacterPrint>()); //отчитываемся GameManager
        }


      //  abstract protected void Death(); //вызывается при смерти персонажа
    }
}