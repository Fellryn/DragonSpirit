using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using KurtSingle;
namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to ... in Unity
    /// </summary>
	public class CameraController : MonoBehaviour 
	{
		[SerializeField] CinemachineSplineDolly dollyCamera;
		[SerializeField] CinemachineCameraOffset dollyCameraOffset;

        [SerializeField] Transform playerTransform;

        [SerializeField] float cameraOffsetModifier = 0.25f;


        private void OnEnable()
        {
			dollyCamera.CameraPosition = 0;

            dollyCameraOffset = dollyCamera.GetComponent<CinemachineCameraOffset>();
        }

        private void Update()
        {
            if (playerTransform != null)
            {
                dollyCameraOffset.Offset.x = playerTransform.position.x * cameraOffsetModifier;
            }
        }
    }
}