using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racing {
	public class BonusManager : MonoBehaviour
	{
		public static BonusManager Instance;

		[Tooltip("Prefab of speedUp bonus")]
		public GameObject speedUpPrefab;
		GameObject speedUpBonus;                    //instance of bonus

		public Transform[] spawnPositions;          //positions to spawn a bonus

		[Tooltip("Time before first spawn")]
		public float startSpawnTime = 5;
		[Tooltip("Time between spawns")]
		public float timeBtwSpawn = 3;

		[HideInInspector]
		public bool bonusSpawned = false;           //flag did we spawn a bonus

		#region Singleton
		private void Awake()
		{
			if (Instance != null)
				return;
			Instance = this;
		}
		#endregion

		private void Start()
		{
			//find checkpoint holder
			Transform pathHolder = GameObject.FindWithTag("Checkpoint").transform;
			Transform instantiatedHolder = GameObject.FindWithTag("Instantiated").transform;
			spawnPositions = new Transform[pathHolder.childCount];

			for (int i = 0; i < pathHolder.childCount; i++)
			{
				spawnPositions[i] = pathHolder.GetChild(i);
			}

			//create our speedUp instance
			speedUpBonus = Instantiate(speedUpPrefab, Vector3.zero, Quaternion.identity, instantiatedHolder);
			//disable that
			speedUpBonus.SetActive(false);
			//start infinity cicle of spawning
			InvokeRepeating("SpawnBonus", startSpawnTime, timeBtwSpawn);
		}

		void SpawnBonus()
		{
			//if we dont have a bonus on our plane
			if (!bonusSpawned)
			{
				//random our spawnPos indexes
				int randPos = Random.Range(0, spawnPositions.Length - 1);
				//apply this position
				speedUpBonus.transform.position = spawnPositions[randPos].position;
				//enable our bonus instance
				speedUpBonus.SetActive(true);
				//say that we already spawned a bonus
				bonusSpawned = true;
			}
		}
	}
}
