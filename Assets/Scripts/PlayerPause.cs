using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
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
        [SerializeField] RectTransform cheatPanel;
        [SerializeField] bool cheatPanelLastState = false;
        [SerializeField] InputActionReference menuKeyPlayer;
        [SerializeField] InputActionReference menuKeyUI;

        PlayerInput playerInput;
        InputAction cancelActionPlayer;
        InputAction cancelActionUI;


        private void Awake()
        {
			GameManager.OnGameStateChanged += OnGameManagerStateChanged;
   //         menuKeyPlayer.action.performed += HardwareMenuKeyPressed;
   //         menuKeyUI.action.performed += HardwareMenuKeyPressed;

            playerInput = GetComponent<PlayerInput>();

            cancelActionPlayer = playerInput.actions.FindAction("Cancel");
            cancelActionUI = playerInput.actions.FindAction("Cancel");

            cancelActionPlayer.performed += HardwareMenuKeyPressed;
            cancelActionUI.performed += HardwareMenuKeyPressed;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= OnGameManagerStateChanged;
            //menuKeyPlayer.action.performed -= HardwareMenuKeyPressed;
            //menuKeyUI.action.performed -= HardwareMenuKeyPressed;

            cancelActionPlayer.performed -= HardwareMenuKeyPressed;
            cancelActionUI.performed -= HardwareMenuKeyPressed;

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
            
            if (pausePanel.gameObject.activeSelf)
            {
                cheatPanel.gameObject.SetActive(cheatPanelLastState);
            } else
            {
                cheatPanelLastState = cheatPanel.gameObject.activeSelf;
                cheatPanel.gameObject.SetActive(false);
            }
        }
		
	}
}