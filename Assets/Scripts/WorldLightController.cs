using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using Unity.Cinemachine;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to ... in Unity
	/// </summary>
	public class WorldLightController : MonoBehaviour 
	{
		[SerializeField]
		Light worldLight;

		[SerializeField]
		CinemachineSplineDolly splineCamera;

		[SerializeField]
		float maxLightIntensity;
		[SerializeField]
		float minLightIntensity;

        private void Update()
        {
            if (splineCamera != null)
            {
				float intendedIntensity = ((1f - splineCamera.CameraPosition) * maxLightIntensity) + minLightIntensity;
				if (intendedIntensity != worldLight.intensity) worldLight.intensity = intendedIntensity;
            }
        }
    }
}