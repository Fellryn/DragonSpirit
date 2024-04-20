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
        TextMeshProUGUI livesText;
        [SerializeField]
        TextMeshProUGUI manaText;
        [SerializeField]
        RectTransform healthMask;

        private Vector2 healthMaskOriginalPositions;
        private Vector2 healthMaskOriginalDimensions;



        [SerializeField]
        SceneNavigation sceneNavigation;
        [SerializeField]
        string gameOverSceneName = "GameOver";

        [SerializeField]
        Transform cachedModel;




        private void OnEnable()
        {
            EnemyBase.onKill += AddScore;
            gameTickSystem.OnTickWhole.AddListener(delegate { PlayerGainMana(1f); }) ;

            healthMaskOriginalPositions = healthMask.anchoredPosition;
            healthMaskOriginalDimensions = healthMask.sizeDelta;
        }


        private void OnDisable()
        {
            EnemyBase.onKill -= AddScore;
            gameTickSystem.OnTickWhole.RemoveListener(delegate { PlayerGainMana(1f); });
        }

        private void Start()
        {
            PlayerLife = 10;
            UpdateLivesText();
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
            livesText.text = $"Lives: {PlayerLife.ToString()}";

            healthMask.anchoredPosition = new Vector2(healthMaskOriginalPositions.x, (healthMaskOriginalPositions.y) * (PlayerLife / maxLives));
            //healthMask.sizeDelta = new Vector2(healthMaskOriginalDimensions.x, ((healthMaskOriginalDimensions.y) * (PlayerLife / maxLives)) * 2f);
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

        private void UpdateManaText()
        {
            manaText.text = PlayerMana.ToString();
        }


    }
}