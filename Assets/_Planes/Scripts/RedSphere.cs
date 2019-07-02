using UnityEngine;

//Показывает вокруг игрока красный шар, предупреждающий, что он улетел слишком далеко
namespace Planes
{
    public class RedSphere : MonoBehaviour
    {
        [Header("Current State")]
        [Tooltip("Убирается ли сфера в данный момент")]
        public bool disabling = false; //показывает, убирается ли шар в данный момент
        [SerializeField] [Tooltip("Создается ли сфера в данный момент")]
        private bool enabling = false; //показывает, создается ли шар в данный момент
        [Tooltip("Находится ли игрок в допустимой сфере-зоне")]
        public bool inSphere; //показывает, находится игрок внутри допустимой зоны-сферы

        [Header("Time Settings")]
        [SerializeField] [Tooltip("Скорость прорисовки сферы")]
        private float enablingTime = 0.3f; //скорость создания шара
        //[SerializeField] [Tooltip("Время от начала прорисовки сферы до нанесения первого урона")]
        private float timeBeforeDamage = 7f;
        //[SerializeField] [Tooltip("Время между нанесениями урона")]
        private float deltaTimeDamage = 1f;

        [Header("Other Settings")]
        [SerializeField] [Tooltip("Максимальная непрозрачность, которую достигает сфера")]
        private float maxAlphaOfSphere = 1f; //максимальная непрозрачность шара
        //[SerializeField] [Tooltip("Единицы урона сферы")]
        private float damage;

        [Header("Components")]
        [SerializeField]
        Transform _player; //для отслеживания позиции игрока
        [SerializeField]
        Transform _nose; //точка, с которой ведется прорисовка вспомогательной линии
        [SerializeField]
        LineRenderer _way; //вспомогательная линия для возвращения в сферу-зону

        GameObject _comeBackText; //текст, оповещающий о вылете из сферы-зоны
        Material _sphere; //материал сферы для изменения его прозрачности
        float currentAlpha; //прозрачность в данный момент

        float timer; //подсчет времени
        float timeSinceLastDamage; //момент, когда урон наносился в последний раз
        [SerializeField]
        PlayerBluePrint _playerScript; //для нанесения урона игроку

        void Awake()
        {
            _sphere = GetComponent<MeshRenderer>().material;
            currentAlpha =  _sphere.GetFloat("_alpha");
            _way = GetComponent<LineRenderer>();
            _comeBackText = GameObject.FindWithTag("ComeBackText");
            _comeBackText.SetActive(false);
            damage = _playerScript.maxHealth / 25f;
        }

        private void Update()
        {
            if (!GameManager.gameIsGoing || _playerScript.takeoffMode)
            {
                return;
            }
            if (enabling) //нанесение урона
            {
                timer += Time.deltaTime;
                if (timer > timeBeforeDamage) //начинаем наносить урон только если истекло допустимое время после вылета из сферы-зоны
                {
                    if (timer - timeSinceLastDamage > deltaTimeDamage) //считаем промежутки времени, когда может наноситься урон
                    {
                        _playerScript.TakeDamage(damage);
                       timeSinceLastDamage = timer;
                    }
                }
            }
            else //если игрое вернулся в сферу-зону, в следующий раз начинаем отсчет сначала
            {
                timer = 0;
                timeSinceLastDamage = 0;
            }
        }

        private void FixedUpdate()
        {
            if (!GameManager.gameIsGoing || _playerScript.takeoffMode)
            {
                return;
            }
            if (inSphere && _player.position.y <= 0) //если игрок в допустимой зоне-сфере, но ниже купюры
            {
                disabling = false; //прекратить убирать красный шар
                EnableSphere(); //создание красного шара
                return;
            }
            if (inSphere && _player.position.y > 0) //если игрок в зоне-сфере и выше купюры
            {
                DisableSphere(); //убирать красный шар
                return;
            }

            if (enabling) //обеспечивает прорисовку красного шара, когда игрок не в сфере-зоне
            {
                EnableSphere();
            }
        }

        public void DisableSphere() //убирает красный шар
        {
            if (_player.position.y < 0) //если игрок находится ниже купюры, то убирать шар никак нельзя
            {
                return;
            }
            else //если шар убирать можно, то нужно "выключить" его создание
            {
                enabling = false;
            }
            if (currentAlpha == 0) //если прозрачность шара достигла нуля, то есть он не виден, прекратить выполнять функцию
            {
                disabling = false;
                return;
            }
            disabling = true; //показать, что в данный момент шар убирается

            _sphere.SetFloat("_alpha", Mathf.Lerp(currentAlpha, 0f, enablingTime * Time.deltaTime)); //постепенно убираем непрозрачность - от текущей до нуля
            currentAlpha = _sphere.GetFloat("_alpha");

            //убираем рекомендуемый путь
            _way.enabled = false;
            //убираем предупреждение
            _comeBackText.SetActive(false);
        }

        public void EnableSphere()
        {
            if (currentAlpha == maxAlphaOfSphere) //если красный шар достиг своей максимальной непрозрачности, прекращаем выполнение функции
            {
                enabling = false;
                return;
            }
            enabling = true; //показать, что в данный момент шар прорисовывается

            _sphere.SetFloat("_alpha", Mathf.Lerp(currentAlpha, maxAlphaOfSphere, enablingTime * Time.deltaTime)); //постепенно добавляем непрозраность - от текущей до максимальной
            currentAlpha = _sphere.GetFloat("_alpha");

            //прорисовка пути к купюре
            _way.enabled = true;
            _way.SetPositions(new Vector3[] { _nose.position, new Vector3(0, 0, 0)});
            //вывод предупреждения
            _comeBackText.SetActive(true);
        }
    }
}