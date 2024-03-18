using System.Collections;
using System.Collections.Generic;
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
				enemiesHolder.GetChild(i).GetComponent<EnemyBase>().BeginDeath();
            }

			for (int i = 0; i < randomEnemiesHolder.childCount; i++)
			{
				randomEnemiesHolder.GetChild(i).GetComponent<EnemyBase>().BeginDeath();
			}

			for (int i = 0; i < projectileHolder.childCount; i++)
			{
				Destroy(projectileHolder.GetChild(i).gameObject);
			}

			for (int i = 0; i < staticEnemiesHolder.childCount; i++)
			{
				Destroy(projectileHolder.GetChild(i).gameObject);
			}
		}

		public void ToggleEnemySpawns()
        {
			gameVars.EnemySpawn(!gameVars.AllowEnemySpawn);
        }

		public void DisplayGodMode()
        {
			if (playerStatsScript.GodmodeActive)
            {
				godmodeButton.GetComponent<Image>().color = Color.red;
            } else
            {
				godmodeButton.GetComponent<Image>().color = Color.white;
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
	}
}