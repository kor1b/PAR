using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planes
{
    public class SandStorm : MonoBehaviour
    {
        [SerializeField] [Tooltip("Длительность песчаной бури")]
        private float durability = 15f;
        [SerializeField] [Tooltip("Увеличение скорости движения игрока (умножение)")]
        private float speedIncrease = 2;
        [SerializeField] [Tooltip("Увеличение скорости вращения игрока (умножение)")]
        private float rotationIncrease = 4;
        [SerializeField] [Tooltip("Наносимый урон")]
        private float damage;
        [SerializeField] [Tooltip("Промежутки времени, через которые наносится урон")]
        private float deltaTimeDamage;
        [SerializeField]
        private ParticleSystem _sandStorm; //визуальный эффект бури
        [SerializeField]
        private AudioSource _audio; //звук ветра

        private bool isStorming = false; //активна ли буря в данный момен
        private float timer = 0; //таймер
        private float beginTime = 2f;
        private float timeSinceLastDamage; //для исчисления времени между нанесениями урона
        private Vector3 randomRotation; //направление рандомного кручения
        private double timeForRandomRotate; //время, которое игрок крутится в рандомную сторону

        private Transform _playerTransform; //для закручивания игрока
        private PlayerBluePrint _playerScript; //для нанесения урона игрока
        private PlayerMovement _playerMovement; //для смены параметров игрока
        private FixedJoystick _joystick; //для смены параметров управления

        //сохранить параметры игрока, чтобы вернуть их после конца бури
        private float standartSpeed;
        private float standartRotationSpeed;

        private System.Random random;

        void Start()
        {
            _joystick = GameObject.FindWithTag("Joystick").GetComponent<FixedJoystick>();
            GameObject _player = GameObject.FindWithTag("Player");
            _playerTransform = _player.GetComponent<Transform>();
            _playerMovement = _player.GetComponent<PlayerMovement>();
            _playerScript = _player.GetComponent<PlayerBluePrint>();
            standartSpeed = _playerMovement.speed;
            standartRotationSpeed = _playerMovement.rotationSpeed;
            random = new System.Random();
        }
        
        void Update()
        {
            if (!isStorming)
            {
                return;
            }
            timer += Time.deltaTime;
            if (timer < beginTime) //ждем немного пока начнется анимация
            {
                if (!_sandStorm.isEmitting) //если эффект еще не запущен
                {
                    _sandStorm.Play();
                    _audio.Play();
                }
                return;
            }
            if (isStorming && timer < durability)
            {
                //меняем управление и характеристики игрока
                _joystick.SnapX = true;
                _joystick.SnapY = true;
                _playerMovement.speed = standartSpeed * speedIncrease;
                _playerMovement.rotationSpeed = standartRotationSpeed * rotationIncrease;

                //если джойстик отпущен - начинаем вращать игрока в рандомном направлении
                if (_joystick.Horizontal >= -0.2 && _joystick.Horizontal <= 0.2 && _joystick.Vertical >= -0.2 && _joystick.Vertical <= 0.2)
                {
                    if (timeForRandomRotate > 0) //отсчет того, сколько времени вращаемся в каждую сторону
                    {
                        timeForRandomRotate -= Time.deltaTime;
                        _playerTransform.Rotate(randomRotation * standartRotationSpeed * rotationIncrease);
                    }
                    else //выбор нового направления
                    {
                        timeForRandomRotate = random.NextDouble() * (2 - 0.5) + 0.5;
                        randomRotation.x = (float) random.NextDouble();
                        randomRotation.y = (float)random.NextDouble();
                    }
                }
                else //если игрок использует джойстик, забываем про последний рандомный поворот
                {
                    timeForRandomRotate = 0;
                }
                if (timer - timeSinceLastDamage >= deltaTimeDamage) //наносим урон
                {
                    _playerScript.TakeDamage(damage);
                    timeSinceLastDamage = timer;
                }
            }
            if (isStorming && timer > durability) //когда время бури подошло к концу
            {
                //возвращаем управление и характеристики
                _joystick.SnapX = false;
                _joystick.SnapY = false;
                _playerMovement.speed = standartSpeed;
                _playerMovement.rotationSpeed = standartRotationSpeed;

                //останавливаем эффект, перезагружаем переменные
                _sandStorm.Stop();
                if (_audio.volume > 0.02) //плавно убавляем звук
                {
                    _audio.volume = Mathf.Lerp(_audio.volume, 0, 0.5f * Time.deltaTime);
                }
                else
                {
                    _audio.Stop();
                    timer = 0;
                    isStorming = false;
                    timeSinceLastDamage = 0;
                }
                Debug.Log("Sandstorm ends");
            }
        }

        public void StartSandStorm() //для начала песчаной бури
        {
            isStorming = true;
            _audio.volume = 1;
            timer = 0;
        }
    }
}