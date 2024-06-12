using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to play and handle sound in Unity
	/// </summary>
	public class SoundHandler : MonoBehaviour 
	{
		[SerializeField]
		AudioSource audioSource;

		[SerializeField]
		AudioClip[] audioClips;

        private void Start()
        {
            if (audioSource == null)
            {
				audioSource = GameObject.FindWithTag("SoundEffects").GetComponent<AudioSource>();
            }
        }

        public void PlaySound(int index, float volume = 1f, float chanceToPlay = 1f)
        {
			if (Random.Range(0f, 1f) <= chanceToPlay)
			{
				audioSource.PlayOneShot(audioClips[index], volume);
			}
        }

    }
}