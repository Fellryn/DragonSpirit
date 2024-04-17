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







        private int rechecksAllowed = 5;
        private int rechecksDone = 0;


        protected override void Start()
        {
            base.Start();

            enemyHolders = GameObject.FindGameObjectsWithTag("EnemyHolder");

            StartMove();

            AimAtRandomEnemy();
        }


        protected override void FixedUpdate()
        {
            colliderGameObject.transform.LookAt(cachedPlayerCamera);

            CheckIfTargetDead();
            Move();
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
                if (randomEnemyToFollow.TryGetComponent(out EnemyBase enemyBase))
                {
                    if (enemyBase.cachedModel.TryGetComponent(out Renderer renderer))
                    {
                        if (!renderer.isVisible) AimAtRandomEnemy();
                    }
                }
                //if (!randomEnemyToFollow.GetComponent<EnemyBase>().cachedModel.GetComponent<Renderer>().isVisible) AimAtRandomEnemy();
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

        protected override void Move()
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

        protected override void RotateSprite()
        {
            
        }

    }
}