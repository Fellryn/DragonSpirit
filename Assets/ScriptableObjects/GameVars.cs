using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	[CreateAssetMenu(fileName = "GameVars", menuName = "ScriptableObjects/GameVarsScriptableObject", order = 1)]
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to set and use game wide variables with a scriptable object in Unity
	/// </summary>

	public class GameVars : ScriptableObject 
	{
        public bool AllowEnemySpawn { get; private set; }
        public bool wonLastLevel { get; private set; }

        private void OnEnable()
        {
            SetDefaults();
        }


        public void SetDefaults()
        {
            EnemySpawn(true);
        }


        public void EnemySpawn(bool foo)
        {
			AllowEnemySpawn = foo;
        }

        public void WonLevel(bool foo)
        {
            wonLastLevel = foo;
        }


    }
}