using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to display and update highscores using an external database in Unity
	/// </summary>
	public class HighScoreDisplay : MonoBehaviour 
	{
		[SerializeField]
		DisplayAllLeaderboardData displayAllLeaderboardData;
		[SerializeField]
		GameVars gameVars;
		[SerializeField]
		HighScores highScores;

		[SerializeField]
		GameObject scoreInputArea;

		[SerializeField]
		TextMeshProUGUI playerOneScoreText;
		[SerializeField]
		TextMeshProUGUI playerTwoScoreText;

		[SerializeField]
		TextMeshProUGUI playerOneNameInput;
		[SerializeField]
		TextMeshProUGUI playerTwoNameInput;

		[SerializeField]
		TMP_InputField playerOneNameInputParent;
		[SerializeField]
		TMP_InputField playerTwoNameInputParent;

		[SerializeField]
		Button playerOneInputOverlay;
		[SerializeField]
		Button playerTwoInputOverlay;

		[SerializeField]
		Button[] playerTwoInputs;
		[SerializeField]
		TextMeshProUGUI[] playerTwoText;
		[SerializeField]
		TextMeshProUGUI submitText;

		[SerializeField]
		Button playerOneSubmitButton;
		[SerializeField]
		Button playerTwoSubmitButton;

		[SerializeField]
		PlayerInput playerInput;


 
		public bool isUsingGamepad = false;

		private void Start()
        {
			DisplayLeaderboard(1000);

			if (!gameVars.wonLastLevel)
            {
				scoreInputArea.SetActive(false);
            }

			playerOneScoreText.text = "0";
			playerTwoScoreText.text = "0";

			if (gameVars.PlayerOneScore > 0)
			{
				playerOneScoreText.text = gameVars.PlayerOneScore.ToString("N0");
			}

			if (gameVars.PlayerTwoScore > 0)
			{
				playerTwoScoreText.text = gameVars.PlayerTwoScore.ToString("N0");
			} else
            {
				foreach (Button obj in playerTwoInputs)
                {
					obj.interactable = false;
                }
				foreach (TextMeshProUGUI text in playerTwoText)
                {
					text.alpha = 0.11f;
                }
				submitText.alpha = 0.15f;

            }

			playerOneSubmitButton.onClick.AddListener(delegate { SubmitPlayerScore(true); });
			playerTwoSubmitButton.onClick.AddListener(delegate { SubmitPlayerScore(false); });
		}

        private void OnDestroy()
        {
			playerOneSubmitButton.onClick.RemoveListener(delegate { SubmitPlayerScore(true); });
			playerTwoSubmitButton.onClick.RemoveListener(delegate { SubmitPlayerScore(false); });
		}

        private void Update()
        {
            if (playerInput.currentControlScheme == "Gamepad")
            {
				isUsingGamepad = true;
            } else
            {
				isUsingGamepad = false;
            }
        }

        private async void DisplayLeaderboard(int timeInMs)
        {
			highScores.RefreshScoresDelayed(1);
			try
			{
				await Task.Delay(timeInMs);
				displayAllLeaderboardData.UpdateValues();
            }
            catch (Exception e)
            {
				Debug.Log($"Error! {e.ToString()}");
            }
        }

		private void SubmitPlayerScore(bool isPlayerOne)
        {
			if (isPlayerOne && gameVars.PlayerOneScore > 0 && playerOneNameInput.text.Length != 1)
            {
				highScores.UploadScoreTimeComment(playerOneNameInput.text, gameVars.PlayerOneScore, 60, "Tafe Player");
				playerOneSubmitButton.interactable = false;
				playerOneInputOverlay.interactable = false;
				playerOneSubmitButton.GetComponentInChildren<TextMeshProUGUI>().alpha = 0.05f;

				DisplayLeaderboard(5000);
            } else if (gameVars.PlayerTwoScore > 0 && playerTwoNameInput.text.Length != 1)
            {
				highScores.UploadScoreTimeComment(playerTwoNameInput.text, gameVars.PlayerTwoScore, 60, "Tafe Player");
				playerTwoSubmitButton.interactable = false;
				playerTwoInputOverlay.interactable = false;
				playerTwoSubmitButton.GetComponentInChildren<TextMeshProUGUI>().alpha = 0.05f;

				DisplayLeaderboard(5000);
			}
        }

    }
}