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
    public class ProjectileFireball : MonoBehaviour
    {


        private float capsuleHeight = 25f;
        public float capsuleRadius = 0.05f;
        private float fireballLifetime = 10f;
        public float ProjectileMoveSpeed { get; set; }
        public string HitTag { get; set; }

        public Transform cachedPlayerCamera;
        public Transform cachedUnitTransform;

        private string enemyTag = "Enemy";
        private string playerTag = "Player";
        private string projectileString = "Projectile";

        GameObject colliderGameObject;
        CapsuleCollider cachedCollider;
        Rigidbody cachedRigidbody;

        GameObject cachedModel;

        private void OnEnable()
        {
            if (ProjectileMoveSpeed == 0)
            {
                ProjectileMoveSpeed = 20f;
            }

            cachedRigidbody = GetComponent<Rigidbody>();
            //cachedCollider = GetComponent<CapsuleCollider>();
            cachedRigidbody.useGravity = false;
            cachedRigidbody.drag = 0f;


            colliderGameObject = new GameObject("Collider");
            colliderGameObject.transform.tag = transform.tag;
            colliderGameObject.transform.SetParent(transform);
            colliderGameObject.layer = LayerMask.NameToLayer("NoCollide");
            cachedCollider = colliderGameObject.AddComponent<CapsuleCollider>();
            cachedCollider.height = capsuleHeight;
            cachedCollider.radius = capsuleRadius;
            cachedCollider.direction = 2;
            cachedCollider.isTrigger = true;
            cachedCollider.excludeLayers = 1 << LayerMask.NameToLayer("NoCollide");


            cachedModel = Instantiate(FindAnyObjectByType<PrefabReferencesLink>().prefabReferences.fireballPrefab, transform.position, Quaternion.identity, transform);
        }

        private void Start()
        {
            transform.position = cachedUnitTransform.position;
            transform.rotation = cachedUnitTransform.rotation;

            if (transform.CompareTag(enemyTag + projectileString))
            {
                ProjectileMoveSpeed *= -1;
            }

            Destroy(gameObject, fireballLifetime);

            Move();
 
        }


        private void FixedUpdate()
        {
            colliderGameObject.transform.LookAt(cachedPlayerCamera);

        }

        private void OnTriggerEnter(Collider other)
        {
            if (transform.CompareTag(enemyTag + projectileString))
            {
                if (other.CompareTag(playerTag))
                {
                    Destroy(other.gameObject);
                }
            }

            if (transform.CompareTag(playerTag + projectileString))
            {
                if (other.CompareTag(enemyTag))
                {
                    Destroy(other.gameObject);
                }
            }

        }

        private void Move()
        {
            cachedRigidbody.AddRelativeForce(Vector3.forward * ProjectileMoveSpeed, ForceMode.Impulse);
        }


    }
}