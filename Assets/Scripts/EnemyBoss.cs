using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using UlianaKutsenko;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to make a child script for an enemy (the boss) in Unity
	/// </summary>
	public class EnemyBoss : EnemyStatic 
	{
		[SerializeField] GameVars gameVars;
		[SerializeField] SceneNavigation sceneNavigation;
 
        private void OnDestroy()
        {
			if (!gameObject.scene.isLoaded) return;

			gameVars.WonLevel(true);
			sceneNavigation.ChangeScene("GameOver");
        }


    }
}