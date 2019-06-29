using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class MeteorScript : MonoBehaviour
    {

        public GameObject ShadowPrefab;  //"Тень" метеорита

        Ray directionRay;                //Луч для определения места падения
        RaycastHit MeteorFallOnPlace;    //Место падения
        int mask = 8;                    //Маска, слой для метеорита

        public float distance = 10f;    //Дистанция спавна

        bool shadowFlag;                 //Флаг создания тени

        void Start()
        {
            mask = LayerMask.GetMask("Meteor"); //Задаём маску
        }
        void OnEnable()
        {
            shadowFlag = true;                  //Разрешаем создать тень, как только появляется метеорит
            ShadowPrefab.SetActive(false);      //Не показываем тень раньше времени
        }
        void Update()
        {

            /* Ищем поверхность лучём */
            directionRay.origin = transform.position;
            directionRay.direction = -transform.forward;

            if (Physics.Raycast(directionRay, out MeteorFallOnPlace, distance, mask))
            {
                Quaternion rot = Quaternion.FromToRotation(Vector3.up, MeteorFallOnPlace.normal);
                Vector3 fallingPos = MeteorFallOnPlace.point;

                /* Если нашли поверхность, то создаём на ней тень в найденном месте */
                if (MeteorFallOnPlace.collider.CompareTag("PlanetSurface"))
                {

                    if (shadowFlag)
                    {
                        //Debug.Log("SurfaceDetected and Shadow is active!");
                        ShadowPrefab.SetActive(true);
                        shadowFlag = false;
                    }

                    ShadowPrefab.transform.position = fallingPos;
                    ShadowPrefab.transform.rotation = rot;
                }

                //Debug.Log("SomethingtDetected!");
            }
            else
            {

            }


        }
    }
}