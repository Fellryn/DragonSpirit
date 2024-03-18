using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to make a static enemy that is a child to the enemy superscripts in Unity
	/// </summary>
	public abstract class EnemyStatic : EnemyRangedAttack 
	{
		[Header("Movement")]
		[SerializeField] float rotationSpeed = 30f;
		[SerializeField] Camera mainCamera;
		[SerializeField] bool canRotate = true;

		protected override void OnEnable()
		{
			base.OnEnable();
			gameTickSystem.OnEveryHalfTick.AddListener(DoChecks);
			cachedPlayerTransform = FindAnyObjectByType<PlayerMovement>().GetComponent<Transform>();
			mainCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
			gameTickSystem.OnEveryHalfTick.RemoveListener(DoChecks);
		}

		private void DoChecks()
        {
			IsOnscreenCheck();
        }

		protected virtual void FixedUpdate()
        {
			RotateTowardPlayer();
		}

		private void RotateTowardPlayer()
		{
			if (canAttack && canRotate)
			{
				Vector3 rigidbody = new Vector3(cachedRigidbody.position.x, 0, cachedRigidbody.position.z);
				Vector3 player = new Vector3(cachedPlayerTransform.position.x, 0, cachedPlayerTransform.position.z);
				Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, Quaternion.LookRotation(rigidbody - player), rotationSpeed * Time.deltaTime);
				cachedRigidbody.MoveRotation(rotateTarget);
			}
		}


		
		private void IsOnscreenCheck()
        {
			if (mainCamera.WorldToViewportPoint(transform.position).y < 1.2f)
            {
				canAttack = true;
            }

			if (mainCamera.WorldToViewportPoint(transform.position).y < -0.2f)
            {
				canAttack = false;
				Destroy(gameObject);
            }
		}
	}
}