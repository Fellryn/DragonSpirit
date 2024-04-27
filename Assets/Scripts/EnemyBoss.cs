using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using UlianaKutsenko;
using DG.Tweening;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to manage a boss's movement and attacking in Unity
    /// Inherits from the EnemyRangedAttack script (EnemyBase hierarchy)
    /// </summary>
    public class EnemyBoss : EnemyRangedAttack
    {
        [Header("References")]
        [SerializeField] GameVars gameVars;
        [SerializeField] SceneNavigation sceneNavigation;
        [SerializeField] EnemyBossAnimation enemyBossAnimation;
        [SerializeField] EnemyBossLookConstraint enemyBossLookConstraint;
        [SerializeField] CameraController cameraController;
        private Camera cachedPlayerCamera;

        [Header("Attack Extended")]
        [SerializeField] int maxFireballPerVolley = 15;
        private int currentFireballVolleyCount = 0;
        [SerializeField] float maxTimeBetweenAttacks = 3f;
        [SerializeField] float minTimeBetweenAttacks = 1f;
        [SerializeField] bool randomTimeBetweenAttacks = true;
        private float attackTimer = 0f;

        [Header("Movement")]
        //[SerializeField] float movementSpeed = 10f;
        [SerializeField] float delayBeforeMove = 3f;
        [SerializeField] float maxMoveTime = 3f;
        [SerializeField] float minMoveTime = 0.5f;
        [SerializeField] Vector3 firstPosition;
        [SerializeField] Transform minPositionObject;
        [SerializeField] Transform maxPositionObject;
        [SerializeField] float yHeight = -28f;
        private Vector3 moveTargetPosition;
        private float moveTimer = 0f;
        private bool allowTimerCount;

        [Header("PowerUp Spawns")]
        [SerializeField] GameObject[] powerUpPrefabs;

        protected override void OnEnable()
        {
            base.OnEnable();
            gameTickSystem.OnEveryHalfTick.AddListener(DoChecks);
            cachedPlayerCamera = mainCameraTransform.GetComponent<Camera>();
            cameraController.AtBossEvent.AddListener(BossInit);
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            gameTickSystem.OnEveryHalfTick.RemoveListener(DoChecks);
            cameraController.AtBossEvent.RemoveListener(BossInit);
        }


        protected virtual void Update()
        {
            if (enemyBossLookConstraint.lookingAtPlayer && attackTimer <= 0.99f)
            {
                FireballAttack();
            }

            //if (attackTimer >= 1f && enemyBossAnimation.CheckAttackState())
            //         {
            //	enemyBossAnimation.AttackCompleted();
            //         }
        }

        private void OnDestroy()
        {
            DOTween.Kill(transform);

            if (!gameObject.scene.isLoaded) return;

            gameVars.WonLevel(true);
            sceneNavigation.ChangeScene("GameOver");

        }

        private void BossInit()
        {
            moveTargetPosition = cachedPlayerCamera.ViewportToWorldPoint(firstPosition);
            Move();

        }


        private void DoChecks()
        {
            MoveTimingCheck();
            AttackTimingCheck();
        }

        protected override void HealthCheck()
        {
            base.HealthCheck();

            if (health % 10 == 0)
            {
                int i = Random.Range(0, powerUpPrefabs.Length);
                Instantiate(powerUpPrefabs[i], projectileOrigin.position, Quaternion.identity);
            }
        }



        private void MoveTimingCheck()
        {
            if (moveTimer >= delayBeforeMove)
            {
                SetRandomMovePosition();
                moveTimer = 0f;
            }
            else
            {
                if (allowTimerCount) moveTimer += 0.5f;
            }
        }



        private void SetRandomMovePosition()
        {
            float randomPositionX = Random.Range(minPositionObject.position.x, maxPositionObject.position.x);
            float randomPositionZ = Random.Range(minPositionObject.position.z, maxPositionObject.position.z);

            //Vector3 randomScreenPosition = new Vector3(randomScreenPositionX, randomScreenPositionY, yHeight);

            //moveTargetPosition = cachedPlayerCamera.ViewportToWorldPoint(randomScreenPosition);
            moveTargetPosition = new Vector3(randomPositionX, yHeight, randomPositionZ);

            Move();
        }


        private void Move()
        {
            DOTween.Kill(400);

            allowTimerCount = false;

            moveTargetPosition.y = yHeight;

            float moveTime = (Vector3.Distance(transform.position, moveTargetPosition) * 0.1f);
            moveTime = Mathf.Clamp(moveTime, minMoveTime, maxMoveTime);
            enemyBossAnimation.SetMoveBool(true);

            transform.DOMove(moveTargetPosition, moveTime).SetEase(Ease.InOutCubic).OnComplete(StopMove).SetId(600);

            if (Vector3.Distance(transform.position, moveTargetPosition) > 3f)
            {
                if (transform.position.x < moveTargetPosition.x)
                {
                    transform.DORotate(new Vector3(0f, -10f, 0f), moveTime / 2f).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo).SetId(500);
                }
                else
                {
                    transform.DORotate(new Vector3(0f, 10f, 0f), moveTime / 2f).SetEase(Ease.InOutCubic).SetLoops(2, LoopType.Yoyo).SetId(500);
                }
            }


            //transform.DOJump(moveTargetPosition, 4f, 1, moveTime).SetEase(Ease.OutCubic).OnComplete(StopMove).SetId(100);
        }

        private void StopMove()
        {
            canAttack = true;

            transform.DOShakePosition(delayBeforeMove, 1, 1, fadeOut: false).SetId(400);

            allowTimerCount = true;
            enemyBossAnimation.SetMoveBool(false);
        }

        private void AttackTimingCheck()
        {
            if (attackTimer >= maxTimeBetweenAttacks)
            {
                Attack();
                attackTimer = 0f;
            }
            else
            {
                attackTimer += 0.5f;
                if (randomTimeBetweenAttacks && attackTimer >= minTimeBetweenAttacks)
                {
                    if (Random.Range(0f, 1f) <= attackChance)
                    {
                        Attack();
                        attackTimer = 0f;
                    }
                }
            }

            if (attackTimer >= 1f && enemyBossAnimation.CheckAttackState())
            {
                enemyBossAnimation.AttackCompleted();
            }
        }


        protected override void Attack()
        {
            if (canAttack)
            {
                currentFireballVolleyCount = 0;
                enemyBossLookConstraint.LookAtPlayer();
            }
            
        }



        public void FireballAttack()
        {
            if (!enemyBossAnimation.CheckAttackState())
            {
                enemyBossAnimation.AttackBegun();
            }


            if (currentFireballVolleyCount <= maxFireballPerVolley)
            {
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

                currentFireballVolleyCount++;
            }



            //canAttack = false;
        }




    }
}