using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using KurtSingle;


namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to make a child/parent class for a melee attacker in Unity
	/// </summary>
	public abstract class EnemyMeleeAttack : EnemyBase 
	{
        [Header("Attack")]
        [SerializeField] int ticksBeforeAttack = 4;
        private int ticksRun = 0;
        private bool isPerformingAttack = false;
        private Transform cachedPlayerTransform;
        private Camera cachedPlayerCamera;


        [Header("Movement")]
        [SerializeField] float moveSpeed = 10f;
        [SerializeField] float rotationSpeed = 15f;
        public bool useSpline = false;
        [SerializeField] float distanceThresholdToAttack = 5f;
        [SerializeField] float attackSpeed = 3f;

        public Vector3 initialMoveTarget;

        private Vector3 storedMoveTarget;
        private float storedMoveSpeed;



        protected override void OnEnable()
        {
            base.OnEnable();
            gameTickSystem.OnEveryHalfTick.AddListener(DoChecks);
            cachedPlayerTransform = FindAnyObjectByType<PlayerMovement>().GetComponent<Transform>();
            cachedPlayerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            
            storedMoveSpeed = moveSpeed;
            storedMoveTarget = initialMoveTarget;
        }

        protected virtual void OnDisable()
        {
            gameTickSystem.OnEveryHalfTick.RemoveListener(DoChecks);
            
        }

        protected virtual void FixedUpdate()
        {
            Move();

            if (canAttack)
            {
                DoScreenCheck();
            }
        }


        protected virtual void DoChecks()
        {
            DoPositionCheck();
            DoAttackTimingCheck();
        }

        private void DoPositionCheck()
        {
            if (!useSpline && !isPerformingAttack)
            {
                if (Vector3.Distance(cachedRigidbody.position, cachedPlayerTransform.position + initialMoveTarget) < distanceThresholdToAttack)
                {
                    canAttack = true;
                    moveSpeed = storedMoveSpeed / 2;
                }
                else
                {
                    moveSpeed = storedMoveSpeed;
                    initialMoveTarget = storedMoveTarget;
                }
            }
        }


        protected virtual void DoScreenCheck()
        {
            if (canAttack)
            {
                if (cachedPlayerCamera.WorldToViewportPoint(transform.position).x > 1.1f  ||
                    cachedPlayerCamera.WorldToViewportPoint(transform.position).x < -0.1f ||
                    cachedPlayerCamera.WorldToViewportPoint(transform.position).y > 1.1f  ||
                    cachedPlayerCamera.WorldToViewportPoint(transform.position).y < -0.1f)
                {
                    Destroy(gameObject);
                }
            }
        }


        protected virtual void DoAttackTimingCheck()
        {
            if (canAttack)
            {
                if (ticksRun >= ticksBeforeAttack)
                {
                    Attack();
                    ticksRun = 0;
                } else
                {
                    ticksRun += 1;
                }
            }
        }


        protected virtual void Attack()
        {
            isPerformingAttack = true;
            initialMoveTarget = new Vector3(0f, 0f, 0f);

        }


        protected virtual void Move()
        {
            Vector3 moveTarget = Vector3.MoveTowards(cachedRigidbody.position, cachedPlayerTransform.position + initialMoveTarget, moveSpeed * Time.deltaTime);
            if (isPerformingAttack)
            {
                //moveTarget = Vector3.MoveTowards(cachedRigidbody.position, cachedPlayerTransform.position, moveSpeed * Time.deltaTime);
                cachedRigidbody.isKinematic = false;
                cachedRigidbody.AddForce((cachedPlayerTransform.position - cachedRigidbody.position) * attackSpeed, ForceMode.Force);
                if (attackSpeed != 0f && cachedRigidbody.velocity.magnitude >= 25f)
                {
                    attackSpeed = 0f;
                }
                    return;
            }

            Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, cachedPlayerTransform.rotation, rotationSpeed * Time.deltaTime);
            //Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, Quaternion.LookRotation(cachedRigidbody.position - cachedPlayerTransform.position), rotationSpeed * Time.deltaTime);
            cachedRigidbody.MovePosition(moveTarget);
            cachedRigidbody.MoveRotation(rotateTarget);
        }


        public void ChangeMoveTarget(Vector3 target)
        {
            storedMoveTarget = target;
        }


    }
}