using UnityEngine;
using UlianaKutsenko;
using DG.Tweening;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to manage a boss's movement and attacking in Unity
    /// Inherits from the EnemyBase > EnemyRangedAttack parent classes
    /// </summary>
    public class EnemyBoss : EnemyRangedAttack
    {
        [Header("References")]
        [SerializeField] GameVars gameVars;
        [SerializeField] SceneNavigation sceneNavigation;
        [SerializeField] EnemyBossAnimation enemyBossAnimation;
        [SerializeField] EnemyBossLookConstraint enemyBossLookConstraint;
        [SerializeField] EnemyBossHealthBar enemyBossHealthBar;
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

        [Header("Extras")]
        [SerializeField] GameObject[] powerUpPrefabs;
        [SerializeField] int pointsAwardedForHit = 25;
        private int lastPowerupIndex = -1;
        public delegate void OnBossHit(int points, bool lastHitByPlayerOne);
        public static event OnBossHit onBossHit;

        // Example of polymorphism (extending parent function by using base.OnEnable())
        // Adding and removing listeners, getting references
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

        // Used for mass fireball attacking. If looking at player, keep sending fireballs
        protected virtual void Update()
        {
            if (enemyBossLookConstraint.lookingAtPlayer && attackTimer <= 0.99f)
            {
                FireballAttack();
            }

            //Debug.Log(Vector3.Distance(projectileOrigin.position, cachedPlayerTransform.position));
            //if (Vector3.Distance(projectileOrigin.position, cachedPlayerTransform.position) <= 18f && canAttack)
            //{
            //    moveTimer = delayBeforeMove;
            //}
        }

        // When destroyed, win round
        private void OnDestroy()
        {
            DOTween.Kill(transform);

            if (!gameObject.scene.isLoaded) return;

            gameVars.WonLevel(true);
            sceneNavigation.ChangeScene("GameOver");
        }


        // Called when first entering boss area, instructs boss to move to center of screen
        private void BossInit()
        {
            moveTargetPosition = cachedPlayerCamera.ViewportToWorldPoint(firstPosition);
            enemyBossHealthBar.ShowHealthBar(health);
            Move();

        }

        // Do various checks, which is called by tick system event
        private void DoChecks()
        {
            MoveTimingCheck();
            AttackTimingCheck();
        }

        // Example of polymorphism (Changing the function while still keeping the same name and previous behaviour)
        // When health is 0, do normal behaviour, but also every 10 health spawn a power up
        protected override void HealthCheck()
        {
            base.HealthCheck();

            if (health % 10 == 0)
            {
                int i = Random.Range(0, powerUpPrefabs.Length);
                if (i == lastPowerupIndex)
                {
                    i++;
                    if (i >= powerUpPrefabs.Length) i = 0;
                }
                lastPowerupIndex = i;
                Instantiate(powerUpPrefabs[i], projectileOrigin.position, Quaternion.identity);
            }
        }

        // Check if boss should move by using game tick timer
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

        // Set random move position based off two scene objects
        private void SetRandomMovePosition()
        {
            float randomPositionX = Random.Range(minPositionObject.position.x, maxPositionObject.position.x);
            float randomPositionZ = Random.Range(minPositionObject.position.z, maxPositionObject.position.z);

            moveTargetPosition = new Vector3(randomPositionX, yHeight, randomPositionZ);

            Move();
        }

        // Move toward random move position using DOTween
        // Rotate boss during movement depending on which direction moving
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
        }


        // Called by tween callback when move complete. Resets states and begins "floating" tween
        private void StopMove()
        {
            canAttack = true;

            transform.DOShakePosition(delayBeforeMove, 1, 1, fadeOut: false).SetId(400);

            allowTimerCount = true;
            enemyBossAnimation.SetMoveBool(false);
        }

        // Checks if long enough has passed before attacking again. Uses game tick system as well
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

        // Example of Polymorphism (attack, but do different attack than normal)
        // Once beginning attack, look at player and reset fireball count
        protected override void Attack()
        {
            if (canAttack)
            {
                currentFireballVolleyCount = 0;
                enemyBossLookConstraint.LookAtPlayer();
            }
        }


        // Example of Abstraction (using function from other parent scripts to send fireballs)
        // Plays attacking animation and launches fireballs if not at max fireball count
        public void FireballAttack()
        {
            if (!enemyBossAnimation.CheckAttackState())
            {
                enemyBossAnimation.AttackBegun();
            }

            if (currentFireballVolleyCount <= maxFireballPerVolley)
            {
                LaunchFireball();

                currentFireballVolleyCount++;
            }
        }

        public override void TakeDamage(int damageAmount, bool hitByPlayerOne)
        {
            base.TakeDamage(damageAmount, hitByPlayerOne);

            onBossHit?.Invoke(pointsAwardedForHit, hitByPlayerOne);

            enemyBossHealthBar.UpdateHealth(health);
        }

    }
}