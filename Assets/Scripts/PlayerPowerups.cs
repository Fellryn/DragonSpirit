using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to pickup powerups and apply their effects in Unity
	/// </summary>
	public class PlayerPowerups : MonoBehaviour 
	{
        [SerializeField] PlayerStats playerStats;

        //[SerializeField] CapsuleCollider cachedCollider;

		[SerializeField] string lifePowerupTag = "PowerupLife";
		public int healPerPickup = 1;



        private void OnTriggerEnter(Collider otherColliderHit)
        {
            if (otherColliderHit.CompareTag(lifePowerupTag))
			{
				playerStats.PlayerHeal(healPerPickup);

				Destroy(otherColliderHit.gameObject);
			}

        }



    }
}