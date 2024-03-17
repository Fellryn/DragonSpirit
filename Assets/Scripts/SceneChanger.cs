using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to change scene  in Unity
	/// </summary>
	public class SceneChanger : MonoBehaviour 
	{
		public void ChangeScene(string sceneName)
        {
			SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
        }
		
	}
}