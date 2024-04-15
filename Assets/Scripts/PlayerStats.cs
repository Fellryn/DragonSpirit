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

        public int PlayerScore { get; private set; }
        public int PlayerLife { get; set; }
        public bool GodmodeActive { get; set; }
        public int maxLives = 10;

        public bool powerupAttackTracking = false;

        [SerializeField]
        TextMeshProUGUI scoreText;
        [SerializeField]
        TextMeshProUGUI livesText;

        [SerializeField]
        SceneNavigation sceneNavigation;
        [SerializeField]
        string gameOverSceneName = "GameOver";

        [SerializeField]
        Transform cachedModel;




        private void OnEnable()
        {
            EnemyBase.onKill += AddScore;
        }


        private void OnDisable()
        {
            EnemyBase.onKill -= AddScore;
        }

        private void Start()
        {
            PlayerLife = 10;
            UpdateLivesText();
        }


        public void AddScore(int score)
        {
			PlayerScore += score;
            DoScoreChecks();
            UpdateScoreText();
        }

        
        public void RemoveScore(int score)
        {
            PlayerScore -= score;
            DoScoreChecks();
            UpdateScoreText();
        }

        private void DoScoreChecks()
        {
            if (PlayerScore >= 10)
            {
                powerupAttackTracking = true;
            }

            if (PlayerScore <= 0)
            {
                powerupAttackTracking = false;
            }
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


    }
}