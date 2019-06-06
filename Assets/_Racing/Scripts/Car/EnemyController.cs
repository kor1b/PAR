using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Racing
{
	public class EnemyController : CarSystem
	{
		//agent
		float standardAgentAcceleration;                    //acceleration variable in navmeshagent

		float switchCheckpointDistance;                     //distance when we change enemy waypoint
		Transform enemyCheckpointTarget;                      //next waypoint of enemy
		int enemyCheckpointTargetIndex = 0;                 //index of waypoint


		NavMeshAgent agent;
		EnemySkillController enemySkill;

		private new void Awake()
		{
			base.Awake();

			agent = GetComponent<NavMeshAgent>();
			enemySkill = GetComponent<EnemySkillController>();
			standardAgentAcceleration = agent.acceleration;
		}

		private void Start()
		{
			switchCheckpointDistance = carStats.switchCheckpointDistance;

			enemyCheckpointTarget = checkpointsPosition[0];
		}

		private void OnEnable()
		{
			agent.enabled = true;
			enemySkill.enabled = true;
		}

		private void OnDisable()
		{
			agent.enabled = false;
			enemySkill.enabled = false;

		}

		private void FixedUpdate()
		{
			if (!OnTurnTurtle())
				EnemyMovement();
		}

		void EnemyMovement()
		{
			//check for the next checkpoint
			if (OnGround())
				NextWaypoint();
			//rotatate our enemy
			EnemyRotation();

			//if we use agent moving system
			if (agent.enabled)
				agent.SetDestination(enemyCheckpointTarget.position);

			else
			{
				//create a vector, that move us forward
				Vector3 movement = transform.forward * movementInputValue * movingSpeed * Time.deltaTime * 0.2f;
				//move our player by this vector
				m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
			}
		}

		void EnemyRotation()
		{
			//find our rotation vector
			Vector3 direction = enemyCheckpointTarget.position - transform.position;
			//look for this vector
			Quaternion lookRotation = Quaternion.LookRotation(direction);
			//rotate our player smoothly
			Vector3 rotation = Quaternion.Lerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
			//apply our rotations by y coordinate
			transform.rotation = Quaternion.Euler(0, rotation.y, 0);
		}

		void NextWaypoint()
		{
			//if our distance is less than switchCheckpointDistance
			if ((transform.position - enemyCheckpointTarget.position).sqrMagnitude <= switchCheckpointDistance * switchCheckpointDistance)
			{
				//infinity cicle, in the end we will back to first element of array
				enemyCheckpointTargetIndex = (enemyCheckpointTargetIndex + 1) % checkpointsPosition.Length;
				enemyCheckpointTarget = checkpointsPosition[enemyCheckpointTargetIndex];
			}
		}

		private new void OnTriggerEnter(Collider other)
		{
			base.OnTriggerEnter(other);

			if (other.CompareTag("SwitchAgent"))
			{
				//increase car acceleration to any big num to delete a speed gap when controllers are switching
				agent.acceleration = 500_000;
				//switch controllers
				agent.enabled = !agent.enabled;
			}
		}

		private new void OnTriggerExit(Collider other)
		{
			base.OnTriggerExit(other);

			if (other.CompareTag("SwitchAgent"))
			{
				agent.acceleration = standardAgentAcceleration;
			}

			else if (other.CompareTag("Finish") && fullLoop)
			{
				if (normalLapsTime == 0)
					normalLapsTime = lapsTime;
			}
		}
	}
}
