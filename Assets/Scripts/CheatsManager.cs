using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to show and hide a cheats menu, and hold most cheat methods, in Unity
	/// </summary>
	public class CheatsManager : MonoBehaviour 
	{
		[SerializeField] RectTransform cheatsPanel;

		[SerializeField] Transform enemiesHolder;
		[SerializeField] Transform randomEnemiesHolder;
		[SerializeField] Transform projectileHolder;
		[SerializeField] Transform staticEnemiesHolder;
		[SerializeField] PlayerMovement playerMovement;

		[SerializeField] GameVars gameVars;

		[SerializeField] Button godmodeButton;
		[SerializeField] PlayerStats playerStatsScript;

		public bool timeScaleToggled = false;
		
		public void ToggleCheatsMenu()
        {
			cheatsPanel.gameObject.SetActive(!cheatsPanel.gameObject.activeSelf);
        }

		public void WipeAllEnemies()
        {
            for (int i = 0; i < enemiesHolder.childCount; i++)
            {
				enemiesHolder.GetChild(i).GetComponent<EnemyBase>().score = 0;
				enemiesHolder.GetChild(i).GetComponent<EnemyBase>().BeginDeath();
			}

			for (int j = 0; j < randomEnemiesHolder.childCount; j++)
			{
				randomEnemiesHolder.GetChild(j).GetComponent<EnemyBase>().score = 0;
				randomEnemiesHolder.GetChild(j).GetComponent<EnemyBase>().BeginDeath();
			}

            for (int k = 0; k < staticEnemiesHolder.childCount; k++)
            {
				staticEnemiesHolder.GetChild(k).GetComponent<EnemyBase>().score = 0;
				staticEnemiesHolder.GetChild(k).GetComponent<EnemyBase>().BeginDeath();
			}

			//for (int k = 0; k < projectileHolder.childCount; k++)
			//{
			//	Destroy(projectileHolder.GetChild(k).gameObject);
			//}

			//for (int l = 0; l < staticEnemiesHolder.childCount; l++)
			//{
			//	Destroy(projectileHolder.GetChild(l).gameObject);
			//}
		}

		public void ToggleEnemySpawns()
        {
			gameVars.EnemySpawn(!gameVars.AllowEnemySpawn);
        }

		public async void DisplayGodMode()
        {
			await Task.Delay(1);
			if (playerStatsScript.GodmodeActive)
            {
				var colors = godmodeButton.colors;
				colors.normalColor = Color.green;
				godmodeButton.colors = colors;

				//godmodeButton.GetComponent<Image>().color = Color.red;
            } else
            {
				//godmodeButton.GetComponent<Image>().color = Color.white;

				var colors2 = godmodeButton.colors;
				colors2.normalColor = Color.white;
				godmodeButton.colors = colors2;
			}
        }

		public void ToggleTimeScale()
        {
			timeScaleToggled = !timeScaleToggled;

			if (timeScaleToggled)
            {
				Time.timeScale = 5f;
            } else
            {
				Time.timeScale = 1f;
            }
        }

		public void SkipToBoss()
        {
			//playerStatsScript.powerupAttackTracking = false;
			WipeAllEnemies();
			//playerMovement.CheatSkipToBoss();

			PlayerMovement[] allPlayerMovements = FindObjectsByType<PlayerMovement>(FindObjectsInactive.Exclude ,FindObjectsSortMode.None);

			foreach (PlayerMovement playerMovement in allPlayerMovements)
            {
				playerMovement.CheatSkipToBoss();
            }
        }
	}
}