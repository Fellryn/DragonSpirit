using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using System.Threading.Tasks;
using System;
using TMPro;
using UnityEngine.UI;

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
		GameObject[] playerTwoFields;

		[SerializeField]
		Button playerOneSubmitButton;
		[SerializeField]
		Button playerTwoSubmitButton;

		private void Start()
        {
			DisplayLeaderboard();

			if (!gameVars.wonLastLevel)
            {
				scoreInputArea.SetActive(false);
            }

			if (gameVars.PlayerOneScore != 0)
			{
				playerOneScoreText.text = gameVars.PlayerOneScore.ToString();
			}

			if (gameVars.PlayerTwoScore != 0)
			{
				playerTwoScoreText.text = gameVars.PlayerTwoScore.ToString();
			} else
            {
				foreach (GameObject obj in playerTwoFields)
                {
					obj.SetActive(false);
                }
            }

			playerOneSubmitButton.onClick.AddListener(delegate { SubmitPlayerScore(true); });
			playerTwoSubmitButton.onClick.AddListener(delegate { SubmitPlayerScore(false); });
		}

        private void OnDestroy()
        {
			playerOneSubmitButton.onClick.RemoveListener(delegate { SubmitPlayerScore(true); });
			playerTwoSubmitButton.onClick.RemoveListener(delegate { SubmitPlayerScore(false); });
		}

        private async void DisplayLeaderboard()
        {
			try
			{
				await Task.Delay(1000);
				displayAllLeaderboardData.UpdateValues();
            }
            catch (Exception e)
            {
				Debug.Log($"Error! {e.ToString()}");
            }
        }

		private void SubmitPlayerScore(bool isPlayerOne)
        {
			if (isPlayerOne)
            {
				highScores.UploadScoreTimeComment(playerOneNameInput.text, gameVars.PlayerOneScore, 60, "Tafe Player");
            } else
            {
				highScores.UploadScoreTimeComment(playerTwoNameInput.text, gameVars.PlayerTwoScore, 60, "Tafe Player");
			}
        }

    }
}