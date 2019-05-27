using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racing {
	public class EnvironmentStats : MonoBehaviour
	{
		public Transform playerSpawn;               //player spawn point
		public Quaternion playerRotation;
		public Vector3 environmentPosition;			//environment spawn

		private void Awake()
		{
			//set environment pos
			transform.position = environmentPosition;

			//set player pos
			Transform player = GameObject.FindWithTag("Player").transform;

			player.SetParent(gameObject.transform);

			player.position = playerSpawn.position;
			player.rotation = playerRotation;
		}
	}
}
