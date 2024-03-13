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
    public class EnemyBat : EnemyBase
    {
        [Header("General")]
        [SerializeField] string enemyName = "Bat";
        [SerializeField] int points = 50;

        [Header("Movement")]
        [SerializeField] float moveSpeed = 30f;
        [SerializeField] float rotationSpeed = 15f;

        [Header("Attack")]
        [SerializeField] float fireballMoveSpeedMin = 10f;
        [SerializeField] float fireballMoveSpeedMax = 15f;

        private void Start()
        {
            Initialise(enemyName, points, moveSpeed, rotationSpeed);
        }

        protected override void SetProjectileMoveSpeed(ProjectileFireball fireballScript)
        {
            fireballScript.ProjectileMoveSpeed = Random.Range(fireballMoveSpeedMin, fireballMoveSpeedMax);
        }
    }
}