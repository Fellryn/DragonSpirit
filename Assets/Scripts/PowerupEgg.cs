using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to make an egg that, when destroyed, releases a powerup floating around for the player to pick up in Unity
	/// </summary>
	public class PowerupEgg : MonoBehaviour 
	{
		[SerializeField]
		GameObject powerupPrefabToSpawn;

		[SerializeField]
		bool spawnRandomPowerup;
		[SerializeField]
		GameObject[] powerupsArray;

		[SerializeField]
		GameObject powerupEggDestroyEffect;



		private void OnDestroy()
        {
			if (!gameObject.scene.isLoaded) return;

            if (spawnRandomPowerup)
            {
				int powerupIndex = Random.Range(0, powerupsArray.Length);
				Instantiate(powerupsArray[powerupIndex], transform.position, Quaternion.identity);
            } else
            {
				Instantiate(powerupPrefabToSpawn, transform.position, Quaternion.identity);
			}

			Instantiate(powerupEggDestroyEffect, transform.position, Quaternion.identity);
        }

    }
}