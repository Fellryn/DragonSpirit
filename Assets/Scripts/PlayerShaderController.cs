using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using DG.Tweening;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to control the players shader in Unity
    /// Useful for changing emission of player when powered up, using abilities, etc.
	/// </summary>
	public class PlayerShaderController : MonoBehaviour 
	{
		[SerializeField] Material cachedMaterial;
        [SerializeField] PlayerStats playerStats;

        [Header("Emission Tweening")]
        [SerializeField] float maxEmissionValue = 50;
        [SerializeField] float minEmissionValue = 0;
        [SerializeField] Transform tweenObject;
        private bool emissionCurrentlyTweening = false;

        private void Update()
        {
            if (emissionCurrentlyTweening) cachedMaterial.SetFloat("_EmissionMultiplier", tweenObject.position.x);
        }


        private void OnDestroy()
        {
            DOTween.Kill(tweenObject);
        }


        public void TweenEmissionFromMana()
        {
            float emissionLevel = playerStats.GetManaPercentage() * (maxEmissionValue - minEmissionValue);

            TweenEmissionToValue(emissionLevel, useRelativeDuration: true);
        }

        /// <summary>
        /// Max emission value is 50. Set whatever value you want the emission to be tweened to.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="duration"></param>
        /// <param name="useRelativeDuration"></param>
        public void TweenEmissionToValue(float value, float duration = 1f, bool useRelativeDuration = false)
        {
            DOTween.Kill(tweenObject);

            float modifiedDuration = duration;

            if (useRelativeDuration)
            {
                modifiedDuration = (Mathf.Abs(tweenObject.position.x - value)) * 0.01f;

                if (modifiedDuration <= 0.1f) modifiedDuration = 0.1f;
            }

            tweenObject.DOMoveX(value, modifiedDuration).OnComplete(() => emissionCurrentlyTweening = false);

            emissionCurrentlyTweening = true;

            
        }

        //private void EndEmissionTween()
        //{
        //    emissionCurrentlyTweening = false;
        //}
            
        

    }
}