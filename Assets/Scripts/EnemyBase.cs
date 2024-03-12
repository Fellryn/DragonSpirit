using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to make an enemy base class in Unity
	/// </summary>

	public abstract class EnemyBase : MonoBehaviour
	{
		public string enemyName = "Enemy";
		public int points = 50;
		public float moveSpeed = 30f;
		public float rotationSpeed = 15f;
		public CinemachineSplineCart splineCart;
		[SerializeField] GameTickSystem gameTickSystem;


		
		public Vector3 initialOffset;

		private Rigidbody cachedRigidbody;

		protected virtual void OnEnable()
        {

			cachedRigidbody = GetComponent<Rigidbody>();
			gameTickSystem = FindFirstObjectByType<GameTickSystem>();
			gameTickSystem.OnEveryTickInterval.AddListener(Move);
        }

		protected virtual void FixedUpdate()
        {
			Move();
        }


		public virtual void Move()
        {
			Vector3 moveTarget = Vector3.MoveTowards(cachedRigidbody.position, splineCart.transform.position + initialOffset, moveSpeed * Time.deltaTime);
			Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, splineCart.transform.rotation, rotationSpeed * Time.deltaTime);
			cachedRigidbody.MovePosition(moveTarget);
			cachedRigidbody.MoveRotation(rotateTarget);
        }


		public virtual void Attack()
        {

        }

		protected virtual void OnDisable()
        {
			gameTickSystem.OnEveryTickInterval.RemoveListener(Move);
        }
    }
}