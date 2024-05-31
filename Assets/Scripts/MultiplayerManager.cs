using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UlianaKutsenko;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to make and manage multiplayer states in Unity
    /// </summary>

    public class MultiplayerManager : MonoBehaviour
    {
        [SerializeField]
        PlayerInputManager playerInputManager;
        [SerializeField]
        GameTickSystem gameTickSystem;
        [SerializeField]
        SceneNavigation sceneNavigation;
        [SerializeField]
        GameVars gameVars;

        [SerializeField]
        GameObject singlePlayerHud;
        [SerializeField]
        GameObject multiPlayerHud;

        [SerializeField]
        GameObject firstPlayer;
        [SerializeField]
        GameObject secondPlayer;

        [SerializeField]
        PlayerStats firstPlayerStats;
        [SerializeField]
        PlayerStats secondPlayerStats;

        [SerializeField]
        float respawnTime = 3f;
        float timer = 0f;

        void Start()
        {
            playerInputManager.onPlayerJoined += PlayerJoined;
            gameTickSystem.OnEveryHalfTick.AddListener(CheckPlayerStatus);
        }

        void OnDestroy()
        {
            playerInputManager.onPlayerJoined -= PlayerJoined;
            gameTickSystem.OnEveryHalfTick.RemoveListener(CheckPlayerStatus);
        }

        void PlayerJoined(PlayerInput playerInput)
        {
            singlePlayerHud.SetActive(false);
            multiPlayerHud.SetActive(true);

            //playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

            secondPlayer = playerInput.gameObject;
            secondPlayerStats = playerInput.GetComponent<PlayerStats>();

            playerInputManager.enabled = false;

            MovePlayerToOtherPlayer(false);
        }



        void CheckPlayerStatus()
        {
            if (secondPlayer != null)
            {

                if (!firstPlayer.activeSelf && !secondPlayer.activeSelf)
                {
                    LoseLevel();
                    return;
                }

                if (!firstPlayer.activeSelf || !secondPlayer.activeSelf)
                {
                    if (timer >= respawnTime)
                    {
                        firstPlayer.SetActive(true);
                        secondPlayer.SetActive(true);
                        timer = 0f;

                        if (firstPlayerStats.PlayerLife <= 0)
                        {
                            firstPlayerStats.PlayerHeal(10);
                            MovePlayerToOtherPlayer();
                        }

                        if (secondPlayerStats.PlayerLife <= 0)
                        {
                            secondPlayerStats.PlayerHeal(10);
                            MovePlayerToOtherPlayer(false);
                        }

                    }
                    else
                    {
                        timer += 0.5f;
                    }
                }
            } else
            {
                if (!firstPlayer.activeSelf) LoseLevel();
            }
        }

        private void MovePlayerToOtherPlayer(bool firstToSecond = true)
        {
            if (firstToSecond)
            {
                firstPlayer.GetComponent<Rigidbody>().MovePosition(secondPlayer.transform.position);
                //firstPlayer.GetComponent<PlayerMovement>().enabled = false;
                //firstPlayer.transform.position = secondPlayer.transform.position;
                //firstPlayer.GetComponent<PlayerMovement>().enabled = true;
            }
            else
            {
                secondPlayer.GetComponent<Rigidbody>().MovePosition(firstPlayer.transform.position);
                //secondPlayer.GetComponent<PlayerMovement>().enabled = false;
                //secondPlayer.transform.position = firstPlayer.transform.position;
                //secondPlayer.GetComponent<PlayerMovement>().enabled = true;
            }
        }

        private void LoseLevel()
        {
            gameVars.WonLevel(false);
            sceneNavigation.ChangeScene("GameOver");
        }
    }
}