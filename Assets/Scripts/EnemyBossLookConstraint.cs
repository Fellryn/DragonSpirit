using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using KurtSingle;
using DG.Tweening;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to control the constraint that lets boss look at player in Unity
	/// It uses a LookAt constraint, which the weight of is set by tweening an object between 0 and 1 
	/// </summary>
	public class EnemyBossLookConstraint : MonoBehaviour 
	{
		[SerializeField] LookAtConstraint headLookConstraint;
		[SerializeField] Transform bossLookTransform;
		[SerializeField] EnemyBoss enemyBoss;
		[SerializeField] EnemyBossAnimation enemyBossAnimation;

		public bool lookingAtPlayer = false;

		[SerializeField] float totalTurnTime = 1.25f;

        private void Start()
        {
			
		}

		public void LookAtPlayer()
        {
			bossLookTransform.DOScale(1f, totalTurnTime * 0.5f).SetEase(Ease.InOutCubic).OnComplete(LookAwayFromPlayer);
		}

		private void LookAwayFromPlayer()
        {
			bossLookTransform.DOScale(0f, totalTurnTime * 0.5f).SetEase(Ease.InOutCubic);
		}

        private void Update()
        {
			if (headLookConstraint.weight != bossLookTransform.localScale.y)
			{
				headLookConstraint.weight = bossLookTransform.transform.localScale.y;
			}

			if (headLookConstraint.weight >= 0.75f)
			{
				lookingAtPlayer = true;
			} else 
			{
				lookingAtPlayer = false;
            }

			//if (enemyBossNew.)
		}

    }
}