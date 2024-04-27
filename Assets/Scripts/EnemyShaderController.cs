using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to control shaders on enemy units in Unity
	/// Shaders that dissolve the enemy away upon death, shader animations when attacking etc.
	/// </summary>
	public class EnemyShaderController : MonoBehaviour 
	{
		[SerializeField]
		Renderer cachedRenderer;

		[Header("Dissolve Animation")]
		[SerializeField]
		bool useDissolveAnimation = false;
		public bool BegunDissolveAnimation { get; private set; }
		private float timer;

		[SerializeField]
		float maxAlphaCutoff = 2f;
		[SerializeField]
		float minumumAlphaCutoff = -1.5f;

		[SerializeField]
		float timeMod = 0.33f;


		[Header("Glow Cycle")]
		[SerializeField] bool useGlowCycleAnimation = false;
		[SerializeField] float maxGlowMultiplier = 4f;
		[SerializeField] float minGlowMultiplier = 1f;
		[SerializeField] float glowTime = 3f;

		private float glowTimer;
		private float glowTarget;
		private bool glowDecreasing = false;



		public void BeginDissolveAnimation()
        {
			if (useDissolveAnimation)
			{
				BegunDissolveAnimation = true;
				//cachedRenderer.material.shader = Shader.Find("Dissolve");
			}
        }

        private void Update()
        {
			if (BegunDissolveAnimation)
            {
				timer += Time.deltaTime * timeMod;
				cachedRenderer.material.SetFloat("_Cutoff", Mathf.Lerp(maxAlphaCutoff, minumumAlphaCutoff, timer));
			}

			if (useGlowCycleAnimation)
            {
				float incrementBy = ((maxGlowMultiplier - minGlowMultiplier) / glowTime) * 0.01f;

				if (glowDecreasing)
                {
					glowTarget -= incrementBy;
                } else
                {
					glowTarget += incrementBy;
                }

				if (glowTarget > maxGlowMultiplier && !glowDecreasing)
                {
					glowDecreasing = true;
                } else if (glowTarget <= minGlowMultiplier)
                {
					glowDecreasing = false;
                }

				cachedRenderer.sharedMaterial.SetFloat("_EmissionMultiplier", glowTarget);
            }

        }

    }
}