using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LukeMartin;

namespace LukeMartin
{
	/// <summary>
	/// Author: Luke Martin
	/// Description: This script demonstrates how to change material of a particle effect in Unity
	/// </summary>
	public class ParticleEffectRandom : MonoBehaviour 
	{
		[SerializeField] bool useRandomMaterial = false;
		[SerializeField] Material[] materials;

        private void Start()
        {
            if (useRandomMaterial == true)
            {
				int materialNumber = Random.Range(0, materials.Length);

				if (materialNumber != 0) 
				{
					GetComponent<ParticleSystemRenderer>().material = materials[materialNumber];
				}

            }
        }
    }
}