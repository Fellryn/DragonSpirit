using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using DG.Tweening;
using TMPro;

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
        [SerializeField] RectTransform powerUpText;
        [SerializeField] TextMeshProUGUI powerUpActualText;

        [SerializeField] RectTransform powerUpTextMultiplayer;
        [SerializeField] TextMeshProUGUI powerUpActualTextMultiplayer;

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

        [Header("PowerUp Text")]
        [SerializeField] Vector2 textOffscreenPos;
        [SerializeField] Vector2 textOnscreenPos;
        [SerializeField] float totalTweenTime = 6f;
        [SerializeField] Ease textEaseInType = Ease.OutQuint;
        [SerializeField] Ease textEaseOutType = Ease.InQuint;
        [SerializeField] float shakeStrength = 5f;
        [SerializeField] int shakeVibrato = 4;
        [SerializeField] Ease shakeEaseType = Ease.InOutQuad;
        Sequence powerUpTextSequence;
        Sequence powerUpTextMultiplayerSequence;



        private void OnEnable()
        {
            MultiShotActive = false;
            gameTickSystem.OnEveryHalfTick.AddListener(PowerUpTimingChecks);
        }


        private void OnDisable()
        {
            gameTickSystem.OnEveryHalfTick.RemoveListener(PowerUpTimingChecks);
        }

        private void Start()
        {

            powerUpText.anchoredPosition = new Vector2(powerUpText.anchoredPosition.x, textOffscreenPos.y);
            powerUpTextMultiplayer.anchoredPosition = new Vector2(powerUpTextMultiplayer.anchoredPosition.x, textOffscreenPos.y);

            powerUpTextSequence = DOTween.Sequence();
            powerUpTextSequence
                .Append(powerUpText.DOAnchorPos(new Vector2(powerUpText.anchoredPosition.x, textOnscreenPos.y), totalTweenTime * 0.25f).SetEase(textEaseInType))
                .AppendInterval(totalTweenTime * 0.5f)
                .Join(powerUpText.DOShakeAnchorPos(totalTweenTime * 0.75f, shakeStrength, shakeVibrato, fadeOut: false).SetEase(shakeEaseType))
                .Append(powerUpText.DOAnchorPos(new Vector2(powerUpText.anchoredPosition.x, textOffscreenPos.y), totalTweenTime * 0.25f).SetEase(textEaseOutType))
                .SetAutoKill(false);

            powerUpTextMultiplayerSequence = DOTween.Sequence();
            powerUpTextMultiplayerSequence
                .Append(powerUpTextMultiplayer.DOAnchorPos(new Vector2(powerUpTextMultiplayer.anchoredPosition.x, textOnscreenPos.y), totalTweenTime * 0.25f).SetEase(textEaseInType))
                .AppendInterval(totalTweenTime * 0.5f)
                .Join(powerUpTextMultiplayer.DOShakeAnchorPos(totalTweenTime * 0.75f, shakeStrength, shakeVibrato, fadeOut: false).SetEase(shakeEaseType))
                .Append(powerUpTextMultiplayer.DOAnchorPos(new Vector2(powerUpTextMultiplayer.anchoredPosition.x, textOffscreenPos.y), totalTweenTime * 0.25f).SetEase(textEaseOutType))
                .SetAutoKill(false);

            powerUpTextSequence.Complete();
            powerUpTextMultiplayerSequence.Complete();
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
                            DisplayPowerup("Healed +5", Color.green);
                            //Instantiate(lifePowerUpPlayerEffect, transform);
                            break;
                        case multiShotPowerUpName:
                            ChangeMultiShotLevel(1);
                            DisplayPowerup("MultiShot", Color.yellow);
                            //Instantiate(multiShotPowerUpPlayerEffect, transform);
                            break;
                        case trackingPowerUpName:
                            ActivateTrackingAbility();
                            DisplayPowerup("Tracking Shot", Color.cyan);
                            //Instantiate(trackingPowerUpPlayerEffect, transform);
                            break;
                        default:
                            break;
                    }

                    powerUpBase.PassPlayerTransform(transform);
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
                }
                else
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
            }
            else
            {
                multiShotTimer = multiShotActiveTime;
            }
        }

        public int UseMultiShot()
        {
            if (CurrentMultiShotLevel == 1)
            {
                return 3;
            }
            else
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
            }
            else
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

        private void DisplayPowerup(string powerUpName, Color powerUpColour)
        {

            powerUpActualText.color = powerUpColour;
            powerUpActualText.text = powerUpName;

            powerUpActualTextMultiplayer.color = powerUpColour;
            powerUpActualTextMultiplayer.text = powerUpName;

            powerUpTextSequence.Complete();
            powerUpTextMultiplayerSequence.Complete();

            powerUpTextSequence.Restart();
            powerUpTextMultiplayerSequence.Restart();
        }

    }
}