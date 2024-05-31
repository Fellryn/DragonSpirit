using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to let  player collision be detected at any depth in Unity
	/// </summary>
	public class PlayerCollision : MonoBehaviour 
	{
        [SerializeField]
        PlayerStats playerStats;
        [SerializeField]
        Transform cachedPlayerCamera;

        private float capsuleHeight = 50f;
        public float capsuleRadius = 0.5f;

        GameObject colliderGameObject;
        CapsuleCollider cachedCollider;

        [SerializeField]
        string enemyTag = "Enemy";

        private void OnEnable()
        {
            colliderGameObject = new GameObject("Collider");
            colliderGameObject.transform.tag = transform.tag;
            colliderGameObject.transform.SetParent(transform);
            colliderGameObject.transform.position = transform.position;
            colliderGameObject.layer = LayerMask.NameToLayer("NoCollide");
            cachedCollider = colliderGameObject.AddComponent<CapsuleCollider>();
            cachedCollider.height = capsuleHeight;
            cachedCollider.radius = capsuleRadius;
            cachedCollider.direction = 2;
            cachedCollider.isTrigger = true;
            cachedCollider.excludeLayers = 1 << LayerMask.NameToLayer("NoCollide");
        }

        private void Update()
        {
            colliderGameObject.transform.LookAt(cachedPlayerCamera);
        }

        protected void OnTriggerEnter(Collider colliderHit)
        {
            if (colliderHit.CompareTag(enemyTag))
            {
                colliderHit.TryGetComponent<EnemyBase>(out EnemyBase enemyBase);
                if (enemyBase != null)
                {
                    playerStats.PlayerTakeDamage(enemyBase.health);
                    enemyBase.TakeDamage(enemyBase.health, playerStats.isPlayerOne);
                    enemyBase.BeginDeath();
                }
            }
        }

    }
}