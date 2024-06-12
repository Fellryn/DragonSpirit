using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using DG.Tweening;
using UnityEngine.UI;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to display and update the bosses health bar in Unity
	/// </summary>
	public class EnemyBossHealthBar : MonoBehaviour 
	{
		[SerializeField]
		Slider bossHealthSlider;
		[SerializeField]
		RectTransform bossHealthSliderTransform;

		[SerializeField]
		float healthBarYMoveTarget = -36f;
		[SerializeField]
		float healthBarMoveDuration = 1f;
		[SerializeField]
		Ease healthBarEasyType = Ease.InOutSine;

		private void Start()
        {
			//ShowHealthBar(41);
        }

        public void ShowHealthBar(int maxHealth)
        {
			bossHealthSlider.maxValue = maxHealth;
			bossHealthSlider.value = maxHealth;

			bossHealthSliderTransform.DOAnchorPos(new Vector2(bossHealthSliderTransform.anchoredPosition.x, healthBarYMoveTarget), healthBarMoveDuration).SetEase(healthBarEasyType);
        }

		public void UpdateHealth(int currentHealth)
        {
			bossHealthSlider.value = currentHealth;

			if (currentHealth <= 0) 
			{
				bossHealthSliderTransform.DOAnchorPos(new Vector2(bossHealthSliderTransform.anchoredPosition.x, Mathf.Abs(healthBarYMoveTarget)), healthBarMoveDuration).SetEase(healthBarEasyType);
				this.enabled = false;
			}

        }


	}
}