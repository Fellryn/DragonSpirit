using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to randomly properties of a particle effect in Unity
	/// Change material sheet, colour, speed etc.
	/// </summary>
	public class ParticleEffectRandom : MonoBehaviour 
	{
		[Header("Materials")]
		[SerializeField]
		bool useRandomMaterial = false;
		[SerializeField]
		Material[] materials;

        private void Start()
        {
            if (useRandomMaterial)
            {
				int materialIndex = Random.Range(0, materials.Length);

				if (materialIndex != 0) 
				{
					GetComponent<ParticleSystemRenderer>().material = materials[materialIndex];
				}

            }
        }



    }
}