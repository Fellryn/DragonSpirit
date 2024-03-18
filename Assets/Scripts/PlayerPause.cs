using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to pause the game with a game manager in Unity
	/// </summary>
	public class PlayerPause : MonoBehaviour 
	{
        [SerializeField] RectTransform pausePanel;
        [SerializeField] InputActionReference menuKeyPlayer;
        [SerializeField] InputActionReference menuKeyUI;

        private void Awake()
        {
			GameManager.OnGameStateChanged += OnGameManagerStateChanged;
            menuKeyPlayer.action.performed += HardwareMenuKeyPressed;
            menuKeyUI.action.performed += HardwareMenuKeyPressed;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnGameManagerStateChanged;
            menuKeyPlayer.action.performed -= HardwareMenuKeyPressed;
            menuKeyUI.action.performed -= HardwareMenuKeyPressed;
        }

        private void HardwareMenuKeyPressed(InputAction.CallbackContext obj)
        {
            MenuKeyPressed();
        }

        public void SoftwareMenuKeyPressed()
        {
            MenuKeyPressed();
        }

        private void MenuKeyPressed()
        {
            if (GameManager.Instance.State == GameManager.GameStates.Paused)
            {
                GameManager.Instance.ChangeGameState(GameManager.GameStates.Running);
            }
            else
            {
                GameManager.Instance.ChangeGameState(GameManager.GameStates.Paused);
            }
        }

        private void OnGameManagerStateChanged(GameManager.GameStates state)
        {
            pausePanel.gameObject.SetActive(state == GameManager.GameStates.Paused); 
        }
		
	}
}