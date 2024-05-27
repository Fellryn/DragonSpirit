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
        //[SerializeField] PlayerStats playerStats;

        [Header("Emission Tweening")]
        [SerializeField] float maxEmissionValue = 50;
        [SerializeField] float minEmissionValue = 0;
        [SerializeField] bool useStartAnimation = true;
        [SerializeField] float startAnimationDuration = 2f;
        [SerializeField] Transform tweenObject;

        private bool emissionCurrentlyTweening = false;
        [SerializeField] private bool startAnimationComplete = false;

        private void Start()
        {
            PlayerStats.OnManaChanged += TweenEmissionFromMana;

            if (useStartAnimation)
            {
                TweenEmissionFromMana(1);
            }
        }

        private void Update()
        {
            if (emissionCurrentlyTweening) cachedMaterial.SetFloat("_EmissionMultiplier", tweenObject.position.x);
        }


        private void OnDestroy()
        {
            DOTween.Kill(tweenObject);
            PlayerStats.OnManaChanged -= TweenEmissionFromMana;
        }


        public void TweenEmissionFromMana(float manaPercentage)
        {
            float emissionLevel = (manaPercentage * (maxEmissionValue - minEmissionValue)) + minEmissionValue;

            TweenEmissionToValue(emissionLevel, useRelativeDuration: true);
        }

        /// <summary>
        /// Max emission value is 250. Set whatever value you want the emission to be tweened to.
        /// </summary>
        public void TweenEmissionToValue(float value, float duration = 1f, bool useRelativeDuration = false)
        {
            DOTween.Kill(tweenObject);

            float modifiedDuration = duration;

            if (useRelativeDuration)
            {
                modifiedDuration = (Mathf.Abs(tweenObject.position.x - value)) * 0.01f;

                if (modifiedDuration <= 0.1f) modifiedDuration = 0.1f;
            }

            if (!startAnimationComplete)
            {
                //tweenObject.DOMoveX(value, startAnimationDuration / 2).OnComplete(() => tweenObject.DOMoveX(minEmissionValue, startAnimationDuration / 2).OnComplete(() => startAnimationComplete = true));
                Sequence startupSequence = DOTween.Sequence();

                startupSequence
                    .Append(tweenObject.DOMoveX(value, startAnimationDuration / 2))
                    .Append(tweenObject.DOMoveX(minEmissionValue, startAnimationDuration / 2))
                    .OnComplete(EndStartupAnimation);
            } else
            {
                tweenObject.DOMoveX(value, modifiedDuration).OnComplete(() => emissionCurrentlyTweening = false);
            }

            emissionCurrentlyTweening = true;

            
        }

        private void EndStartupAnimation()
        {
            startAnimationComplete = true;
            emissionCurrentlyTweening = false;
        }

        //private void EndEmissionTween()
        //{
        //    emissionCurrentlyTweening = false;
        //}
            
        

    }
}