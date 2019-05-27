using UnityEngine;

//добавляет в сцену игрока, врага и остров соответсвующего уровня
namespace Planes
{
    public class SceneDifficultyManager : MonoBehaviour
    {
        public static SceneDifficultyManager instance;
        //новосозданные объекты
        private GameObject island;
        private GameObject enemy;
        private GameObject player;
        private GameObject lvlPack;
        private GameObject playerAnimation; //объект с анимацией взлета для игрока
        private Takeoff coordinates;
        private CharacterPrint enemyScript;
        private CharacterPrint playerScript;
        private GameObject islandCollider;
        private Transform moneyTarget;

        GameObject controlButtons;
        private void Awake()
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
        }

        private void Start()
        {
            //распраковка уровня
            moneyTarget = GameObject.FindWithTag("Target").GetComponent<Transform>();
            island = GameObject.FindWithTag("Island");
            enemy = GameObject.FindWithTag("Enemy");
            playerAnimation = GameObject.FindWithTag("PlayerAnim");

            player = GameObject.FindWithTag("Player");
            player.transform.SetParent(playerAnimation.transform);
            enemyScript = enemy.GetComponent<CharacterPrint>();
            playerScript = player.GetComponent<CharacterPrint>();
            coordinates = playerAnimation.GetComponent<Takeoff>();

            //устанавливаем позицию и вращение игрока и врага в первоначальную позицию, отключаем коллайдеры острова
            islandCollider = GameObject.FindWithTag("IslandColliders");
            islandCollider.SetActive(false);
            enemy.transform.position = coordinates.EnemyFirstCoordinates;
            enemy.transform.rotation = Quaternion.Euler(coordinates.EnemyFirstRotation);
            playerAnimation.transform.position = coordinates.PlayerFirstCoordinates;
            playerAnimation.transform.rotation = Quaternion.Euler(coordinates.PlayerFirstRotation);
            island.transform.position = coordinates.IslandFirstCoordinates;
            island.transform.rotation = Quaternion.Euler(coordinates.IslandFirstRotation);
        }

        public void TakeoffEnd(bool isPlayer)
        {
            //включаем скрипты после завершения взлета
            if (isPlayer)
            {
                Debug.Log("Player Scripts Enabled");
                //player.transform.SetParent(null);
                player.transform.SetParent(moneyTarget.transform);
                Destroy(playerAnimation);
                playerScript.takeoffMode = false;
            }
            else if (!isPlayer)
            {
                Debug.Log("Enemy Scripts Enabled");
                enemyScript.takeoffMode = false;
            }
            if (!enemyScript.takeoffMode && !playerScript.takeoffMode)
            {
                islandCollider.SetActive(true);
                island.GetComponent<Animator>().enabled = true;
            }
        }
    }
}