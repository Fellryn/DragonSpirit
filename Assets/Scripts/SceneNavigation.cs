using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
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
			EventSystem.current.SetSelectedGameObject(null);

			if (SceneManager.sceneCount > 1)
            {
				CloseSceneAdditive();
				return;
			}

			sceneNameToChangeTo = sceneName;

			ChangeScene();
		}


		public void ChangeSceneWithDelay(string sceneName)
		{
			sceneNameToChangeTo = sceneName;

			Invoke("ChangeScene", sceneChangeDelay);
		}


		public void ChangeSceneAdditive(string sceneName)
		{
			EventSystem.current.SetSelectedGameObject(null);

			SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
        }


		public void CloseSceneAdditive()
		{
			SceneManager.UnloadSceneAsync(gameObject.scene);
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