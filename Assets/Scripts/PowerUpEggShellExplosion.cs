using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to throw egg shell parts around randomly when spawned (when egg is destroyed) in Unity
	/// </summary>
	public class PowerUpEggShellExplosion : MonoBehaviour 
	{
		[SerializeField] Rigidbody[] eggShellParts;

        [SerializeField] float explosionForce = 500f;
        [SerializeField] float explosionRadius = 3f;
        [SerializeField] float explosionUpwardModifier = 1f;

        private void Start()
        {
            for (int i = 0; i < eggShellParts.Length; i++)
            {
                eggShellParts[i].AddExplosionForce(explosionForce, transform.position, explosionRadius, explosionUpwardModifier);
            }
        }

    }
}