using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Racing
{
	public class PlayerController : CarSystem
	{
		private void FixedUpdate()
		{
			PlayerMovement();
		}

		void PlayerMovement()
		{
			if (!OnTurnTurtle())
				Move();
			if (OnGround())
				Turn();
		}

		private void Move()
		{
			// Create a vector in the direction the tank is facing with a magnitude based on the input, speed and the time between frames.
			Vector3 movement = _transform.forward * movementInputValue * movingSpeed * Time.deltaTime * 0.2f;

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
	}
}
