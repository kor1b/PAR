using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Rockets
{
    public class ObjectPoolingManager : MonoBehaviour
    {


        private static ObjectPoolingManager instance;
        public static ObjectPoolingManager Instance { get { return instance; } }


        public int bulletAmount = 20;
        public int meteorAmount = 20;
        public int missileAmount = 20;

        private Queue<GameObject> playerBullets;
        private Queue<GameObject> enemyBullets;
        private Queue<GameObject> meteors;
        private Queue<GameObject> shadows;
        private Queue<GameObject> playerMissiles;
        private Queue<GameObject> enemyMissiles;

        void Awake()
        {
            instance = this;            // Делаем ссылку

            /* Создаём очереди для пуллинга всех необходимых объектов */
            playerBullets = new Queue<GameObject>(bulletAmount);
            enemyBullets = new Queue<GameObject>(bulletAmount);
            meteors = new Queue<GameObject>(meteorAmount);
            shadows = new Queue<GameObject>(meteorAmount);
            playerMissiles = new Queue<GameObject>(missileAmount);
            enemyMissiles = new Queue<GameObject>(missileAmount);
        }

        public GameObject GetPlayerBullet(GameObject BulletPrefab)
        {

            foreach (GameObject playerBullet in playerBullets)
            {
                if (!playerBullet.activeInHierarchy)
                {
                    playerBullet.SetActive(true);
                    return playerBullet;
                }
            }

            GameObject prefabInstance = Instantiate(BulletPrefab);
            prefabInstance.transform.SetParent(transform);
            playerBullets.Enqueue(prefabInstance);

            return prefabInstance;
        }

        public GameObject GetEnemyBullet(GameObject BulletPrefab)
        {
            foreach (GameObject enemyBullet in enemyBullets)
            {
                if (!enemyBullet.activeInHierarchy)
                {
                    enemyBullet.SetActive(true);
                    return enemyBullet;
                }
            }
            GameObject prefabInstance = Instantiate(BulletPrefab);
            prefabInstance.transform.SetParent(transform);
            enemyBullets.Enqueue(prefabInstance);

            return prefabInstance;
        }

        public GameObject GetMeteor(GameObject MeteorPrefab)
        {
            foreach (GameObject meteor in meteors)
            {
                if (!meteor.activeInHierarchy)
                {
                    meteor.SetActive(true);
                    return meteor;
                }
            }
            GameObject prefabInstance = Instantiate(MeteorPrefab);
            prefabInstance.transform.SetParent(transform);
            meteors.Enqueue(prefabInstance);

            return prefabInstance;
        }

        public GameObject GetShadow(GameObject ShadowPrefab)
        {
            foreach (GameObject shadow in shadows)
            {
                if (!shadow.activeInHierarchy)
                {
                    shadow.SetActive(true);
                    return shadow;
                }
            }
            GameObject prefabInstance = Instantiate(ShadowPrefab);
            prefabInstance.transform.SetParent(transform);
            shadows.Enqueue(prefabInstance);

            return prefabInstance;
        }

        public GameObject GetPlayerMissile(GameObject MissilePrefab)
        {
            foreach (GameObject playermissile in playerMissiles)
            {
                if (!playermissile.activeInHierarchy)
                {
                    playermissile.SetActive(true);
                    return playermissile;
                }
            }
            GameObject prefabInstance = Instantiate(MissilePrefab);
            prefabInstance.transform.SetParent(transform);
            playerMissiles.Enqueue(prefabInstance);

            return prefabInstance;
        }

        public GameObject GetBossMissile(GameObject MissilePrefab)
        {
            foreach (GameObject enemymissile in enemyMissiles)
            {
                if (!enemymissile.activeInHierarchy)
                {
                    enemymissile.SetActive(true);
                    return enemymissile;
                }
            }
            GameObject prefabInstance = Instantiate(MissilePrefab);
            prefabInstance.transform.SetParent(transform);
            enemyMissiles.Enqueue(prefabInstance);

            return prefabInstance;
        }
    }
}