using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to control the music source in Unity
	/// </summary>
	public class AudioMusicController : MonoBehaviour 
	{
        private void Awake()
        {
			DontDestroyOnLoad(this.gameObject);
        }

    }
}