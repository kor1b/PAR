using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planes
{
    public class Lightning : MonoBehaviour
    {
        [SerializeField] [Tooltip("Минимальная часть \"изломов\"")]
        int minParts = 8;
        [SerializeField] [Tooltip("Максимальная часть \"изломов\"")]
        int maxParts = 20;
        [SerializeField] [Tooltip("Время жизни объекта")]
        float enableTime = 4f;
        [SerializeField] [Tooltip("Значения для последней точки")]
        private int offsetXYZ = 50;
        [SerializeField]
        Transform _thisPoint; //положение этой молнии
        [SerializeField]
        TrailRenderer _trailRenderer; //прорисовка молнии
        [SerializeField]
        CapsuleCollider _capsuleCollider; //коллайдер молнии
        [SerializeField]
        Transform _colliderObj; //позиция объекта коллайдера

        //начальная и конечная точка молнии
        Vector3 pointA;
        Vector3 pointB;
        int parts; //количество изломов молнии
        
        Vector3[] arr; //каждая из точек излома молнии
        int offset = 20; //отклонение от главной линии
        System.Random random;

        //максимальные значения для первой точки
        int maxY = 300;
        int maxX = 300;
        int maxZ = 300;
        Transform _player; //отслеживаем положение игрока
        protected bool hitted = false; //ударен ли игрок

        float x, y, z; //временные координаты для подсчета
        int pointer = 0; //указатель на точку, которая отрисовывается
        float timer; //подсчет времени, когда активна молния
        
        void Awake()
        {
            random = new System.Random();
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
                _player = GetComponent<Transform>();
            else
                _player = null;
        }

        void FixedUpdate()
        {
            if (arr == null)
            {
                timer += Time.deltaTime;
                if (timer >= enableTime) //убираем молнию, как только она отжила свое
                {
                    gameObject.SetActive(false);
                    timer = 0;
                }
                else if (timer >= enableTime/2)  //убираем коллайдер, как только молния отжила половину жизни
                {
                    if (_capsuleCollider != null)
                    {
                        _capsuleCollider.center = new Vector3(0, 0, 0);
                        _capsuleCollider.height = 0;
                    }
                }
                return;
            }
            if (pointer > parts) //когда молния полностью отрисована
            {
                if (hitted) //если мы попали в игрока, убираем ее быстро
                {
                    _trailRenderer.Clear();
                    hitted = false;
                }
                //обнуляем массив точек и таймер
                arr = null; 
                timer = 0;
                return;
            }
            else
            {
                //прорисовка молнии: движемся по каждой точке, просчитанной в массиве
                _thisPoint.position = arr[pointer];
                //растягиваем коллайдер
                float distance = Vector3.Distance(arr[0], arr[pointer]);
                _capsuleCollider.center = new Vector3(0, 0, -distance/2);
                _capsuleCollider.height = distance;
                pointer++;
            }

        }

        void CountArr(int begin, int end) //просчитывает точки молнии, запускается при помощи GenerateNewArr()
        {
            if (end - begin == 1)
            {
                return;
            }
            int middle = ((end - begin) / 2) + begin; //середина массива, над которым сейчас работаем
            //находим середину по каждой координате
            x = (arr[begin].x + arr[end].x) / 2;
            y = (arr[begin].y + arr[end].y) / 2;
            z = (arr[begin].z + arr[end].z) / 2;
            //"сдвигаем" середину на рандомное значение
            int move = random.Next(-offset, offset); 
            x += move;
            move = random.Next(-offset, offset);
            z += move;
            arr[middle] = new Vector3(x, y, z);
            //передаем в работу остальные половинки массива
            CountArr(begin, middle);
            CountArr(middle, end);
        }

        public void GenerateNewArr() //запуск просчета всех точек
        {
            x = random.Next(-maxX, maxX);
            z = random.Next(-maxZ, maxZ);
            if (_player != null) //рандомно выбираем точки, но недалеко от игрока
            {
                pointA = new Vector3(x, Mathf.Clamp(_player.position.y + 50, maxY, maxY * 2), z);
                x = random.Next((int)_player.position.x - offsetXYZ, (int)_player.position.x + offsetXYZ);
                z = random.Next((int)_player.position.z - offsetXYZ, (int)_player.position.z + offsetXYZ);
                pointB = new Vector3(x, Mathf.Clamp(_player.position.y, -maxY * 2, 0), z);
            }
            else //если игрока уже нет
            {
                pointA = new Vector3(x, maxY, z);
                x = random.Next(-maxX, maxX);
                z = random.Next(-maxZ, maxZ);
                pointB = new Vector3(x, -maxY, z);
            }
            parts = random.Next(minParts, maxParts); //выбираем количество изломов молнии
            arr = new Vector3[parts + 1];
            //устанавливаем первую и последнюю точку молнии
            arr[0] = pointA;
            arr[parts] = pointB;
            CountArr(0, parts); //запуск просчета точек массива

            _thisPoint.position = arr[0]; //установка объекта в начальную позицию
            _trailRenderer.Clear(); //сразу же убираем то, что прорисовалось от этого действия
            _colliderObj.LookAt(arr[parts]); //ориентация коллайдера
            //включаем коллайдер и обнуляем его
            _capsuleCollider.enabled = true; 
            _capsuleCollider.center = new Vector3(0, 0, 0);
            _capsuleCollider.height = 0;
            pointer = 1; //в нулевой точке мы уже находимся, начинаем с первой
        }
    }
}