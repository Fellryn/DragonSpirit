using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UlianaKutsenko;
using TMPro;
using DG.Tweening;

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
        EnemyBossLookConstraint enemyBossLookConstraint;

        [SerializeField]
        GameObject singlePlayerHud;
        [SerializeField]
        GameObject multiPlayerHud;

        [SerializeField]
        GameObject singlePlayerScoreHud;
        [SerializeField]
        GameObject multiPlayerScoreHud;

        [SerializeField]
        GameObject firstPlayer;
        [SerializeField]
        GameObject secondPlayer;

        [SerializeField]
        PlayerStats firstPlayerStats;
        [SerializeField]
        PlayerStats secondPlayerStats;

        [SerializeField]
        float respawnTime = 5f;
        float timer = 0f;

        [SerializeField]
        TextMeshProUGUI respawnText;
        [SerializeField] Vector2 textOffscreenPos;
        [SerializeField] Vector2 textOnscreenPos;
        [SerializeField] float totalTweenTime = 5f;
        [SerializeField] Ease textEaseInType = Ease.OutQuint;
        [SerializeField] Ease textEaseOutType = Ease.InQuint;
        [SerializeField] float shakeStrength = 3f;
        [SerializeField] int shakeVibrato = 2;
        [SerializeField] Ease shakeEaseType = Ease.InOutQuad;
        Sequence respawnTextSequence;



        public List<Transform> playersList = new List<Transform>();

        public delegate void OnSecondPlayerJoin();
        public static event OnSecondPlayerJoin onSecondPlayerJoin;

        void Start()
        {
            playerInputManager.onPlayerJoined += PlayerJoined;
            gameTickSystem.OnEveryHalfTick.AddListener(CheckPlayerStatus);
            playersList.Add(firstPlayer.transform);

            respawnTextSequence = DOTween.Sequence();
            respawnTextSequence
                .Append(respawnText.rectTransform.DOAnchorPos(new Vector2(respawnText.rectTransform.anchoredPosition.x, textOnscreenPos.y), totalTweenTime * 0.25f).SetEase(textEaseInType))
                .AppendInterval(totalTweenTime * 0.5f)
                .Join(respawnText.rectTransform.DOShakeAnchorPos(totalTweenTime * 0.75f, shakeStrength, shakeVibrato, fadeOut: false).SetEase(shakeEaseType))
                .Append(respawnText.rectTransform.DOAnchorPos(new Vector2(respawnText.rectTransform.anchoredPosition.x, textOffscreenPos.y), totalTweenTime * 0.25f).SetEase(textEaseOutType))
                .SetAutoKill(false);

            respawnTextSequence.Complete();
        }

        void OnDestroy()
        {
            playerInputManager.onPlayerJoined -= PlayerJoined;
            gameTickSystem.OnEveryHalfTick.RemoveListener(CheckPlayerStatus);
        }

        void PlayerJoined(PlayerInput playerInput)
        {
            onSecondPlayerJoin?.Invoke();

            singlePlayerHud.SetActive(false);
            multiPlayerHud.SetActive(true);
            singlePlayerScoreHud.SetActive(false);
            multiPlayerScoreHud.SetActive(true);

            //playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;

            secondPlayer = playerInput.gameObject;
            secondPlayerStats = playerInput.GetComponent<PlayerStats>();

            playerInputManager.enabled = false;

            MovePlayerToOtherPlayer(false);

            playersList.Add(secondPlayer.transform);

            enemyBossLookConstraint.GetPlayerTransforms(firstPlayer.transform, secondPlayer.transform);
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
                            playersList.Add(firstPlayer.transform);
                        }

                        if (secondPlayerStats.PlayerLife <= 0)
                        {
                            secondPlayerStats.PlayerHeal(10);
                            MovePlayerToOtherPlayer(false);
                            playersList.Add(secondPlayer.transform);
                        }

                    }
                    else
                    {
                        timer += 0.5f;

                        if (timer == 0.5f)
                        {
                            DisplayRespawnTimer();
                        }

                        if (timer % 1 == 0)
                        {
                            respawnText.text = $"Respawn in: {(respawnTime - timer).ToString()}";
                        }

                        if (timer == respawnTime)
                        {
                            respawnText.text = "Respawning!";
                        }

                        if (!firstPlayer.activeSelf)
                        {
                            playersList.Remove(firstPlayer.transform);
                        }

                        if (!secondPlayer.activeSelf)
                        {
                            playersList.Remove(secondPlayer.transform);
                        }
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

        private void DisplayRespawnTimer()
        {
            respawnText.text = "Respawn in: 5";

            respawnTextSequence.Complete();

            respawnTextSequence.Restart();
        }

        private void LoseLevel()
        {
            //gameVars.WonLevel(false);
            sceneNavigation.ChangeScene("GameOver");
        }
    }
}
