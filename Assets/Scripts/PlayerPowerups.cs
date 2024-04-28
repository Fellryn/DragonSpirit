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
        [Header("References")]
        [SerializeField] PlayerStats playerStats;
        [SerializeField] GameTickSystem gameTickSystem;
        [SerializeField] PlayerAttack playerAttack;
        [SerializeField] PlayerShaderController playerShaderController;
        [SerializeField] RectTransform powerUpDisplay;
		[SerializeField] string powerUpTag = "PowerUp";

        
        
        [Header("Life PowerUp")]
        [SerializeField]
        GameObject lifePowerUpPlayerEffect;
        [SerializeField]
        const string lifePowerUpName = "LifePowerUp";
        public int healPerPickup = 5;

        [Header("MultiShot PowerUp")]
        [SerializeField]
        GameObject multiShotPowerUpPlayerEffect;
        [SerializeField]
        float multiShotActiveTime = 5f;
        [SerializeField]
        const string multiShotPowerUpName = "MultiShotPowerUp";
        [SerializeField]
        bool additiveTimePerPickup = true;
        public int CurrentMultiShotLevel { get; set; }
        public bool MultiShotActive { get; set; }
        private float multiShotTimer;

        [Header("Tracking PowerUp")]
        [SerializeField] GameObject trackingPowerUpPlayerEffect;
        const string trackingPowerUpName = "TrackingPowerUp";
        public bool TrackingPowerUpActive { get; set; }




        private void OnEnable()
        {
            MultiShotActive = false;
            gameTickSystem.OnEveryHalfTick.AddListener(PowerUpTimingChecks);
        }


        private void OnDisable()
        {
            gameTickSystem.OnEveryHalfTick.RemoveListener(PowerUpTimingChecks);
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
                            Instantiate(lifePowerUpPlayerEffect, transform);
                            break;
                        case multiShotPowerUpName:
                            ChangeMultiShotLevel(1);
                            Instantiate(multiShotPowerUpPlayerEffect, transform);
                            break;
                        case trackingPowerUpName:
                            ActivateTrackingAbility();
                            Instantiate(trackingPowerUpPlayerEffect, transform);
                            break;
                        default:
                            break;
                    }

                    powerUpBase.BeginDestruction();
                }

                //Destroy(otherColliderHit.gameObject);

            }
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


        private void ActivateTrackingAbility()
        {
            DeactivateAllAbilites();

            powerUpDisplay.gameObject.SetActive(true);
            TrackingPowerUpActive = true;
            playerStats.PlayerGainMana(100f);
            //playerShaderController.TweenEmissionFromMana();
        }

        public bool UseTrackingAbility()
        {
            if (playerStats.PlayerUseMana(playerAttack.trackingFireballManaCost))
            {
                return true;
            } else
            {
                return false;
            }
            
        }


        public void DeactivateAllAbilites()
        {
            powerUpDisplay.gameObject.SetActive(false);
            playerStats.PlayerMana = 0f;
            TrackingPowerUpActive = false;

        }

    }
}