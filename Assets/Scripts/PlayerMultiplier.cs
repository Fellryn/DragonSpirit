using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using System.Threading;
using TMPro;
using DG.Tweening;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to track and add points based off player multiplier in Unity
	/// </summary>
	public class PlayerMultiplier : MonoBehaviour 
	{
		[SerializeField]
		PlayerStats playerStats;
		[SerializeField]
		GameTickSystem gameTickSystem;

        public int MultiplierLevel { get; private set; }

		[SerializeField]
		int maxMultiplierLevel = 10;
		[SerializeField]
		float timeBetweenHitsToIncreaseMultiplier = 3f;
		[SerializeField]
		int numberOfEnemiesToIncreaseMultiplier = 5;

        public int numberOfEnemiesKilledSinceLastHit { get; private set; }
        private float timer = 0f;

		[Header("References")]
		[SerializeField]
		private TextMeshProUGUI multiplierText;
		[SerializeField]
		private TextMeshProUGUI multiplierTextMultiplayer;



		private void Start()
        {
            EnemyBase.onKill += AddKillCount;
			PlayerStats.OnDamageTaken += PlayerHit;
			gameTickSystem.OnTickWhole.AddListener(DoChecks);
			MultiplierLevel = 1;
			SetMultiplierText();
        }

        private void OnDestroy()
        {
            EnemyBase.onKill -= AddKillCount;
			PlayerStats.OnDamageTaken -= PlayerHit;
			gameTickSystem.OnTickWhole.RemoveListener(DoChecks);
		}

        public void DoChecks()
		{
			TimerCheck();
		}

        private void TimerCheck()
		{
			timer++;
			TryIncreaseMultiplier();
		}

		public void AddKillCount(int Score, bool lastHitByPlayerOne)
		{
			if (playerStats.isPlayerOne == lastHitByPlayerOne)
            {
				numberOfEnemiesKilledSinceLastHit++;
			}
		}

		public void PlayerHit(bool playerOneHit)
		{
			if (playerOneHit == playerStats.isPlayerOne)
			{
				numberOfEnemiesKilledSinceLastHit = 0;
				MultiplierLevel = 1;
				DoMultiplierTween();
				SetMultiplierText();
			}
        }

		public void TryIncreaseMultiplier()
		{
			if (timer >= timeBetweenHitsToIncreaseMultiplier && numberOfEnemiesKilledSinceLastHit >= numberOfEnemiesToIncreaseMultiplier && MultiplierLevel < maxMultiplierLevel)
			{
				numberOfEnemiesKilledSinceLastHit -= numberOfEnemiesToIncreaseMultiplier;
				timer = 0f;
				DoMultiplierTween();
				SetMultiplierText();
                MultiplierLevel++;
            }
		}

		private void DoMultiplierTween()
		{
			
		}

		private void SetMultiplierText()
		{
			multiplierText.text = $"{MultiplierLevel}x";
			multiplierTextMultiplayer.text = $"{MultiplierLevel}x";
		}

    }
}