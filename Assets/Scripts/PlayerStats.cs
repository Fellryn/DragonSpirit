using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
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
        [SerializeField] PlayerShaderController playerShaderController;

        public int PlayerScore { get; private set; }
        public int PlayerLife { get; set; }
        public bool GodmodeActive { get; set; }
        public float PlayerMana { get; set; }


        [SerializeField]
        float maxMana = 10f;
        public int maxLives = 10;

        [SerializeField]
        TextMeshProUGUI scoreText;
        [SerializeField]
        RectTransform healthMask;
        [SerializeField]
        RectTransform manaMask;

        [SerializeField]
        SceneNavigation sceneNavigation;
        [SerializeField]
        string gameOverSceneName = "GameOver";

        [SerializeField]
        Transform cachedModel;

        private Vector2 healthMaskOriginalPositions;
        private Vector2 healthMaskOriginalDimensions;

        private Vector2 manaMaskOriginalPositions;
        private Vector2 manaMaskOriginalDimensions;




        private void OnEnable()
        {
            EnemyBase.onKill += AddScore;
            //gameTickSystem.OnTickWhole.AddListener(delegate { PlayerGainMana(1f); }) ;

            healthMaskOriginalPositions = healthMask.anchoredPosition;
            healthMaskOriginalDimensions = healthMask.sizeDelta;

            manaMaskOriginalPositions = manaMask.anchoredPosition;
            manaMaskOriginalDimensions = manaMask.sizeDelta;
        }


        private void OnDisable()
        {
            EnemyBase.onKill -= AddScore;
            //gameTickSystem.OnTickWhole.RemoveListener(delegate { PlayerGainMana(1f); });
        }

        private void Start()
        {
            PlayerLife = 10;
            UpdateLivesText();
            UpdateManaText();
        }


        public void AddScore(int score)
        {
			PlayerScore += score;
            UpdateScoreText();
            UpdateManaText();
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

            scoreText.text = $"Score: {PlayerScore.ToString()}";
        }


        private void UpdateLivesText()
        {
            //livesText.text = $"Lives: {PlayerLife.ToString()}";

            float newHealthYPosition = (healthMaskOriginalPositions.y * ((float)PlayerLife / maxLives)) + (11f - (11f * ((float)PlayerLife / maxLives)));
            Vector2 newHealthSizeDelta = new Vector2(healthMaskOriginalDimensions.x, healthMaskOriginalDimensions.y * ((float)PlayerLife / maxLives));

            DOTween.Complete(0);
            DOTween.Complete(1);
            healthMask.DOAnchorPosY(newHealthYPosition, 0.2f).SetId(0);
            healthMask.DOSizeDelta(newHealthSizeDelta, 0.2f).SetId(1);

            //healthMask.anchoredPosition = new Vector2(healthMaskOriginalPositions.x, healthMaskOriginalPositions.y * ((float)PlayerLife / maxLives));
            //healthMask.sizeDelta = new Vector2(healthMaskOriginalDimensions.x, healthMaskOriginalDimensions.y * ((float)PlayerLife / maxLives));
        }


        private void CheckLifeStatus()
        {
            if (PlayerLife <= 0 && !GodmodeActive)
            {
                gameVars.WonLevel(false);
                sceneNavigation.ChangeScene(gameOverSceneName);
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
            } else
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
            } else
            {
                return PlayerMana / maxMana;
            }
            
        }


        private void UpdateManaText()
        {
            //manaText.text = PlayerMana.ToString();
            float newManaYPosition = (manaMaskOriginalPositions.y * (PlayerMana / maxMana)) + (11f - (11f * (PlayerMana / maxMana)));
            Vector2 newManaSizeDelta = new Vector2(manaMaskOriginalDimensions.x, manaMaskOriginalDimensions.y * (PlayerMana / maxMana));

            DOTween.Complete(2);
            DOTween.Complete(3);
            manaMask.DOAnchorPosY(newManaYPosition, 0.2f).SetId(2);
            manaMask.DOSizeDelta(newManaSizeDelta, 0.2f).SetId(3);

            playerShaderController.TweenEmissionFromMana();

            //manaMask.anchoredPosition = new Vector2(manaMaskOriginalPositions.x, manaMaskOriginalPositions.y * (PlayerMana / maxMana));
            //manaMask.sizeDelta = new Vector2(manaMaskOriginalDimensions.x, manaMaskOriginalDimensions.y * (PlayerMana / maxMana));
        }


    }
}