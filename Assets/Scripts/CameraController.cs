using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Events;
using UnityEditor;
using KurtSingle;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to control camera and camera related events in Unity
    /// </summary>
	public class CameraController : MonoBehaviour 
	{
        [SerializeField] GameVars gameVars;

		[SerializeField] CinemachineSplineDolly dollyCamera;
		[SerializeField] CinemachineCameraOffset dollyCameraOffset;
        private CinemachineCamera dollyCinemachineCamera;

        [SerializeField] CinemachineCamera bossCamera;
        [SerializeField] CinemachineCamera bossCameraWidescreen;

        [SerializeField] Transform playerTransform;

        [SerializeField] float cameraOffsetModifier = 0.25f;
        [SerializeField] float bossSplinePosition = 1f;
        [SerializeField] float stopEnemySpawnSplinePosition = 0.9f;

        [SerializeField] Vector2 standardCameraSettings = new Vector2(60, 22);
        [SerializeField] Vector2 widescreenCameraSettings = new Vector2(22, 40);

        public bool atBoss = false;
        public UnityEvent AtBossEvent;

        private Vector2 originalCameraFovOffset;
        private bool useWidescreenBossCamera;

        private void OnEnable()
        {
			dollyCamera.CameraPosition = 0;

            dollyCameraOffset = dollyCamera.GetComponent<CinemachineCameraOffset>();
            dollyCinemachineCamera = dollyCamera.GetComponent<CinemachineCamera>();

            float aspectRatio = (float)Screen.width / Screen.height;

            if (aspectRatio >= 5f)
            {
                useWidescreenBossCamera = true;
                dollyCinemachineCamera.Lens.FieldOfView = widescreenCameraSettings.x;
                dollyCamera.SplineOffset.y = widescreenCameraSettings.y;
            } else
            {
                dollyCinemachineCamera.Lens.FieldOfView = standardCameraSettings.x;
                dollyCamera.SplineOffset.y = standardCameraSettings.y;
            }

            originalCameraFovOffset.x = dollyCamera.GetComponent<CinemachineCamera>().Lens.FieldOfView;
            originalCameraFovOffset.y = dollyCamera.SplineOffset.y;
        }

        private void OnDisable()
        {
            //dollyCinemachineCamera.Lens.FieldOfView = originalCameraFovOffset.x;
            //dollyCamera.SplineOffset.y = originalCameraFovOffset.y;
        }


        private void OnApplicationQuit()
        {
            //dollyCinemachineCamera.Lens.FieldOfView = originalCameraFovOffset.x;
            //dollyCamera.SplineOffset.y = originalCameraFovOffset.y;
            dollyCamera.AutomaticDolly.Enabled = false;
            dollyCamera.CameraPosition = 0.01f;
        }

        //private void LateUpdate()
        //{
        //    if (resetCameraPosition)
        //    {
        //        dollyCamera.CameraPosition = 0.01f;
        //    }
        //}


        private void Update()
        {
            if (playerTransform != null)
            {
                dollyCameraOffset.Offset.x = playerTransform.position.x * cameraOffsetModifier;
            }
        }

        public void DoChecks()
        {
            AtBossCheck();
            AllowEnemySpawnCheck();
        }

        private void AtBossCheck()
        {
            if (dollyCamera.CameraPosition >= bossSplinePosition && !atBoss)
            {
                if (useWidescreenBossCamera)
                {
                    bossCameraWidescreen.gameObject.SetActive(true);
                } else
                {
                    bossCamera.gameObject.SetActive(true);
                }
                
                atBoss = true;
                AtBossEvent?.Invoke();
            }
        }

        private void AllowEnemySpawnCheck()
        {
            if (dollyCamera.CameraPosition >= stopEnemySpawnSplinePosition && gameVars.AllowEnemySpawn)
            {
                gameVars.EnemySpawn(false);
            }
        }
    }
}