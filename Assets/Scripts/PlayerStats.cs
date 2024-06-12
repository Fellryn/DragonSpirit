using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using KurtSingle;
using UlianaKutsenko;
using TMPro;
using DG.Tweening;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to store player stats (score, life, active powerups) 
    /// and provide ways for events to be called for gaining/losing score/life in Unity
    /// </summary>
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] GameVars gameVars;
        [SerializeField] GameTickSystem gameTickSystem;
        [SerializeField] PlayerPowerups playerPowerUps;
        [SerializeField] PlayerMultiplier playerMultiplier;
        [SerializeField] Button godmodeButton;
        [SerializeField] SoundHandler soundHandler;

        public int PlayerScore { get; private set; }
        public int PlayerLife { get; set; }
        public bool GodmodeActive { get; set; }
        public float PlayerMana { get; set; }
        public bool isPlayerOne = true;

        [SerializeField]
        float maxMana = 10f;
        public int maxLives = 10;

        [SerializeField]
        TextMeshProUGUI scoreText;
        [SerializeField]
        DisplayScorePosition displayScorePosition;
        [SerializeField]
        DisplayScorePosition displayScorePositionMultiplayer;
        [SerializeField]
        RectTransform healthMask;
        [SerializeField]
        RectTransform manaMask;

        [Header("Multiplayer")]
        [SerializeField]
        TextMeshProUGUI scoreTextMultiplayer;
        [SerializeField]
        RectTransform healthMaskMultiplayer;
        [SerializeField]
        RectTransform manaMaskMultiplayer;

        [SerializeField]
        Transform cachedModel;


        private static Vector2 healthMaskOriginalPositions;
        private static Vector2 healthMaskOriginalDimensions;

        private static Vector2 manaMaskOriginalPositions;
        private static Vector2 manaMaskOriginalDimensions;

        public delegate void ManaChanged(float ManaPercentage);
        public static event ManaChanged OnManaChanged;
        private bool firstInvokeIgnored = false;

        public delegate void DamageTaken(bool playerOneTookDamage);
        public static event DamageTaken OnDamageTaken;


        private void Start()
        {
            EnemyBase.onKill += AddScore;
            EnemyBoss.onBossHit += AddScore;
            godmodeButton.onClick.AddListener(ToggleGodmode);
            //gameTickSystem.OnTickWhole.AddListener(delegate { PlayerGainMana(1f); }) ;

            if (isPlayerOne)
            {
                healthMaskOriginalPositions = healthMask.anchoredPosition;
                healthMaskOriginalDimensions = healthMask.sizeDelta;

                manaMaskOriginalPositions = manaMask.anchoredPosition;
                manaMaskOriginalDimensions = manaMask.sizeDelta;
            }

            PlayerLife = 10;
            UpdateLivesText();
            UpdateManaText();

            gameVars.SetPlayerOneScore(0);
            gameVars.SetPlayerTwoScore(0);
            gameVars.WonLevel(false);


            //gameTickSystem.OnEveryHalfTick.AddListener()
        }


        private void OnDisable()
        {
            //gameTickSystem.OnTickWhole.RemoveListener(delegate { PlayerGainMana(1f); });
            //gameTickSystem.OnEveryHalfTick.RemoveListener()

            if (isPlayerOne)
            {
                gameVars.SetPlayerOneScore(PlayerScore);
            }
            else
            {
                gameVars.SetPlayerTwoScore(PlayerScore);
            }
        }


        private void OnDestroy()
        {
            EnemyBase.onKill -= AddScore;
            EnemyBoss.onBossHit -= AddScore;
            godmodeButton.onClick.RemoveListener(ToggleGodmode);
            DOTween.Kill(healthMask);
            DOTween.Kill(manaMask);


        }


        public void AddScore(int score, bool lastHitByPlayerOne)
        {
            if (lastHitByPlayerOne == isPlayerOne)
            {
                PlayerScore += (score * playerMultiplier.MultiplierLevel);
                displayScorePosition.SetScore(PlayerScore);
                displayScorePositionMultiplayer.SetScore(PlayerScore);
                UpdateScoreText();
                //UpdateManaText();
            }
        }


        public void RemoveScore(int score)
        {
            PlayerScore -= score;
            UpdateScoreText();
        }


        public void PlayerTakeDamage(int damage)
        {
            PlayerLife -= damage;
            PlayerLife = Mathf.Clamp(PlayerLife, 0, maxLives);
            DamageVisualEffect();
            UpdateLivesText();
            CheckLifeStatus();
            OnDamageTaken?.Invoke(isPlayerOne);

            int index = UnityEngine.Random.Range(1, 3);
            soundHandler.PlaySound(index);
        }


        public void PlayerHeal(int amount)
        {
            PlayerLife += amount;
            PlayerLife = Mathf.Clamp(PlayerLife, 0, maxLives);
            UpdateLivesText();
        }


        private void UpdateScoreText()
        {
            PlayerScore = Mathf.Clamp(PlayerScore, 0, int.MaxValue);

            scoreText.text = PlayerScore.ToString();

            scoreTextMultiplayer.text = PlayerScore.ToString();
        }


        private void UpdateLivesText()
        {
            //livesText.text = $"Lives: {PlayerLife.ToString()}";

            float newHealthYPosition = (healthMaskOriginalPositions.y * ((float)PlayerLife / maxLives)) + (20f - (20f * ((float)PlayerLife / maxLives)));
            Vector2 newHealthSizeDelta = new Vector2(healthMaskOriginalDimensions.x, healthMaskOriginalDimensions.y * ((float)PlayerLife / maxLives));

            DOTween.Complete(0);
            DOTween.Complete(1);
            healthMask.DOAnchorPosY(newHealthYPosition, 0.2f).SetId(0);
            healthMask.DOSizeDelta(newHealthSizeDelta, 0.2f).SetId(1);

            healthMaskMultiplayer.DOAnchorPosY(newHealthYPosition, 0.2f).SetId(0);
            healthMaskMultiplayer.DOSizeDelta(newHealthSizeDelta, 0.2f).SetId(1);

            //healthMask.anchoredPosition = new Vector2(healthMaskOriginalPositions.x, healthMaskOriginalPositions.y * ((float)PlayerLife / maxLives));
            //healthMask.sizeDelta = new Vector2(healthMaskOriginalDimensions.x, healthMaskOriginalDimensions.y * ((float)PlayerLife / maxLives));
        }


        private void CheckLifeStatus()
        {
            if (PlayerLife <= 0 && !GodmodeActive)
            {
                //gameVars.WonLevel(false);
                //sceneNavigation.ChangeScene(gameOverSceneName);

                gameObject.SetActive(false);
                soundHandler.PlaySound(0);
            }
        }


        public void ToggleGodmode()
        {
            GodmodeActive = !GodmodeActive;
            if (!GodmodeActive) PlayerHeal(100);
        }


        private void DamageVisualEffect()
        {
            DOTween.Complete(100);
            cachedModel.DOPunchScale(Vector3.one * 0.8f, 0.3f, 1, 0.3f).SetId(100);
        }

        public void PlayerGainMana(float amount)
        {
            PlayerMana += amount;
            PlayerMana = Mathf.Clamp(PlayerMana, 0f, maxMana);
            UpdateManaText();
        }

        public bool PlayerUseMana(float amount)
        {
            if (amount <= PlayerMana)
            {
                PlayerMana -= amount;
                UpdateManaText();
                return true;
            }
            else
            {
                // Do effect for no mana
                return false;
            }
        }

        public float GetManaPercentage()
        {
            if (PlayerMana <= 0f)
            {
                return 0f;
            }
            else
            {
                return PlayerMana / maxMana;
            }
        }


        private void UpdateManaText()
        {
            if (PlayerMana <= 0)
            {
                playerPowerUps.DeactivateAllAbilites();
            }

            //manaText.text = PlayerMana.ToString();
            float newManaYPosition = (manaMaskOriginalPositions.y * (PlayerMana / maxMana)) + (20f - (20f * (PlayerMana / maxMana)));
            Vector2 newManaSizeDelta = new Vector2(manaMaskOriginalDimensions.x, manaMaskOriginalDimensions.y * (PlayerMana / maxMana));

            DOTween.Complete(2);
            DOTween.Complete(3);
            manaMask.DOAnchorPosY(newManaYPosition, 0.2f).SetId(2);
            manaMask.DOSizeDelta(newManaSizeDelta, 0.2f).SetId(3);

            manaMaskMultiplayer.DOAnchorPosY(newManaYPosition, 0.2f).SetId(2);
            manaMaskMultiplayer.DOSizeDelta(newManaSizeDelta, 0.2f).SetId(3);

            if (!firstInvokeIgnored)
            {
                firstInvokeIgnored = true;
            }
            else
            {
                OnManaChanged?.Invoke(GetManaPercentage());
            }


            //manaMask.anchoredPosition = new Vector2(manaMaskOriginalPositions.x, manaMaskOriginalPositions.y * (PlayerMana / maxMana));
            //manaMask.sizeDelta = new Vector2(manaMaskOriginalDimensions.x, manaMaskOriginalDimensions.y * (PlayerMana / maxMana));
        }



    }
}