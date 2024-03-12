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
        Rigidbody cachedRigidbody;

        private float capsuleHeight = 100f;
        public float capsuleRadius = 0.05f;

        public Transform cachedPlayerCamera;
        public Transform cachedPlayerTransform;

        GameObject colliderGameObject;
        CapsuleCollider cachedCollider;

        GameObject cachedModel;

        private void OnEnable()
        {
            cachedRigidbody = GetComponent<Rigidbody>();
            //cachedCollider = GetComponent<CapsuleCollider>();
            cachedRigidbody.useGravity = false;


            colliderGameObject = new GameObject("Collider");
            colliderGameObject.transform.tag = transform.tag;
            colliderGameObject.transform.SetParent(transform);
            cachedCollider = colliderGameObject.AddComponent<CapsuleCollider>();
            cachedCollider.height = capsuleHeight;
            cachedCollider.radius = capsuleRadius;
            cachedCollider.direction = 2;
            cachedCollider.isTrigger = true;


            cachedModel = Instantiate(FindAnyObjectByType<PrefabReferencesLink>().prefabReferences.fireballPrefab, transform.position, Quaternion.identity, transform);
        }

        private void Start()
        {
            transform.position = cachedPlayerTransform.position;
            transform.rotation = cachedPlayerTransform.rotation;
        }


        private void Update()
        {
            colliderGameObject.transform.position = (transform.position + cachedPlayerCamera.position) / 2;
            colliderGameObject.transform.LookAt(transform.position);

            transform.Translate(Vector3.forward * 0.3f);

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Enemy"))
            {
                Destroy(other.gameObject);
            }
        }
    }
}