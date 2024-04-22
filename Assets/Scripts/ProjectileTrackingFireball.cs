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
    public class ProjectileTrackingFireball : ProjectileBase
    {

        private GameObject[] enemyHolders;

        public Transform randomEnemyToFollow;
        private GameTickSystem gameTickSystem;

        [SerializeField] float startMoveSpeedMod = 0.3f;

        [SerializeField]
        float maxVelocity = 25f;

        private EnemyBoss enemyBoss;





        private int rechecksAllowed = 25;
        [SerializeField]
        private int rechecksDone = 0;
        private bool followingPlayer = false;


        protected override void Start()
        {
            base.Start();

            enemyHolders = GameObject.FindGameObjectsWithTag("EnemyHolder");

            StartMove();

            enemyBoss = FindAnyObjectByType<EnemyBoss>();

            AimAtRandomEnemy();

            cachedRigidbody.maxLinearVelocity = maxVelocity;

            gameTickSystem = FindAnyObjectByType<GameTickSystem>();

            gameTickSystem.OnEveryHalfTick.AddListener(DoChecks);
        }

        protected virtual void OnDestroy()
        {
            gameTickSystem.OnEveryHalfTick.RemoveListener(DoChecks);
        }

        protected virtual void DoChecks()
        {
            if (rechecksDone <= rechecksAllowed)
            {
                CheckIfTargetDead();
            }
            else if (!followingPlayer)
            {
                followingPlayer = true;
                randomEnemyToFollow = cachedUnitTransform;
            }

            if (followingPlayer)
            {
                if (Vector3.Distance(transform.position, cachedUnitTransform.position) <= 7f)
                {
                    followingPlayer = false;
                    rechecksDone = 0;
                    AimAtRandomEnemy();
                }
            }
        }

        protected override void FixedUpdate()
        {
            colliderGameObject.transform.LookAt(cachedPlayerCamera);
            transform.LookAt(randomEnemyToFollow);

            
            Move();
            //CheckPosition();
        }


        protected override void OnTriggerEnter(Collider other)
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

                PlayEffects(other.transform);
                Destroy(gameObject);
            }


            if (other.CompareTag("Background"))
            {
                PlayEffects(this.transform);
                Destroy(gameObject);
            }


        }

        private void AimAtRandomEnemy()
        {
            int randomIndex = Random.Range(0, enemyHolders.Length);

            if (rechecksDone >= rechecksAllowed)
            {
                randomEnemyToFollow = cachedUnitTransform;
                followingPlayer = true;
                return;
            }
            else
            {
                if (enemyBoss.canAttack == true)
                {
                    randomEnemyToFollow = enemyBoss.cachedModel.GetComponent<Transform>();
                }

                if (randomEnemyToFollow != enemyBoss.cachedModel.GetComponent<Transform>())
                {
                    for (int i = 0; i < enemyHolders[randomIndex].transform.childCount; i++)
                    {


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
                }

                if (randomEnemyToFollow == null)
                {
                    rechecksDone++;
                    AimAtRandomEnemy();
                }
                else
                {
                    if (randomEnemyToFollow.TryGetComponent(out EnemyBase enemyBase))
                    {
                        rechecksDone++;
                        if (enemyBase.canAttack == false) AimAtRandomEnemy();
                    }
                    
                }
            }
        }

        private void CheckIfTargetDead()
        {
            if (randomEnemyToFollow == null)
            {              
                AimAtRandomEnemy();
            } else if (randomEnemyToFollow.TryGetComponent(out EnemyBase enemyBase))
            {
                if (enemyBase.deathBegun)
                {
                    AimAtRandomEnemy();
                }
            }
        }

        private void StartMove()
        {
            cachedRigidbody.AddRelativeForce(transform.forward * ProjectileMoveSpeed * startMoveSpeedMod, ForceMode.Impulse);
        }

        protected override void Move()
        {
            if (randomEnemyToFollow != null)
            {
                //cachedRigidbody.MoveRotation(Quaternion.LookRotation(transform.position - randomEnemyToFollow.position));

                //Vector3 moveTarget = Vector3.MoveTowards(cachedRigidbody.position, randomEnemyToFollow.position, ProjectileMoveSpeed * Time.deltaTime);
                //cachedRigidbody.MovePosition(moveTarget);
                cachedRigidbody.AddForce(transform.forward * ProjectileMoveSpeed * 0.1f, ForceMode.Impulse);

            } else
            {
                cachedRigidbody.AddForce(transform.forward * ProjectileMoveSpeed * 0.1f, ForceMode.Impulse);
            }
            
            if (cachedRigidbody.position.y <= -8.5)
            {
                cachedRigidbody.AddForce(Vector3.up * ProjectileMoveSpeed * 0.1f, ForceMode.Impulse);
            }
        }



        protected override void RotateSprite()
        {
            
        }
    }
}