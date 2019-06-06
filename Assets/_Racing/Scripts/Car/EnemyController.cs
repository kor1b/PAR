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

		new Transform _transform;
		CarBlueprint carStats;

		private void Start()
		{
			carStats = GetComponent<CarBlueprint>();
			_transform = transform;
			agent = GetComponent<NavMeshAgent>();
			enemySkill = GetComponent<EnemySkillController>();
			standardAgentAcceleration = agent.acceleration;

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
				Vector3 movement = _transform.forward * movementInputValue * movingSpeed * Time.deltaTime * 0.2f;
				//move our player by this vector
				m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
			}
		}

		void EnemyRotation()
		{
			//find our rotation vector
			Vector3 direction = enemyCheckpointTarget.position - _transform.position;
			//look for this vector
			Quaternion lookRotation = Quaternion.LookRotation(direction);
			//rotate our player smoothly
			Vector3 rotation = Quaternion.Lerp(_transform.rotation, lookRotation, Time.deltaTime * turnSpeed).eulerAngles;
			//apply our rotations by y coordinate
			_transform.rotation = Quaternion.Euler(0, rotation.y, 0);
		}

		void NextWaypoint()
		{
			//if our distance is less than switchCheckpointDistance
			if ((_transform.position - enemyCheckpointTarget.position).sqrMagnitude <= switchCheckpointDistance * switchCheckpointDistance)
			{
				Debug.Log(enemyCheckpointTargetIndex);
				//infinity cicle, in the end we will back to first element of array
				enemyCheckpointTargetIndex = (enemyCheckpointTargetIndex + 1) % checkpointsPosition.Length;
				enemyCheckpointTarget = checkpointsPosition[enemyCheckpointTargetIndex];
			}
		}

		public new void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("SwitchAgent"))
			{
				//increase car acceleration to any big num to delete a speed gap when controllers are switching
				agent.acceleration = 500_000;
				//switch controllers
				agent.enabled = !agent.enabled;
			}
		}

		public new void OnTriggerExit(Collider other)
		{
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
