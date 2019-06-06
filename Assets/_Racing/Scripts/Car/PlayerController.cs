using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racing
{
	public class PlayerController : CarSystem
	{
		private new void Update()
		{
			base.Update();

			//turn car by press keys on keyboard
			if (Input.GetKey(KeyCode.LeftArrow))
				TurnCar(-1);
			if (Input.GetKey(KeyCode.RightArrow))
				TurnCar(1);


			if (Input.GetKeyUp(KeyCode.LeftArrow))
				turnInputValue = 0;
			if (Input.GetKeyUp(KeyCode.RightArrow))
				turnInputValue = 0;
		}

		private void FixedUpdate()
		{
			PlayerMovement();
		}

		void PlayerMovement()
		{
			//if the car is not onturnturtle, we move
			if (!OnTurnTurtle())
				Move();
			//if the car on the ground. we can turn
			if (OnGround())
				Turn();
		}

		private void Move()
		{
			// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
			Vector3 movement = transform.forward * movementInputValue * movingSpeed * Time.deltaTime * 0.2f;

			// Apply this movement to the rigidbody's position.
			m_Rigidbody.MovePosition(m_Rigidbody.position + movement);
		}

		private void Turn()
		{
			// Determine the number of degrees to be turned based on the input, speed and time between frames.
			float turn = turnInputValue * turnSpeed * Time.deltaTime;

			// Make this into a rotation in the y axis.
			Quaternion turnRotation = Quaternion.Euler(0f, turn, 0f);

			// Apply this rotation to the rigidbody's rotation.
			m_Rigidbody.MoveRotation(m_Rigidbody.rotation * turnRotation);
		}

		//method to change turnInputValue from -1 (left) to 1 (right) to 0 (forward)
		public void TurnCar(int side)
		{
			if (movementInputValue != 0)
			{
				turnInputValue = side;
			}
		}
	}
}
