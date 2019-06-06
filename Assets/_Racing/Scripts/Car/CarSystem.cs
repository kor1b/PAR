using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;

namespace Racing
{
	public class CarSystem : MonoBehaviour
	{
		//public enum Character { Player, Enemy }
		//[Tooltip("Кто наша машина: игрок или враг")]
		//public Character character;                         //who is our car: player or enemy

		public GameObject explosionPrefab;

		[Header("CheckPoints")]
		public Transform checkpointsHolder;
		protected Transform[] checkpointsPosition;
		Transform lastCheckpointPos;                          //last checpoint position
		protected bool fullLoop = false;                              //did the player complete a full loop?

		float normalMovingSpeed;
		protected float movingSpeed;
		float stopSpeed = 30;                               //Speed with which our car stop in front of the board
		float accelerationSpeed;
		protected float turnSpeed;

		public float lapsTime { get; private set; } = 0;
		public static float normalLapsTime = 0;


		float turnTurtleDistance;                           // Distance to the ground if the car is turned turtle
		float toGroundDistance;                             // Distance to the ground if the car is normal
		float toBorderDistance;                             // Distance to bord if car is leaned on board

		protected Rigidbody m_Rigidbody;                      // Reference used to move the tank.
		protected float movementInputValue;                   // The current value of the movement input.
		protected float turnInputValue;                       // The current value of the turn input.
		private int roadLayerMask;                          // LayerMask for road
		private int borderLayerMask;                        // LayerMask for bord


		//Cash
		protected Transform _transform;
		protected CarBlueprint carStats;

		protected void Awake()
		{
			//cash
			_transform = GetComponent<Transform>();
			m_Rigidbody = GetComponent<Rigidbody>();
			carStats = GetComponent<CarBlueprint>();

			//get data from the CarBlueprint
			//Получение данных из CarBlueprint
			normalMovingSpeed = carStats.movingSpeed;
			stopSpeed = carStats.stopSpeed;
			accelerationSpeed = carStats.accelerationSpeed;
			turnSpeed = carStats.turnSpeed;
			turnTurtleDistance = carStats.turnTurtleDistance;
			toGroundDistance = carStats.toGroundDistance;
			toBorderDistance = carStats.toBorderDistance;

			movingSpeed = normalMovingSpeed;

			roadLayerMask = LayerMask.GetMask("Road");
			borderLayerMask = LayerMask.GetMask("Border");

			//find the first gameobject with tag "checkpoint"
			checkpointsHolder = GameObject.FindWithTag("Checkpoint").transform;

			checkpointsPosition = new Transform[checkpointsHolder.childCount];

			for (int i = 0; i < checkpointsHolder.childCount; i++)
			{
				checkpointsPosition[i] = checkpointsHolder.GetChild(i);
			}
			lastCheckpointPos = checkpointsPosition[0];
		}

		private void OnEnable()
		{
			// When the car is turned on, make sure it's not kinematic.
			m_Rigidbody.isKinematic = false;

			// Also reset the input values.
			movementInputValue = 0f;

			AccelerateCar();
		}

		protected void Update()
		{
			movementInputValue = Mathf.Clamp(movementInputValue, 0, 1);
			LeanOnBorder();

			lapsTime += Time.deltaTime;
		}

		//accelerate our car after stop/slowdown
		void AccelerateCar()
		{
			//after start moving increase our speed with a time
			movementInputValue += Time.deltaTime * accelerationSpeed;
		}

		//slowdown car
		void SlowdownCar()
		{
			//until the some speed (0.4) we slowdown the car
			if (movementInputValue > 0.4f)
				movementInputValue -= Time.deltaTime * stopSpeed;
			//after that moment set the speed to 0.1
			else
				movementInputValue = 0.1f;

		}

		protected bool OnGround()
		{
			return Physics.Raycast(_transform.position, -_transform.up, toGroundDistance, roadLayerMask);
		}

		//if car is turned turtle (lie on the side or roof)
		//если машина перевернулась (лежит на боку либо на крыше)
		protected bool OnTurnTurtle()
		{
			if (Physics.Raycast(_transform.position, _transform.up, turnTurtleDistance, roadLayerMask) ||
				Physics.Raycast(_transform.position, _transform.right, turnTurtleDistance, roadLayerMask) ||
				Physics.Raycast(_transform.position, -_transform.right, turnTurtleDistance, roadLayerMask))
			{
				Explosion();
				return true;
			}
			return false;
		}

		//if car is lean on boarder
		//если машина уперлась в стену
		protected bool LeanOnBorder()
		{
			RaycastHit hit;

			if (Physics.Raycast(_transform.position, _transform.forward, out hit, toBorderDistance, borderLayerMask))
			{
				SlowdownCar();
				return true;
			}

			AccelerateCar();
			return false;
		}

		//back to track if our car is off track
		//возврат на трек после вылета
		public void BackToTrack()
		{

			transform.position = lastCheckpointPos.position;
			transform.rotation = lastCheckpointPos.rotation;

			//return speed to startSpeed
			movementInputValue = 0.1f;

			AccelerateCar();
		}

		void Explosion()
		{
			if (explosionPrefab != null)
			{
				explosionPrefab.transform.position = transform.position;
				explosionPrefab.SetActive(true);
			}

			GameManager.Instance.CallReload(gameObject);
			gameObject.SetActive(false);
		}

		//increase player speed
		public void ChangeSpeedBonus(float speedUp, float effectTime, float backToNormalTime)
		{
			StartCoroutine(ChangeSpeedBonusCoroutine(speedUp, effectTime, backToNormalTime));
		}

		//increase player speed coroutine
		public IEnumerator ChangeSpeedBonusCoroutine(float speedUp, float effectTime, float backToNormalTime)
		{
			movingSpeed += speedUp;
			yield return new WaitForSeconds(effectTime);

			while (Mathf.Abs(movingSpeed - normalMovingSpeed) >= 0.1f)
			{
				movingSpeed -= Time.deltaTime * speedUp / backToNormalTime;
				yield return null;
			}

			movingSpeed = normalMovingSpeed;
		}

		private void OnTriggerEnter(Collider other)
		{
			//check for enter the checkpoint
			//проверка на вход в чекпоинт
			if (other.CompareTag("Checkpoint"))
			{
				lastCheckpointPos = other.transform;
			}

			//set point on the middle of the loop to check did the player complete a loop
			else if (other.CompareTag("CheckForFullLoop"))
			{
				fullLoop = true;
			}

			else if (other.CompareTag("GravityOff"))
			{
				m_Rigidbody.useGravity = false;
			}

			else if (other.CompareTag("GravityOn"))
			{
				m_Rigidbody.useGravity = true;
			}
		}

		private void OnTriggerExit(Collider other)
		{
			//check for enter the finish
			//проверка на вход в финиш
			if (other.CompareTag("Finish") && fullLoop)
			{
				print("FINISH!!!!!!!!!!!");
				//update score and UI
				carStats.UpdateScore();

				//check for winner of the game
				//проверка на то, победил ли игрок
				GameManager.Instance.CheckGameWinner();

				fullLoop = false;
			}

			else if (other.CompareTag("Border"))
			{
				Explosion();
			}

			//back car acceleration to standard acceleration

		}

	}
}