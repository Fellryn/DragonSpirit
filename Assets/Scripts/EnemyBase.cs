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
		[Header("Base")]
		[SerializeField] float AttackChance = .5f;
		[SerializeField] string enemyProjectileTag = "EnemyProjectile";

		protected string EnemyName { get; set; }
		protected float Points { get; set; }
		protected float MoveSpeed { get; set; }
		protected float RotationSpeed { get; set; }
        public bool CanAttack { get; set; }


		public CinemachineSplineCart splineCart { get; set; }
		private GameTickSystem gameTickSystem;

		[HideInInspector]
		public Vector3 initialOffset;

		private Rigidbody cachedRigidbody;

		protected virtual void OnEnable()
        {

			cachedRigidbody = GetComponent<Rigidbody>();
			gameTickSystem = FindFirstObjectByType<GameTickSystem>();
			gameTickSystem.OnRandomTick.AddListener(Attack);
			CanAttack = false;
        }

		protected virtual void FixedUpdate()
        {
			Move();
        }


		public virtual void Move()
        {
			Vector3 moveTarget = Vector3.MoveTowards(cachedRigidbody.position, splineCart.transform.position + initialOffset, MoveSpeed * Time.deltaTime);
			Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, splineCart.transform.rotation, RotationSpeed * Time.deltaTime);
			cachedRigidbody.MovePosition(moveTarget);
			cachedRigidbody.MoveRotation(rotateTarget);
        }


		public virtual void Attack()
        {
			if (CanAttack)
            {
				if (Random.Range(0f, 1f) > AttackChance)
                {
					var newProjectile = new GameObject().AddComponent<ProjectileFireball>();
					newProjectile.transform.tag = enemyProjectileTag;
					newProjectile.cachedPlayerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
					newProjectile.cachedUnitTransform = transform;

					SetProjectileMoveSpeed(newProjectile);
				}
            }
        }

		protected virtual void OnDisable()
        {
			gameTickSystem.OnRandomTick.RemoveListener(Attack);
        }

		protected virtual void SetProjectileMoveSpeed(ProjectileFireball fireballScript)
        {
			fireballScript.ProjectileMoveSpeed = 0f;

		}

		protected virtual void Initialise(string enemyName, int points, float moveSpeed, float rotationSpeed)
        {
			EnemyName = enemyName;
			Points = points;
			MoveSpeed = moveSpeed;
			RotationSpeed = rotationSpeed;
        }

	}
}