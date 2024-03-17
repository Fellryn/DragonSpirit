using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to create an enemy base super class in Unity
	/// </summary>
	public abstract class EnemyBase : MonoBehaviour 
	{
		protected GameTickSystem gameTickSystem;

		[Header("Base")]
		[SerializeField] string enemyName = "Enemy";
		[SerializeField] int health = 1;
		[SerializeField] int score = 1;
		public Transform cachedModel;

		public bool deathBegun = false;

		public delegate void OnKill(int scoreToAdd);
		public static event OnKill onKill;

		protected Rigidbody cachedRigidbody;

		protected virtual void OnEnable()
        {
			gameTickSystem = FindFirstObjectByType<GameTickSystem>();
			cachedRigidbody = GetComponent<Rigidbody>();
			transform.name = enemyName;
		}


		public void TakeDamage(int damageAmount)
        {
			health -= damageAmount;
			HealthCheck();
        }


		private void HealthCheck()
        {
			if (health <= 0)
            {
				BeginDeath();
            }
        }


		public virtual void BeginDeath()
        {
			deathBegun = true;
			onKill?.Invoke(score);

			transform.tag = "Untagged";
			cachedRigidbody.useGravity = true;
			cachedRigidbody.AddExplosionForce(150f, transform.position, 1f);
			cachedRigidbody.isKinematic = false;

			Destroy(gameObject, 3f);

			enabled = false;
		}
	}
}