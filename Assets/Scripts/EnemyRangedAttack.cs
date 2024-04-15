using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to make a child script for a ranged enemy in Unity
	/// This script is part of a series that make up the enemy objects and the hierarchy is:
	/// EnemyBase -> EnemyRangedAttack -> EnemyMobile/EnemyStatic -> Enemy(Name)
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

		protected PrefabReferencesLink prefabLink;
		protected Transform projectileHolder;


		// On enable adds listener for shooting randomly, grabs link to prefab list
		// and caches projectile parent so projectiles go into their own gameobject.
        protected override void OnEnable()
        {
			base.OnEnable();
			gameTickSystem.OnRandomTick.AddListener(Attack);
			cachedPlayerTransform = FindAnyObjectByType<PlayerMovement>().GetComponent<Transform>();
			prefabLink = FindAnyObjectByType<PrefabReferencesLink>();
			projectileHolder = GameObject.FindWithTag("ProjectileHolder").GetComponent<Transform>();
		}

		protected virtual void OnDisable()
		{
			gameTickSystem.OnRandomTick.RemoveListener(Attack);
		}

		protected virtual void AttackCompleted()
        {
			if (TryGetComponent<EnemyAnimation>(out EnemyAnimation enemyAnimationScript))
			{
				enemyAnimationScript.AttackCompleted();
			}
		}



		private void Attack()
        {
			// When tick occurs, further random chance to shoot or not shoot
			if (canAttack && Random.Range(0f,1f) < attackChance)
            {
				if (TryGetComponent<EnemyAnimation>(out EnemyAnimation enemyAnimationScript)){
					enemyAnimationScript.AttackBegun();
                }

				// Instantiate the fireball
				var newProjectile = Instantiate(prefabLink.prefabReferences.fireballPrefab, projectileHolder);

				// This is where my question lays. If I change what is instantiated, I then have to change the related script
				// GetComponent<ProjectileFireball> to whatever a different projectile is called. So when I cache the script:
				ProjectileFireball cachedProjectileScript = newProjectile.GetComponent<ProjectileFireball>();

				// The local variable still has to be the type of <ProjectileFireball>. If we had another projectile like 
				// <ProjectileGiganticFireball> then the rest of the script wouldn't work.
				// Ideally I'd like whatever is instantiated to be able to use the rest of the script.
				// Maybe there is another way or pattern to get a reference to the script? Would it mean making a parent/child class 
				// and just having the script access a <ProjectileBase> class? 
				// Or just make multiple Methods for different attack types and copy this section with their different classes?
				// Let me know if you need more details, else I might talk to you on the Wednesday about it


				newProjectile.transform.name = $"{transform.name} Projectile";
				newProjectile.transform.tag = enemyProjectileTag;

				cachedProjectileScript.cachedPlayerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
				cachedProjectileScript.ProjectileDamage = damage;
				cachedProjectileScript.cachedUnitTransform = transform;


				// Change rotation of projectile to face player if enemy doesn't just shoot "forward"
				if (aimAtPlayer)
                {
					cachedProjectileScript.usingCustomRotation = true;
					newProjectile.transform.rotation = Quaternion.LookRotation(cachedRigidbody.position - cachedPlayerTransform.position);

				}

				SetProjectileMoveSpeed(cachedProjectileScript);
			}
        }


		// Set speed of projectile (leave 0 in inspector if want default speed)
		private void SetProjectileMoveSpeed(ProjectileFireball fireballScript)
        {
			fireballScript.ProjectileMoveSpeed = Random.Range(projectileMoveSpeedMin, projectileMoveSpeedMax);
		}

	}
}