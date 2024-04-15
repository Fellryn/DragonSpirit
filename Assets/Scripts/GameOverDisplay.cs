using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using TMPro;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to change the score screen to reflect winning or losing in Unity
	/// </summary>
	public class GameOverDisplay : MonoBehaviour 
	{
		[SerializeField] GameVars gameVars;
		[SerializeField] TextMeshProUGUI winLoseText;

		[Header("Win")]
		[SerializeField] string winText = "You Won!";
		[SerializeField] Color winColour = Color.green;

		[Header("Lose")]
		[SerializeField] string loseText = "You Lost! Bad Luck!";
		[SerializeField] Color loseColour = Color.red;

		private void Start()
        {
            if (gameVars.wonLastLevel)
            {
				winLoseText.text = winText;
				winLoseText.color = winColour;
            }
            else
            {
				winLoseText.text = loseText;
				winLoseText.color = loseColour;
			}
        }
    }
}