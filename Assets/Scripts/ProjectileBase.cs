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
        [Header("Collider")]
        [SerializeField]
        protected float capsuleHeight = 100f;
        public float capsuleRadius = 0.25f;
        [Header("Projectile Options")]
        [SerializeField]
        protected float ProjectileLifetime = 5f;
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

        [Header("References and Tags")]
        public Transform cachedPlayerCamera;
        public Transform cachedUnitTransform;
        private Transform cachedPlayerTransform;
        protected bool playerOneProjectile = true;

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
        [SerializeField]
        Light projectileLight;
        [SerializeField]
        Color playerProjectileLight;

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
                if (Camera.main.WorldToViewportPoint(transform.position).x > 1.01f ||
                    Camera.main.WorldToViewportPoint(transform.position).x < -0.01f ||
                    Camera.main.WorldToViewportPoint(transform.position).y > 1.01f ||
                    Camera.main.WorldToViewportPoint(transform.position).x < 0.01f)
                {
                    Destroy(gameObject);
                    return;
                }
                else
                {

                    if (other.CompareTag(enemyTag))
                    {
                        if (other.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
                        {
                            enemyBase.TakeDamage(ProjectileDamage, playerOneProjectile);
                        }
                        else
                        {
                            other.GetComponentInParent<EnemyBase>().TakeDamage(ProjectileDamage, playerOneProjectile);
                        }


                        PlayEffects(transform);
                        Destroy(gameObject);
                    }

                    if (other.CompareTag(powerupEggTag))
                    {
                        if (other.TryGetComponent(out PowerupEgg powerUpEgg))
                        {
                            powerUpEgg.BreakEgg();
                        }
                        Destroy(gameObject);
                        PlayEffects(transform);
                    }
                }
            }
        }

        public virtual void Initalise(Transform playerTransform, Transform firingUnitTransform, Transform mainCameraTransform, int projectileDamage = 1, float projectileMoveSpeed = 12f, bool useRandomProjectileSpeed = true, bool isEnemy = false, bool playerOneProjectile = true, bool aimAtPlayer = false, float customRotationY = 0f)
        {
            cachedPlayerTransform = playerTransform;
            cachedUnitTransform = firingUnitTransform;
            transform.name = $"{cachedUnitTransform.name} Projectile";
            cachedPlayerCamera = mainCameraTransform;
            ProjectileDamage = projectileDamage;
            ProjectileMoveSpeed = projectileMoveSpeed;
            this.playerOneProjectile = playerOneProjectile;

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

            if (customRotationY != 0f)
            {
                transform.rotation = Quaternion.Euler(cachedUnitTransform.rotation.eulerAngles + new Vector3(0, customRotationY, 0));
            }
            else
            {
                transform.rotation = cachedUnitTransform.rotation;
            }

            if (!usingCustomPosition) transform.position = cachedUnitTransform.position;

            if (transform.CompareTag(enemyTag + projectileString))
            {
                ProjectileMoveSpeed *= -1;
            }

            // temporary until rotation fixed
            transform.Translate(0f, 0.2f, 0f);

            Destroy(gameObject, ProjectileLifetime);


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
                mainContinue.startRotation = (transform.eulerAngles.y + 180f) * Mathf.Deg2Rad;
                mainStart.startRotation = (transform.eulerAngles.y + 180f) * Mathf.Deg2Rad;

                particleSystemStart.GetComponent<ParticleSystemRenderer>().material = playerMaterial;
                particleSystemContinue.GetComponent<ParticleSystemRenderer>().material = playerMaterial;

                projectileLight.color = playerProjectileLight;
            }
        }


        protected virtual void PlayEffects(Transform targetTransform)
        {
            var explosionEffect = Instantiate(explosionPrefab, targetTransform.position, Quaternion.identity);
            explosionEffect.hideFlags = HideFlags.HideInInspector;

            soundEffectsAudioSource = GameObject.FindWithTag("SoundEffects").GetComponent<AudioSource>();
            soundEffectsAudioSource.PlayOneShot(explosionClip);
        }


    }
}