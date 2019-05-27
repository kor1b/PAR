using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Planes
{
    public class Lightning : MonoBehaviour
    {
        Vector3 pointA;
        Vector3 pointB;
        int parts;
        [SerializeField]
        int minParts = 8;
        [SerializeField]
        int maxParts = 20;
        Vector3[] arr;
        int offset = 20;
        System.Random random;

        //максимальные значения для первой точки
        int maxY = 300;
        int maxX = 300;
        int maxZ = 300;
        //значения для последней точки
        public int offsetXYZ = 50;

        float x, y, z;

        Transform _thisPoint;
        TrailRenderer _trailRenderer;
        CapsuleCollider _capsuleCollider;
        Transform _colliderObj;
        Transform _player;
        protected bool hitted = false;

        int pointer = 0;

        float timer;
        [SerializeField]
        float enableTime = 4f;

        /*Ray ray;
        RaycastHit rayHit;
        int shootableMask;
        */

        

        void Awake()
        {
            random = new System.Random();
            _thisPoint = GetComponent<Transform>();
            _trailRenderer = GetComponent<TrailRenderer>();
            _capsuleCollider = GetComponentInChildren<CapsuleCollider>();
            _colliderObj = _capsuleCollider.gameObject.GetComponent<Transform>();
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                _player = GetComponent<Transform>();
            }
            else
            {
                _player = null;
            }
        }

        void FixedUpdate()
        {
            if (arr == null)
            {
                timer += Time.deltaTime;
                if (timer >= enableTime)
                {
                    gameObject.SetActive(false);
                    timer = 0;
                }
                else if (timer >= enableTime/2)
                {
                    if (_capsuleCollider != null)
                    {
                        _capsuleCollider.center = new Vector3(0, 0, 0);
                        _capsuleCollider.height = 0;
                    }
                }
                //Debug.Log("Generating new arr");
                /*GenerateNewArr();
                pointer = 0;
                thisPoint.position = arr[0];
                trailRenderer.Clear();*/
                return;
            }
            if (pointer > parts)
            {
                //Debug.Log("Null the arr");
                
                //ray.origin = arr[0];
                //ray.direction = arr[parts];
                /*if (Physics.Raycast(arr[0], arr[parts], out rayHit, Mathf.Infinity, shootableMask)) //если попали по объекту на слое Shootable
                {
                    if (rayHit.collider != null)
                    {
                        Debug.Log("Hit smth");
                    }
                    CharacterPrint obj = rayHit.collider.GetComponentInParent<CharacterPrint>();
                    if (obj != null)
                    {
                        Debug.Log("Hitted a plane!");
                        if (obj.isPlayer)
                        {
                            Debug.Log("Player hitted");
                            obj.TakeDamage(obj.maxHealth / 3); //отнимаем здоровье
                            trailRenderer.Clear();
                        }
                        else if (!obj.isPlayer)
                        {
                            Debug.Log("Enemy hitted");
                        }
                    }
                }*/
                if (hitted)
                {
                    _trailRenderer.Clear();
                    hitted = false;
                }
                arr = null;
                timer = 0;
                return;
            }
            else
            {
                //Debug.Log("Drawing a line");
                _thisPoint.position = arr[pointer];
                float distance = Vector3.Distance(arr[0], arr[pointer]);
                _capsuleCollider.center = new Vector3(0, 0, -distance/2);
                _capsuleCollider.height = distance;

                /*if (Physics.Raycast(arr[pointer - 1], arr[pointer], out rayHit, Vector3.Distance(arr[pointer -1], arr[pointer])/*, shootableMask*//*)) */
               /* {
                    if (rayHit.collider != null)
                    {
                        Debug.Log("Hit smth");
                    }
                    CharacterPrint obj = rayHit.collider.GetComponentInParent<CharacterPrint>();
                    if (obj != null)
                    {
                        Debug.Log("Hitted a plane!");
                        if (obj.isPlayer)
                        {
                            Debug.Log("Player hitted");
                            obj.TakeDamage(obj.maxHealth / 3); //отнимаем здоровье
                            trailRenderer.Clear();
                        }
                        else if (!obj.isPlayer)
                        {
                            Debug.Log("Enemy hitted");
                        }
                    }
                }

                */



                pointer++;

            }

        }

        void CountArr(int begin, int end)
        {
            if (end - begin == 1)
            {
                return;
            }
            int middle = ((end - begin) / 2) + begin;
            //arr[middle] = //Vector3. // arr[begin] + arr[end]; //вот здесь подсчет значения в массиве
            x = (arr[begin].x + arr[end].x) / 2;
            y = (arr[begin].y + arr[end].y) / 2;
            z = (arr[begin].z + arr[end].z) / 2;
            int move = random.Next(-offset, offset);
            x += move;
            move = random.Next(-offset, offset);
            z += move;
            arr[middle] = new Vector3(x, y, z);
            CountArr(begin, middle);
            CountArr(middle, end);
        }

        public void GenerateNewArr() //просчитываем все точки молнии
        {
            x = random.Next(-maxX, maxX);
            z = random.Next(-maxZ, maxZ);
            if (_player != null)
            {
                pointA = new Vector3(x, Mathf.Clamp(_player.position.y + 50, maxY, maxY * 2), z);
                x = random.Next((int)_player.position.x - offsetXYZ, (int)_player.position.x + offsetXYZ);
                z = random.Next((int)_player.position.z - offsetXYZ, (int)_player.position.z + offsetXYZ);
                pointB = new Vector3(x, Mathf.Clamp(_player.position.y, -maxY * 2, 0), z);
            }
            else
            {
                pointA = new Vector3(x, maxY, z);
                x = random.Next(-maxX, maxX);
                z = random.Next(-maxZ, maxZ);
                pointB = new Vector3(x, -maxY, z);
            }
            parts = random.Next(minParts, maxParts);
            arr = new Vector3[parts + 1];
            arr[0] = pointA;
            arr[parts] = pointB;
            CountArr(0, parts);
            /*foreach (Vector3 point in arr)
            {
                Debug.Log("(" + point.x + ", " + point.y + ", " + point.z + ")");
            }*/

            _thisPoint.position = arr[0];
            _trailRenderer.Clear();
            _colliderObj.LookAt(arr[parts]);
            _capsuleCollider.enabled = true;
            _capsuleCollider.center = new Vector3(0, 0, 0);
            _capsuleCollider.height = 0;
            pointer = 1;
        }

        /*private void OnCollisionEnter(Collision collision)
        {
            Debug.Log("Hit smth");
            CharacterPrint obj = collision.collider.GetComponentInParent<CharacterPrint>();
            if (obj != null)
            {
                Debug.Log("Hitted a plane!");
                if (obj.isPlayer)
                {
                    Debug.Log("Player hitted");
                    obj.TakeDamage(obj.maxHealth / 3); //отнимаем здоровье
                    _trailRenderer.Clear();
                }
                else if (!obj.isPlayer)
                {
                    Debug.Log("Enemy hitted");
                }
            }
        }*/
    }
}