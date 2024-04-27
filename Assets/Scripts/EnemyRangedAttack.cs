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
        [SerializeField] protected int damage = 1;
        [SerializeField] protected float attackChance = 0.5f;
        [SerializeField] protected float projectileMoveSpeed = 12f;
        [SerializeField] protected bool aimAtPlayer = false;
        [SerializeField] protected bool useCustomProjectileOrigin = false;
        public Transform projectileOrigin;

        protected Transform cachedPlayerTransform;

        protected PrefabReferencesLink prefabLink;
        protected Transform projectileHolder;
        protected Transform mainCameraTransform;


        // On enable adds listener for shooting randomly, grabs link to prefab list
        // and caches projectile parent so projectiles go into their own gameobject.
        protected override void OnEnable()
        {
            base.OnEnable();
            gameTickSystem.OnRandomTick.AddListener(Attack);
            cachedPlayerTransform = FindAnyObjectByType<PlayerMovement>().GetComponent<Transform>();
            prefabLink = FindAnyObjectByType<PrefabReferencesLink>();
            projectileHolder = GameObject.FindWithTag("ProjectileHolder").GetComponent<Transform>();
            mainCameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
            if (!useCustomProjectileOrigin) projectileOrigin = transform;
        }

        protected virtual void OnDisable()
        {
            gameTickSystem.OnRandomTick.RemoveListener(Attack);
        }

        // Called from animation event passer that is on animator child
        protected virtual void AttackCompleted()
        {
            if (TryGetComponent(out EnemyAnimation enemyAnimationScript))
            {
                enemyAnimationScript.AttackCompleted();
                canAttack = true;
            }
        }



        protected virtual void Attack()
        {
            // When tick occurs, further random chance to shoot or not shoot
            if (canAttack && Random.Range(0f, 1f) < attackChance)
            {
                canAttack = false;

                if (TryGetComponent(out EnemyAnimation enemyAnimationScript))
                {
                    enemyAnimationScript.AttackBegun();
                }

                // Instantiate the fireball
                var newProjectile = Instantiate(prefabLink.prefabReferences.fireballPrefab, projectileHolder);

                // Try get projectile base and initialise
                if (newProjectile.TryGetComponent(out ProjectileBase projectileBase))
                {
                    projectileBase.Initalise(
                        playerTransform: cachedPlayerTransform,
                        firingUnitTransform: projectileOrigin,
                        mainCameraTransform: mainCameraTransform,
                        projectileDamage: damage,
                        isEnemy: true,
                        projectileMoveSpeed: projectileMoveSpeed,
                        aimAtPlayer: aimAtPlayer);
                }

            }
        }
    }
}