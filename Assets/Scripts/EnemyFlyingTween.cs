using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using DG.Tweening;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to simply tween an flying enemy in Unity
	/// </summary>
	public class EnemyFlyingTween : MonoBehaviour 
	{
		private Transform modelTransform;
		[SerializeField] float shakeDuration = 1f;
		[SerializeField] float shakeStrength = 0.2f;
		[SerializeField] int shakeVibrato = 1;

        private void Start()
        {
			modelTransform = GetComponentInChildren<MeshRenderer>().GetComponent<Transform>();
			modelTransform.DOShakePosition(shakeDuration, shakeStrength, shakeVibrato).SetLoops(-1, LoopType.Yoyo);
        }

        private void OnDisable()
        {
			modelTransform.DOKill();
        }


    }
}