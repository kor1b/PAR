using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racing
{
	public class EnemySkillController : MonoBehaviour
	{
		public Transform pathHolder;

		public enum EnemySkill { None, ShockWave, SpawnEggs };
		public EnemySkill enemySkill;

		[Tooltip("Time after which enemy uses the skill")]
		public float skillReloadTime;
		float skillCountdown = 0;                                   //timer that we decrease to ckeck a reload time

		[Header("Shock Wave")]
		[Tooltip("Radius of circle of action")]
		public float shockWaveRadius;
		[Tooltip("Coef by which we multiply a shock force")]
		public float forceCoef;
		public float shockTime = 1;									//time of shock action
		float shockTImeCountdown;									//help variable to countdown time of action
		bool shocked = false;										//is the player shocking now?

		[Space]
		[Header("Spawns Eggs")]
		public GameObject eggPrefab;
		GameObject egg;


		//cache
		Transform player;
		Rigidbody playerRB;
		Transform _transform;

		private void Start()
		{
			//cache
			player = GameObject.FindGameObjectWithTag("Player").transform;
			playerRB = player.GetComponent<Rigidbody>();
			_transform = transform;

			//set countdowntimer to start value
			skillCountdown = skillReloadTime;

			shockTImeCountdown = shockTime;

			//spawn the egg at the (0, 0, 0) and disable it
			if (enemySkill == EnemySkill.SpawnEggs)
			{
				egg = Instantiate(eggPrefab, Vector3.zero, Quaternion.identity);
				egg.SetActive(false);
			}
		}

		private void Update()
		{
			if (GameManager.gameIsGoing)
			{
				if (skillCountdown <= 0)
					UseSkill();

				else
					skillCountdown -= Time.deltaTime;
			}
		}

		public void UseSkill()
		{
			//if shock effect is not attempted
			if (enemySkill == EnemySkill.ShockWave && !shocked)
			{
				//if player is in the trigger zone
				if (Vector3.Distance(player.position, _transform.position) < shockWaveRadius)
				{
					Debug.Log("SHOCK");

					StartCoroutine(Shock());
					//reload skill
					skillCountdown = skillReloadTime;
				}
			}

			//move the egg to the new position, enable it and reset countdown timer
			else if (enemySkill == EnemySkill.SpawnEggs)
			{
				egg.SetActive(true);

				egg.transform.position = transform.position;

				skillCountdown = skillReloadTime;
			}
		}

		IEnumerator Shock()
		{
			shocked = true;

			//while the player is shocking 
			while (shockTImeCountdown > 0)
			{
				//add explosion force to player
				playerRB.AddExplosionForce(forceCoef, _transform.position - player.position, shockWaveRadius, 0, ForceMode.Impulse);
				//shockTime is going
				shockTImeCountdown -= Time.deltaTime;
				yield return null;
			}
			//reset time
			shockTImeCountdown = shockTime;
			shocked = false;
		}

		//Draw waypoints positions
		private void OnDrawGizmosSelected()
		{
			Vector3 startPos = pathHolder.GetChild(0).position;
			Vector3 previousPos = startPos;

			foreach (Transform waypoint in pathHolder)
			{
				Gizmos.DrawSphere(waypoint.position, 0.1f);
				Gizmos.DrawLine(previousPos, waypoint.position);

				previousPos = waypoint.position;
			}

			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position, shockWaveRadius);
		}
	}
}
