using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class MeteorSpawner : MonoBehaviour
    {
        private GameObject meteorPrefab; //Цельный префаб готового метеорита(С "тенью", эффектами и т.д.)
        public GameObject FrostMeteorPrefab;
        public GameObject FlameMeteorPrefab;
        public GameObject MagmaMeteorPrefab;

        private GameObject AlfaMainMeteor; //Префаб самого камня метеорита внутри цельного префаба
        public GameObject FrostAlfaMainMeteor;
        public GameObject FlameAlfaMainMeteor;
        public GameObject MagmaAlfaMainMeteor;

        float distance;  // Радиус спавна
        
        private float delayTime = 8f; //Частота спавна

        private GameManager gameController;

        void Awake()
        {
            gameController = gameObject.GetComponent<GameManager>();

            if (gameController.NumberOfLevel == 0)
            {
                AlfaMainMeteor = FlameAlfaMainMeteor;
                meteorPrefab = FlameMeteorPrefab;
            }
            else if (gameController.NumberOfLevel == 1)
            {
                AlfaMainMeteor = FrostAlfaMainMeteor;
                meteorPrefab = FrostMeteorPrefab;
            }
            else if (gameController.NumberOfLevel == 2)
            {
                //Debug.Log("Meteor Level 0!");
                AlfaMainMeteor = MagmaAlfaMainMeteor;
                meteorPrefab = MagmaMeteorPrefab;
            }
        }
        void Start()
        {
            if (AlfaMainMeteor != null)
            {
                distance = AlfaMainMeteor.GetComponent<MeteorScript>().distance;

                StartCoroutine(SpawnMeteor());
            }
            else
            {
                //Debug.Log("Meteor is null :(");
            }
        }

        IEnumerator SpawnMeteor()
        {
            Vector3 pos = Random.onUnitSphere * distance;       // Позиция на поверхности "сферы" заданного радиуса

            GameObject meteorObject = ObjectPoolingManager.Instance.GetMeteor(meteorPrefab);  // Вызываем метеорит из пула

            meteorObject.transform.position = pos;                       //Ставим в позицию
            meteorObject.transform.rotation = Quaternion.identity;       //Не куртим

            yield return new WaitForSeconds(delayTime);

            StartCoroutine(SpawnMeteor());
        }
    }
}