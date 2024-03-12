using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to ... in Unity
	/// </summary>
	public class PlayerCollision : MonoBehaviour 
	{
        [SerializeField] Transform cachedPlayerCamera;

        private float capsuleHeight = 50f;
        public float capsuleRadius = 0.5f;

        GameObject colliderGameObject;
        CapsuleCollider cachedCollider;

        private void OnEnable()
        {
            colliderGameObject = new GameObject("Collider");
            colliderGameObject.transform.tag = transform.tag;
            colliderGameObject.transform.SetParent(transform);
            cachedCollider = colliderGameObject.AddComponent<CapsuleCollider>();
            cachedCollider.height = capsuleHeight;
            cachedCollider.radius = capsuleRadius;
            cachedCollider.direction = 2;
            cachedCollider.isTrigger = true;
        }

        private void Update()
        {
            colliderGameObject.transform.position = (transform.position + cachedPlayerCamera.position) / 2;
            colliderGameObject.transform.LookAt(transform.position);
        }

    }
}