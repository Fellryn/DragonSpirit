using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to make a subclass for a ranged enemy in Unity
	/// </summary>
	public abstract class EnemyRangedAttack : EnemyBase
	{
		[Header("Attack")]
		[SerializeField] int damage = 1;
		[SerializeField] float attackChance = 0.5f;
		[SerializeField] float projectileMoveSpeedMin = 10f;
		[SerializeField] float projectileMoveSpeedMax = 15f;
		[SerializeField] string enemyProjectileTag = "EnemyProjectile";
		[SerializeField] bool aimAtPlayer = false;

		protected Transform cachedPlayerTransform;

		public bool canAttack = false;

        protected override void OnEnable()
        {
			base.OnEnable();
			gameTickSystem.OnRandomTick.AddListener(Attack);
			cachedPlayerTransform = FindAnyObjectByType<PlayerMovement>().GetComponent<Transform>();
		}

		protected virtual void OnDisable()
		{
			gameTickSystem.OnRandomTick.RemoveListener(Attack);
		}

		private void Attack()
        {
			if (canAttack && Random.Range(0f,1f) < attackChance)
            {
				var newProjectile = new GameObject().AddComponent<ProjectileFireball>();
				newProjectile.transform.name = $"{transform.name} Projectile";
				newProjectile.transform.tag = enemyProjectileTag;
				newProjectile.cachedPlayerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
				newProjectile.ProjectileDamage = damage;
				newProjectile.cachedUnitTransform = transform;

				if (aimAtPlayer)
                {
					newProjectile.usingCustomRotation = true;
					newProjectile.transform.rotation = Quaternion.LookRotation(cachedRigidbody.position - cachedPlayerTransform.position);

				}

				SetProjectileMoveSpeed(newProjectile);
			}
        }

		private void SetProjectileMoveSpeed(ProjectileFireball fireballScript)
        {
			fireballScript.ProjectileMoveSpeed = Random.Range(projectileMoveSpeedMin, projectileMoveSpeedMax);
		}

	}
}