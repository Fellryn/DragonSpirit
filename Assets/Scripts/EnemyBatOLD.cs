using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to use a subclass for an enemy (a bat) in Unity
    /// </summary>
    public class EnemyBatOLD : EnemyBaseOLD
    {
        [Header("General")]
        [SerializeField] string enemyName = "Bat";
        [SerializeField] int points = 50;
        [SerializeField] int lifePoints = 1;

        [Header("Movement")]
        [SerializeField] float moveSpeed = 30f;
        [SerializeField] float rotationSpeed = 15f;
        public bool moveSideways = false;

        [Header("Attack")]
        [SerializeField] float fireballMoveSpeedMin = 10f;
        [SerializeField] float fireballMoveSpeedMax = 15f;


        private void Start()
        {
            Initialise(enemyName, points, moveSpeed, rotationSpeed, lifePoints);
        }

        protected override void SetProjectileMoveSpeed(ProjectileFireball fireballScript)
        {
            fireballScript.ProjectileMoveSpeed = Random.Range(fireballMoveSpeedMin, fireballMoveSpeedMax);
        }

        public void BeginSidewaysMovement()
        {
            moveSideways = true;

            newOffset = new Vector3(initialOffset.x * 2, initialOffset.y, initialOffset.z);
        }

        public void StopSidewaysMovement()
        {
            moveSideways = false;
        }

        protected override void DoCheck()
        {
            if (moveSideways)
            {
                if (Vector3.Distance(cachedRigidbody.position, newOffset) > 0.02f)
                {
                    
                }
            }
        }

        public override void Move()
        {
            Vector3 moveTarget = Vector3.MoveTowards(cachedRigidbody.position, splineCart.transform.position + newOffset, MoveSpeed * Time.deltaTime);
            Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, splineCart.transform.rotation, RotationSpeed * Time.deltaTime);
            cachedRigidbody.MovePosition(moveTarget);
            cachedRigidbody.MoveRotation(rotateTarget);
        }
    }
}