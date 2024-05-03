
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Audio;


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

        [SerializeField] GameObject originalFirstSelectedObject;
        [SerializeField] GameObject settingsFirstSelectedObject;

        private void OnEnable()
        {
            musicSlider.onValueChanged.AddListener(ChangeMusicVolume);
            soundFXSlider.onValueChanged.AddListener(ChangeSoundFXVolume);

            EventSystemFirstSelectedObject(true);

            SetSliders();
        }


        private void OnDisable()
        {
            musicSlider.onValueChanged.RemoveListener(ChangeMusicVolume);
            soundFXSlider.onValueChanged.RemoveListener(ChangeSoundFXVolume);

            EventSystemFirstSelectedObject(false);
        }


        private void ChangeMusicVolume(float value)
        {

            var dbVolume = Mathf.Log10(value) * 20;
            if (value == 0.0f)
            {
                audioMixer.SetFloat(musicGroupName, -80f);
            }
            else
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


        public void EventSystemFirstSelectedObject(bool focusSettings)
        {
            if (focusSettings)
            {
                if (EventSystem.current != null)
                {
                    originalFirstSelectedObject = EventSystem.current.firstSelectedGameObject;
                    EventSystem.current.firstSelectedGameObject = settingsFirstSelectedObject;
                }
            }
            else
            {
                if (EventSystem.current != null)
                {
                    EventSystem.current.firstSelectedGameObject = originalFirstSelectedObject;
                }
            }
        }


        private void SetSliders()
        {

            if (audioMixer.GetFloat(musicGroupName, out float valueMusic))
            {
                musicSlider.value = Mathf.Pow(10f, valueMusic / 20f);
            }

            if (audioMixer.GetFloat(soundFXGroupName, out float valueSFX))
            {
                soundFXSlider.value = Mathf.Pow(10f, valueSFX / 20f);
            }
        }

    }
}