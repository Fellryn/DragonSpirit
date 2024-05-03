using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KurtSingle;
using UnityEngine.Audio;
using System;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to change sound effects and music volume from sliders in Unity
	/// </summary>
	public class SettingsControl : MonoBehaviour 
	{
		[SerializeField] Slider musicSlider;
        [SerializeField] Slider soundFXSlider;

        [SerializeField] AudioMixer audioMixer;
        [SerializeField] string musicGroupName = "MusicVolume";
        [SerializeField] string soundFXGroupName = "SoundEffectsVolume";

        private void OnEnable()
        {
            musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            soundFXSlider.onValueChanged.AddListener(ChangeSoundFXVolume);
        }


        private void OnDisable()
        {
            musicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
            soundFXSlider.onValueChanged.RemoveListener(ChangeSoundFXVolume);

        }


        private void ChangeMusicVolume(float value)
        {
            //audioMixer.SetFloat(musicGroupName, musicSlider.value);

            var dbVolume = Mathf.Log10(value) * 20;
            if (value == 0.0f)
            {
                audioMixer.SetFloat(musicGroupName, -80f);
            } else
            {
                audioMixer.SetFloat(musicGroupName, dbVolume);
            }
        }


        private void ChangeSoundFXVolume(float value)
        {

            var dbVolume = Mathf.Log10(value) * 20;
            if (value == 0.0f)
            {
                audioMixer.SetFloat(soundFXGroupName, -80f);
            }
            else
            {
                audioMixer.SetFloat(soundFXGroupName, dbVolume);
            }
        }

    }
}