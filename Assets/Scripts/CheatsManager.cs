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

		[SerializeField] GameVars gameVars;
		
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
		}

		public void ToggleEnemySpawns()
        {
			gameVars.EnemySpawn(!gameVars.AllowEnemySpawn);
        }
	}
}