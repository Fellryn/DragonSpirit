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
    public class ProjectileFireball : MonoBehaviour
    {

        [SerializeField]
        private float capsuleHeight = 50f;
        public float capsuleRadius = 0.25f;
        private float fireballLifetime = 5f;
        public float ProjectileMoveSpeed { get; set; }
        public string HitTag { get; set; }
        public int ProjectileDamage { get; set; }
        public bool usingCustomRotation = false;
        public bool usingCustomPosition = false;

        public Transform cachedPlayerCamera;
        public Transform cachedUnitTransform;

        private string enemyTag = "Enemy";
        private string playerTag = "Player";
        private string projectileString = "Projectile";

        [SerializeField] GameObject explosionPrefab;
        [SerializeField] AudioClip explosionClip;
        private AudioSource soundEffectsAudioSource;


        GameObject colliderGameObject;
        CapsuleCollider cachedCollider;
        Rigidbody cachedRigidbody;

        GameObject cachedModel;

        private void OnEnable()
        {
            if (ProjectileMoveSpeed == 0)
            {
                ProjectileMoveSpeed = 20f;
            }

            if (ProjectileDamage == 0)
            {
                ProjectileDamage = 1;
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

        private void Start()
        {
            if (!usingCustomPosition) transform.position = cachedUnitTransform.position;
            if (!usingCustomRotation) transform.rotation = cachedUnitTransform.rotation;

            if (transform.CompareTag(enemyTag + projectileString))
            {
                ProjectileMoveSpeed *= -1;
            }

            Destroy(gameObject, fireballLifetime);

            Move();
 
        }


        private void FixedUpdate()
        {
            colliderGameObject.transform.LookAt(cachedPlayerCamera);
        }


        private void OnTriggerEnter(Collider other)
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

            }

            if (other.CompareTag("Background"))
            {
                Destroy(gameObject);
                PlayEffects(this.transform);
            }


        }

        private void Move()
        {
            cachedRigidbody.AddRelativeForce(transform.forward * ProjectileMoveSpeed, ForceMode.Impulse);
        }

        private void PlayEffects(Transform targetTransform)
        {
            var explosionEffect = Instantiate(explosionPrefab, targetTransform.position, Quaternion.identity);
            explosionEffect.hideFlags = HideFlags.HideInInspector;

            soundEffectsAudioSource = GameObject.FindWithTag("SoundEffects").GetComponent<AudioSource>();
            soundEffectsAudioSource.PlayOneShot(explosionClip);
        }


    }
}