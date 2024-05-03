using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UlianaKutsenko;

namespace UlianaKutsenko
{

/// <summary>
/// Author: Uliana Kutsenko
/// Description: This script demonstrates how to change scene in Unity
/// </summary>

public class SceneNavigation : MonoBehaviour 
{
		[SerializeField]
		float sceneChangeDelay = 0.15f;

		string sceneNameToChangeTo;

        private void Start()
        {
            if (gameObject.scene.name == "Splash")
            {
				sceneNameToChangeTo = "Main";
				ChangeScene();
            }
        }


        public void ChangeScene(string sceneName)
		{
			sceneNameToChangeTo = sceneName;

			ChangeScene();
		}


		public void ChangeSceneWithDelay(string sceneName)
		{
			sceneNameToChangeTo = sceneName;

			Invoke("ChangeScene", sceneChangeDelay);
		}


		public void ChangeSceneAsync(string sceneName)
		{
            SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        }



		private void ChangeScene()
		{
			Time.timeScale = 1f;
			SceneManager.LoadScene(sceneNameToChangeTo, LoadSceneMode.Single);
        }





		public void QuitApplication()
		{
#if UNITY_WEBGL
#else
			Application.Quit();
#endif
		}




}

}