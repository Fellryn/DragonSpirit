using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to simply pass a Unity event to it's parent object in Unity
	/// </summary>
	public class AnimationEventPasser : MonoBehaviour 
	{
		private EnemyAnimation enemyAnimation;
		private EnemyBossAnimation enemyBossAnimation;
        [SerializeField] SoundHandler soundHandler;

        private void Start()
        {
            if (transform.parent.TryGetComponent(out EnemyAnimation enemyAnimationRef))
            {
                enemyAnimation = enemyAnimationRef;
            }

            if (transform.parent.TryGetComponent(out EnemyBossAnimation enemyBossAnimationRef))
            {
                enemyBossAnimation = enemyBossAnimationRef;
            }
        }

        public void AttackCompleted()
        {            
            if (enemyAnimation != null) enemyAnimation.AttackCompleted();
            if (enemyBossAnimation != null) enemyBossAnimation.AttackCompleted();
        }

        public void WingFlap()
        {
            if (soundHandler != null) soundHandler.PlaySound(3);
        }

		public void BossHeadMovementCompleted()
        {
            enemyBossAnimation.BossHeadMovementCompleted();
        }

    }
}