using UnityEngine;

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
        private EnemyAnimation enemyAnimationScript;
        protected MultiplayerManager multiplayerManager;

        // Example of polymorphism (Changing the function while still keeping the same name and previous behaviour)
        // On enable adds listener for shooting randomly, grabs link to prefab list
        // and caches projectile parent so projectiles go into their own gameobject
        protected override void OnEnable()
        {
            base.OnEnable();

            gameTickSystem.OnRandomTick.AddListener(Attack);
            gameTickSystem.OnTickWhole.AddListener(DoPlayerDistanceCheck);

            cachedPlayerTransform = FindAnyObjectByType<PlayerMovement>().GetComponent<Transform>();
            prefabLink = FindAnyObjectByType<PrefabReferencesLink>();
            projectileHolder = GameObject.FindWithTag("ProjectileHolder").GetComponent<Transform>();
            mainCameraTransform = GameObject.FindWithTag("MainCamera").GetComponent<Transform>();
            multiplayerManager = FindAnyObjectByType<MultiplayerManager>();
            if (TryGetComponent(out EnemyAnimation enemyAnimation)) enemyAnimationScript = enemyAnimation;
            if (!useCustomProjectileOrigin) projectileOrigin = transform;
        }

        protected virtual void OnDisable()
        {
            gameTickSystem.OnRandomTick.RemoveListener(Attack);
            gameTickSystem.OnTickWhole.RemoveListener(DoPlayerDistanceCheck);
        }

        // Called from animation event that is on child object with animator component
        // Disables attacking until attack animation is complete
        protected virtual void AttackCompleted()
        {
            if (enemyAnimationScript != null)
            {
                enemyAnimationScript.AttackCompleted();
                canAttack = true;
            }
        }

        // Attack on random tick from tick system, further random chance to attack or not
        protected virtual void Attack()
        {
            if (canAttack && Random.Range(0f, 1f) < attackChance)
            {
                canAttack = false;

                if (enemyAnimationScript != null)
                {
                    enemyAnimationScript.AttackBegun();
                }

                LaunchFireball();

                PlaySound(1, 0.5f, 0.7f);

            }
        }

        // Example of abstraction (hiding all the details of launching a fireball, inheriting classes can just call LaunchFireball())

        protected void LaunchFireball()
        {


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

        public void DoPlayerDistanceCheck()
        {
            if (multiplayerManager.playersList.Count > 1)
            {
                if (Vector3.Distance(multiplayerManager.playersList[0].position, transform.position) < Vector3.Distance(multiplayerManager.playersList[1].position, transform.position))
                {
                    cachedPlayerTransform = multiplayerManager.playersList[0];
                } else
                {
                    cachedPlayerTransform = multiplayerManager.playersList[1];
                }
            }
        }
    }
}