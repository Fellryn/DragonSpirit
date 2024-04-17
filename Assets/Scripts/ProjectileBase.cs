using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to create a fireball projectile attack in Unity
    /// </summary>

    [RequireComponent(typeof(Rigidbody))]
    public abstract class ProjectileBase : MonoBehaviour
    {

        [SerializeField]
        protected float capsuleHeight = 100f;
        public float capsuleRadius = 0.25f;
        protected float fireballLifetime = 5f;
        public float maxHeight = 7;
        [SerializeField]
        protected float defaultMoveSpeed = 20f;
        [SerializeField]
        protected float moveSpeedVariation = 3f;
        protected int defaultDamage = 1;
        public float ProjectileMoveSpeed { get; set; }
        public string HitTag { get; set; }
        public int ProjectileDamage { get; set; }
        public bool usingCustomRotation = false;
        public bool usingCustomPosition = false;

        public Transform cachedPlayerCamera;
        public Transform cachedUnitTransform;
        private Transform cachedPlayerTransform;

        protected string enemyTag = "Enemy";
        protected string playerTag = "Player";
        protected string projectileString = "Projectile";
        protected string powerupEggTag = "PowerUpEgg";

        [SerializeField]
        protected GameObject explosionPrefab;
        [SerializeField]
        protected AudioClip explosionClip;
        protected AudioSource soundEffectsAudioSource;


        protected GameObject colliderGameObject;
        protected CapsuleCollider cachedCollider;
        protected Rigidbody cachedRigidbody;

        protected GameObject cachedModel;

        [SerializeField]
        ParticleSystem particleSystemStart;
        [SerializeField]
        ParticleSystem particleSystemContinue;
        [SerializeField]
        Material playerMaterial;

        protected virtual void OnEnable()
        {
            if (ProjectileMoveSpeed == 0)
            {
                ProjectileMoveSpeed = defaultMoveSpeed;
            }

            if (ProjectileDamage == 0)
            {
                ProjectileDamage = defaultDamage;
            }



            cachedRigidbody = GetComponent<Rigidbody>();
            cachedRigidbody.useGravity = false;
            cachedRigidbody.drag = 0f;


            colliderGameObject = new GameObject("Collider");
            colliderGameObject.transform.tag = transform.tag;
            colliderGameObject.transform.SetParent(transform);
            colliderGameObject.layer = LayerMask.NameToLayer("NoCollide");
            cachedCollider = colliderGameObject.AddComponent<CapsuleCollider>();
            cachedCollider.height = capsuleHeight;
            cachedCollider.radius = capsuleRadius;
            cachedCollider.direction = 2;
            cachedCollider.isTrigger = true;
            cachedCollider.excludeLayers = 1 << LayerMask.NameToLayer("NoCollide");


            //cachedModel = Instantiate(FindAnyObjectByType<PrefabReferencesLink>().prefabReferences.fireballPrefab, transform.position, Quaternion.identity, transform);
        }

        protected virtual void Start()
        {
            if (!usingCustomPosition) transform.position = cachedUnitTransform.position;
            if (!usingCustomRotation) transform.rotation = cachedUnitTransform.rotation;

            if (transform.CompareTag(enemyTag + projectileString))
            {
                ProjectileMoveSpeed *= -1;
            }

            // temporary until rotation fixed
            transform.Translate(0f, 0.2f, 0f);

            Destroy(gameObject, fireballLifetime);

            Move();

            RotateSprite();
        }


        protected virtual void FixedUpdate()
        {
            colliderGameObject.transform.LookAt(cachedPlayerCamera);


            if (transform.position.y > maxHeight)
            {
                Destroy(gameObject);
            }

        }


        protected virtual void OnTriggerEnter(Collider other)
        {
            if (transform.CompareTag(enemyTag + projectileString))
            {
                if (other.CompareTag(playerTag))
                {
                    if (other.TryGetComponent<PlayerStats>(out PlayerStats playerStats))
                    {
                        playerStats.PlayerTakeDamage(ProjectileDamage);
                    }
                    else
                    {
                        //Destroy(other.gameObject);
                    }

                    Destroy(gameObject);
                    PlayEffects(other.transform);
                }

            }


            if (transform.CompareTag(playerTag + projectileString))
            {
                if (other.CompareTag(enemyTag))
                {
                    if (other.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
                    {
                        enemyBase.TakeDamage(ProjectileDamage);
                    }
                    else
                    {
                        //Destroy(other.gameObject);
                    }

                    Destroy(gameObject);
                    PlayEffects(other.transform);
                }

                if (other.CompareTag(powerupEggTag))
                {
                    Destroy(other.gameObject);
                }

            }

            if (other.CompareTag("Background"))
            {
                Destroy(gameObject);
                PlayEffects(this.transform);
            }


        }

        public virtual void Initalise(Transform playerTransform, Transform firingUnitTransform, Transform mainCameraTransform, int projectileDamage = 1, float projectileMoveSpeed = 12f, bool useRandomProjectileSpeed = true, bool isEnemy = false, bool aimAtPlayer = false)
        {
            cachedPlayerTransform = playerTransform;
            cachedUnitTransform = firingUnitTransform;
            transform.name = $"{cachedUnitTransform.name} Projectile";
            cachedPlayerCamera = mainCameraTransform;
            ProjectileDamage = projectileDamage;
            ProjectileMoveSpeed = projectileMoveSpeed;

            if (useRandomProjectileSpeed) ProjectileMoveSpeed = ProjectileMoveSpeed + Random.Range(-moveSpeedVariation, moveSpeedVariation);

            if (isEnemy)
            {
                transform.tag = enemyTag + projectileString;
            }
            else
            {
                transform.tag = playerTag + projectileString;
            }

            if (aimAtPlayer)
            {
                //usingCustomRotation = true;
                //transform.rotation = Quaternion.LookRotation(cachedRigidbody.position - cachedPlayerTransform.position);
                transform.LookAt(cachedPlayerTransform);
            }

        }

        protected virtual void Move()
        {
            cachedRigidbody.AddRelativeForce(transform.forward * ProjectileMoveSpeed, ForceMode.Impulse);
        }


        protected virtual void RotateSprite()
        {

            ParticleSystem.MainModule mainContinue = particleSystemContinue.main;
            mainContinue.startRotation = transform.eulerAngles.y * Mathf.Deg2Rad;

            ParticleSystem.MainModule mainStart = particleSystemStart.main;
            mainStart.startRotation = transform.eulerAngles.y * Mathf.Deg2Rad;

            if (transform.CompareTag(playerTag + projectileString))
            {
                mainContinue.startRotation = 180f * Mathf.Deg2Rad;
                mainStart.startRotation = 180f * Mathf.Deg2Rad;

                particleSystemStart.GetComponent<ParticleSystemRenderer>().material = playerMaterial;
                particleSystemContinue.GetComponent<ParticleSystemRenderer>().material = playerMaterial;
            }
        }


        protected virtual void PlayEffects(Transform targetTransform)
        {
            var explosionEffect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosionEffect.hideFlags = HideFlags.HideInInspector;

            soundEffectsAudioSource = GameObject.FindWithTag("SoundEffects").GetComponent<AudioSource>();
            soundEffectsAudioSource.PlayOneShot(explosionClip);
        }


    }
}