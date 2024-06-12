using UnityEngine;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to create an enemy base super class in Unity
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour
    {
        protected GameTickSystem gameTickSystem;
        [SerializeField] protected SoundHandler soundHandler;
        [SerializeField] float deathSoundChanceToPlay = 0.7f;

        [Header("Base")]
        [SerializeField]
        string enemyName = "Enemy";
        public int health { get; private set; }
        [SerializeField]
        private int startingHealth = 1;
        public int score = 1;
        public Transform cachedModel;
        public Transform cachedProjectileTrackingPoint;
        protected Collider cachedCollider;
        public bool canAttack = false;
        public bool isInvincible = true;

        [Header("Death")]
        public bool deathBegun = false;
        [SerializeField]
        bool ragdollOnDeath = false;
        [SerializeField]
        float timeBeforeDestroyObject = 5f;
        [SerializeField]
        Transform ragdollObject;
        [SerializeField]
        public bool lastHitByPlayerOne = true;

        [Space(10)]
        [SerializeField]
        private bool useExplosiveForceEffectOnDeath = false;
        [SerializeField]
        private float explosionPower = 10f;
        [SerializeField]
        private float explosionRadius = 5f;

        public delegate void OnKill(int scoreToAdd, bool playerOneScore);
        public static event OnKill onKill;

        protected Rigidbody cachedRigidbody;


        // Get references and cached components, set name and starting health
        protected virtual void OnEnable()
        {
            gameTickSystem = FindFirstObjectByType<GameTickSystem>();
            cachedRigidbody = GetComponent<Rigidbody>();

            if (transform.TryGetComponent(out Collider collider))
            {
                cachedCollider = collider;
            } else
            {
                cachedCollider = transform.GetComponentInChildren<Collider>();
            }

            transform.name = enemyName;

            SetHealth(startingHealth);
        }

        // Take damage (example of encapsulation - private (set) health property is only modified with this or set health)
        public virtual void TakeDamage(int damageAmount, bool hitByPlayerOne)
        {
            if (!isInvincible)
            {
                lastHitByPlayerOne = hitByPlayerOne;

                health -= damageAmount;
                HealthCheck();
            }
        }

        // Set health (example of encapsulation - health must be modified here)
        protected void SetHealth(int amount)
        {
            health = amount;
        }

        // Check health when takes damage
        protected virtual void HealthCheck()
        {
            if (health <= 0)
            {
                BeginDeath();
            }
        }

        // Begin death function (example of abstraction - inheritors and other objects can call this and not worry about the how)
        // Called when health hits 0, changes various physics settings and begins/stop animations
        public virtual void BeginDeath()
        {
            deathBegun = true;
            onKill?.Invoke(score, lastHitByPlayerOne);

            transform.tag = "Untagged";
            cachedRigidbody.useGravity = true;
            cachedRigidbody.isKinematic = false;
            cachedRigidbody.includeLayers = 1 << LayerMask.NameToLayer("NoCollide");
            if (useExplosiveForceEffectOnDeath)
            {
                cachedRigidbody.AddExplosionForce(explosionPower, (UnityEngine.Random.insideUnitSphere * 2f) + transform.position, explosionRadius, 0f, ForceMode.Impulse);
            }

            cachedCollider.isTrigger = false;

            if (ragdollOnDeath)
            {
                cachedModel.gameObject.SetActive(false);
                ragdollObject.gameObject.SetActive(true);
                cachedCollider.isTrigger = true;
            }

            if (TryGetComponent<EnemyShaderController>(out EnemyShaderController enemyShaderController))
            {
                enemyShaderController.BeginDissolveAnimation();
            }

            PlaySound(0, deathSoundChanceToPlay);

            if (TryGetComponent<EnemyAnimation>(out EnemyAnimation enemyAnimation))
            {
                enemyAnimation.OnDeath();
            }



            Destroy(gameObject, timeBeforeDestroyObject);

            enabled = false;
        }

        protected void PlaySound(int index, float volume = 1f, float chanceToPlay = 1f)
        {
            if (soundHandler != null)
            {
                soundHandler.PlaySound(index, volume, chanceToPlay);
            }
        }

    }
}