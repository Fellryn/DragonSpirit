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
        [SerializeField] GameTickSystem gameTickSystem;

        //[SerializeField] CapsuleCollider cachedCollider;

		[SerializeField] string powerUpTag = "PowerUp";
		[SerializeField] const string lifePowerUpName = "LifePowerUp";
		[SerializeField] const string multiShotPowerUpName = "MultiShotPowerUp";
		public int healPerPickup = 1;

        public bool MultiShotActive { get; set; }
        [SerializeField]
        float multiShotActiveTime = 5f;
        [SerializeField]
        bool additiveTimePerPickup = true;
        public int CurrentMultiShotLevel { get; set; }
        private float multiShotTimer;


        private void OnEnable()
        {
            MultiShotActive = false;
            gameTickSystem.OnEveryHalfTick.AddListener(PowerUpTimingChecks);
        }

        private void OnDisable()
        {
            gameTickSystem.OnEveryHalfTick.RemoveListener(PowerUpTimingChecks);
        }

        private void PowerUpTimingChecks()
        {
            // MultiShot Timing Check
            if (MultiShotActive)
            {
                if (multiShotTimer <= 0)
                {
                    MultiShotActive = false;
                    ChangeMultiShotLevel(0, true);
                    multiShotTimer = 0;
                } else
                {
                    multiShotTimer -= 0.5f;
                }
            }

        }

        public void ChangeMultiShotLevel(int level, bool reset = false)
        {
            if (reset)
            {
                CurrentMultiShotLevel = 0;
                return;
            }

            CurrentMultiShotLevel += level;

            MultiShotActive = true;
            if (additiveTimePerPickup)
            {
                multiShotTimer += multiShotActiveTime;
            } else
            {
                multiShotTimer = multiShotActiveTime;
            }
        }

        public int UseMultiShot()
        {
            if (CurrentMultiShotLevel == 1)
            {
                return 3;
            } else
            {
                return 3 + ((CurrentMultiShotLevel - 1) * 2);
            }   
        }



        private void OnTriggerEnter(Collider otherColliderHit)
        {
            if (otherColliderHit.CompareTag(powerUpTag))
            {
                if (otherColliderHit.TryGetComponent(out PowerUpBase powerUpBase))
                {
                    switch (powerUpBase.powerUpName)
                    {
                        case lifePowerUpName:
                            playerStats.PlayerHeal(healPerPickup);
                            break;
                        case multiShotPowerUpName:
                            ChangeMultiShotLevel(1);
                            break;
                        default:
                            break;
                    }
                }

                Destroy(otherColliderHit.gameObject);

            }
        }
    }
}