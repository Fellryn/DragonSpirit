using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using KurtSingle;


namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to make and use a gamemanager state machine in Unity
    /// </summary>

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance;
        [SerializeField] PlayerInput playerInput;
        [SerializeField] EventSystem eventSystem;
        [SerializeField] GameObject firstMenuButton;
        public GameStates State;

        public static event Action<GameStates> OnGameStateChanged;

        private void Awake()
        {
            Instance = this;
            
        }

        private void Start()
        {
            ChangeGameState(GameStates.Running);
        }

        public enum GameStates
        {
            Running,
            Paused
        }



        public void ChangeGameState(GameStates newState)
        {
            State = newState;

            switch (newState)
            {
                case GameStates.Running:
                    DoRunningSettings();
                    break;
                case GameStates.Paused:
                    DoPausedSettings();
                    break;
                    
            }

            OnGameStateChanged?.Invoke(newState);
        }

        private void DoRunningSettings()
        {
            playerInput.SwitchCurrentActionMap("Player");
            Time.timeScale = 1f;
            
        }

        private void DoPausedSettings()
        {
            playerInput.SwitchCurrentActionMap("UI");
            eventSystem.SetSelectedGameObject(firstMenuButton);
            Time.timeScale = 0f;
        }
    }

}
