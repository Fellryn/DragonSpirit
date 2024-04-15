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
    public class ProjectileTrackingFireball : MonoBehaviour
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

        private GameObject[] enemyHolders;
        public Transform randomEnemyToFollow;
        private GameTickSystem gameTickSystem;

        private string enemyTag = "Enemy";
        //private string playerTag = "Player";
        private string projectileString = "Projectile";

        [SerializeField] GameObject explosionPrefab;
        [SerializeField] AudioClip explosionClip;
        private AudioSource soundEffectsAudioSource;


        GameObject colliderGameObject;
        CapsuleCollider cachedCollider;
        Rigidbody cachedRigidbody;

        private int rechecksAllowed = 5;
        private int rechecksDone = 0;



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

            //gameTickSystem = FindAnyObjectByType<GameTickSystem>().GetComponent<GameTickSystem>();
            //gameTickSystem.OnEveryHalfTick.AddListener(DoChecks);
            //cachedModel = Instantiate(FindAnyObjectByType<PrefabReferencesLink>().prefabReferences.fireballPrefab, transform.position, Quaternion.identity, transform);
        }

        private void OnDestroy()
        {
            //gameTickSystem.OnEveryHalfTick.RemoveListener(DoChecks);
        }

        private void Start()
        {

            enemyHolders = GameObject.FindGameObjectsWithTag("EnemyHolder");

            if (!usingCustomPosition) transform.position = cachedUnitTransform.position;
            if (!usingCustomRotation) transform.rotation = cachedUnitTransform.rotation;

            if (transform.CompareTag(enemyTag + projectileString))
            {
                ProjectileMoveSpeed *= -1;
            }

            StartMove();

            AimAtRandomEnemy();

            Destroy(gameObject, fireballLifetime);

            //Move();

        }


        private void FixedUpdate()
        {
            colliderGameObject.transform.LookAt(cachedPlayerCamera);
            CheckIfTargetDead();
            Move();
        }


        private void OnTriggerEnter(Collider other)
        {
            //if (transform.CompareTag(playerTag + projectileString))
            //{
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

                PlayEffects(other.transform);
                Destroy(gameObject);
            }

            //}

            if (other.CompareTag("Background"))
            {
                PlayEffects(this.transform);
                Destroy(gameObject);
            }


        }

        private void AimAtRandomEnemy()
        {
            int randomIndex = Random.Range(0, enemyHolders.Length);


            for (int i = 0; i < enemyHolders[randomIndex].transform.childCount; i++)
            { 
                if (FindAnyObjectByType<EnemyBoss>().cachedModel.GetComponent<Renderer>().isVisible)
                {
                    randomEnemyToFollow = FindAnyObjectByType<EnemyBoss>().cachedModel.GetComponent<Transform>();
                    break;
                }

                if (enemyHolders[randomIndex].transform.childCount == 0)
                {
                    AimAtRandomEnemy();
                    break;
                }


                if (Random.Range(0f, 1f) < 0.25f)
                {
                    randomEnemyToFollow = enemyHolders[randomIndex].transform.GetChild(i).GetComponent<Transform>();
                    break;
                }


                if (i == enemyHolders[randomIndex].transform.childCount)
                {
                    randomEnemyToFollow = enemyHolders[randomIndex].transform.GetChild(i).GetComponent<Transform>();
                    break;
                }
            }


            if (rechecksDone >= rechecksAllowed)
            {
                return;
            }


            if (randomEnemyToFollow == null)
            {
                AimAtRandomEnemy();
                rechecksAllowed++;
            } else
            {
                if (!randomEnemyToFollow.GetComponent<EnemyBase>().cachedModel.GetComponent<Renderer>().isVisible) AimAtRandomEnemy();
                rechecksAllowed++;
            }

            //if (randomEnemyToFollow != null)
            //{
            //    usingCustomRotation = true;
            //}
        }

        //private void DoChecks()
        //{
        //    CheckIfTargetDead();
        //}

        private void CheckIfTargetDead()
        {
            if (randomEnemyToFollow == null)
            {              
                AimAtRandomEnemy();
            } else if (randomEnemyToFollow.TryGetComponent<EnemyBase>(out EnemyBase enemyBase))
            {
                if (enemyBase.deathBegun)
                {
                    AimAtRandomEnemy();
                }
            }
        }

        private void StartMove()
        {
            cachedRigidbody.AddRelativeForce(transform.forward * ProjectileMoveSpeed, ForceMode.Impulse);
        }

        private void Move()
        {
            if (randomEnemyToFollow != null)
            {
                cachedRigidbody.MoveRotation(Quaternion.LookRotation(transform.position - randomEnemyToFollow.position));
                //transform.LookAt(randomEnemyToFollow.position);
                //Quaternion moveTarget = Quaternion.SetLookRotation(transform.position - randomEnemyToFollow.position);
                //cachedRigidbody.MoveRotation(moveTarget);
                //transform.rotation = Quaternion.LookRotation(cachedRigidbody.position - randomEnemyToFollow.position);

                Vector3 moveTarget = Vector3.MoveTowards(cachedRigidbody.position, randomEnemyToFollow.position, ProjectileMoveSpeed * Time.deltaTime);
                cachedRigidbody.MovePosition(moveTarget);
            } else
            {
                cachedRigidbody.AddForce(Vector3.forward * ProjectileMoveSpeed);
            }
            



            //Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, Quaternion.LookRotation(cachedRigidbody.position - cachedPlayerTransform.position), rotationSpeed * Time.deltaTime);



            //cachedRigidbody.AddRelativeForce(transform.forward * ProjectileMoveSpeed * 0.02f, ForceMode.Impulse);
            // cachedRigidbody.maxLinearVelocity = 10f;

            // Debug.Log(cachedRigidbody.velocity);
        }

        private void PlayEffects(Transform targetTransform)
        {
            var explosionEffect = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosionEffect.hideFlags = HideFlags.HideInInspector;

            soundEffectsAudioSource = GameObject.FindWithTag("SoundEffects").GetComponent<AudioSource>();
            soundEffectsAudioSource.PlayOneShot(explosionClip);
        }



    }
}