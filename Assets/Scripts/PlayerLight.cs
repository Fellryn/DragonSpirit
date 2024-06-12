using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to ... in Unity
	/// </summary>
	public class PlayerLight : MonoBehaviour 
	{
		[SerializeField]
		Light playerLight;
		[SerializeField]
		Camera mainCamera;

        private void Update()
        {
			playerLight.transform.rotation = Quaternion.LookRotation(transform.position - mainCamera.transform.position);
        }

    }
}