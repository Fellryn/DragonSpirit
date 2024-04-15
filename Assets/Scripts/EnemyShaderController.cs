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

        }

    }
}