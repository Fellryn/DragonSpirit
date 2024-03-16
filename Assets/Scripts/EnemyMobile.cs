using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using KurtSingle;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to make a child class from a child/parent class for movement in Unity
    /// </summary>
    public abstract class EnemyMobile : EnemyRangedAttack
    {
        [Header("Movement")]
        [SerializeField] float moveSpeed = 30f;
        [SerializeField] float rotationSpeed = 15f;
        public bool useSpline = false;
        [SerializeField] float distanceThresholdToAttack = 5f;

        public CinemachineSplineCart splineCart;
        public Vector3 initialMoveTarget;

        private Vector3 storedMoveTarget;
        private float storedMoveSpeed;

        protected override void OnEnable()
        {
            base.OnEnable();
            gameTickSystem.OnEveryHalfTick.AddListener(DoPositionCheck);
            storedMoveSpeed = moveSpeed;
            storedMoveTarget = initialMoveTarget;

        }

        protected override void OnDisable()
        {
            base.OnDisable();
            gameTickSystem.OnEveryHalfTick.RemoveListener(DoPositionCheck);
        }

        protected virtual void FixedUpdate()
        {
            if (useSpline)
            {
                SplineMove();
            }
            else
            {
                Move();
            }
        }

        protected virtual void SplineMove()
        {
            Vector3 moveTarget = Vector3.MoveTowards(cachedRigidbody.position, splineCart.transform.position + initialMoveTarget, moveSpeed * Time.deltaTime);
            Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, splineCart.transform.rotation, rotationSpeed * Time.deltaTime);
            cachedRigidbody.MovePosition(moveTarget);
            cachedRigidbody.MoveRotation(rotateTarget);
        }

        protected virtual void Move()
        {
            Vector3 moveTarget = Vector3.MoveTowards(cachedRigidbody.position, cachedPlayerTransform.position + initialMoveTarget, moveSpeed * Time.deltaTime);

            Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, Quaternion.LookRotation(cachedRigidbody.position - cachedPlayerTransform.position), rotationSpeed * Time.deltaTime);
            cachedRigidbody.MovePosition(moveTarget);
            cachedRigidbody.MoveRotation(rotateTarget);
        }

        private void DoPositionCheck()
        {
            if (!useSpline)
            {
                if (Vector3.Distance(cachedRigidbody.position, cachedPlayerTransform.position + initialMoveTarget) < distanceThresholdToAttack)
                {
                    canAttack = true;
                    moveSpeed = storedMoveSpeed / 2;

                    initialMoveTarget = new Vector3(0f, storedMoveTarget.y, storedMoveTarget.z);
                } else
                {
                    moveSpeed = storedMoveSpeed;
                    initialMoveTarget = storedMoveTarget;
                }
            }
        }

        public void ChangeMoveTarget(Vector3 target)
        {
            storedMoveTarget = target;
        }


    }
}