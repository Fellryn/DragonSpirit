using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KurtSingle;
using TMPro;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to store player stats (score, life) 
	/// and provide ways for events to be called for gaining/losing score/life in Unity
	/// </summary>
	public class PlayerStats : MonoBehaviour 
	{
        public int PlayerScore { get; private set; }
        public int PlayerLife { get; set; }
        [SerializeField]
        TextMeshProUGUI scoreText;
        [SerializeField]
        TextMeshProUGUI livesText;

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
            UpdateScoreText();
        }

        public void PlayerTakeDamage(int damage)
        {
            PlayerLife -= damage;
            UpdateLivesText();
        }

        private void UpdateScoreText()
        {
            scoreText.text = $"Score: {PlayerScore.ToString()}";
        }

        private void UpdateLivesText()
        {
            livesText.text = $"Lives: {PlayerLife.ToString()}";
        }

    }
}