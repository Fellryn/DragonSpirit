using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to change canvas scale and other settings to make it more responsive in Unity
	/// </summary>
	public class UIController : MonoBehaviour
	{
		[SerializeField] CanvasScaler[] canvasScalers;

        private void Start()
        {
			if (Screen.height > Screen.width)
            {
                for (int i = 0; i < canvasScalers.Length; i++)
                {
                    Debug.Log(canvasScalers[i].scaleFactor);
                    canvasScalers[i].scaleFactor = 2f;
                }
            }
        }
    }
}