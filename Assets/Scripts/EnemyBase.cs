using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to create an enemy base super class in Unity
    /// </summary>
    public abstract class EnemyBase : MonoBehaviour
    {
        protected GameTickSystem gameTickSystem;

        [Header("Base")]
        [SerializeField]
        string enemyName = "Enemy";
        public int health = 1;
        public int score = 1;
        public Transform cachedModel;
        public Transform cachedProjectileTrackingPoint;
        protected Collider cachedCollider;
        public bool canAttack = false;
        public bool isInvincible = true;

        [Header("Death")]
        public bool deathBegun = false;
        [SerializeField]
        bool ragdollOnDeath = false;
        [SerializeField]
        Transform ragdollObject;

        [SerializeField]
        private float explosionPower = 10f;
        [SerializeField]
        private float explosionRadius = 5f;
        [SerializeField]
        private bool useExplosiveForceEffectOnDeath = false;


        public delegate void OnKill(int scoreToAdd);
        public static event OnKill onKill;

        protected Rigidbody cachedRigidbody;


        protected virtual void OnEnable()
        {
            gameTickSystem = FindFirstObjectByType<GameTickSystem>();
            cachedRigidbody = GetComponent<Rigidbody>();

            cachedCollider = GetComponent<Collider>();

            if (transform.TryGetComponent(out Collider collider))
            {
                cachedCollider = collider;
            } else
            {
                cachedCollider = transform.GetComponentInChildren<Collider>();
            }

            

            transform.name = enemyName;
        }


        public void TakeDamage(int damageAmount)
        {
            if (!isInvincible)
            {
                health -= damageAmount;
                HealthCheck();
            }
        }


        protected virtual void HealthCheck()
        {
            if (health <= 0)
            {
                BeginDeath();
            }
        }



        public virtual void BeginDeath()
        {
            deathBegun = true;
            onKill?.Invoke(score);

            transform.tag = "Untagged";
            cachedRigidbody.useGravity = true;
            cachedRigidbody.isKinematic = false;
            cachedRigidbody.includeLayers = 1 << LayerMask.NameToLayer("NoCollide");
            if (useExplosiveForceEffectOnDeath)
            {
                cachedRigidbody.AddExplosionForce(explosionPower, (UnityEngine.Random.insideUnitSphere * 2f) + transform.position, explosionRadius, 0f, ForceMode.Impulse);
            }

            cachedCollider.isTrigger = false;
            
            

            if (ragdollOnDeath)
            {
                cachedModel.gameObject.SetActive(false);
                ragdollObject.gameObject.SetActive(true);
                cachedCollider.isTrigger = true;
            }

            if (TryGetComponent<EnemyShaderController>(out EnemyShaderController enemyShaderController))
            {
                enemyShaderController.BeginDissolveAnimation();
            }

            if (TryGetComponent<EnemyAnimation>(out EnemyAnimation enemyAnimation))
            {
                enemyAnimation.OnDeath();
            }

            Destroy(gameObject, 3f);

            enabled = false;
        }

    }
}