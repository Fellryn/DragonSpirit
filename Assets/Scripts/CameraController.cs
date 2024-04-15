using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
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

        [SerializeField] CinemachineCamera bossCamera;

        [SerializeField] Transform playerTransform;

        [SerializeField] float cameraOffsetModifier = 0.25f;
        [SerializeField] float bossSplinePosition = 1f;
        [SerializeField] float stopEnemySpawnSplinePosition = 0.9f;

        public bool atBoss = false;


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

        public void DoChecks()
        {
            AtBossCheck();
            AllowEnemySpawnCheck();
        }

        private void AtBossCheck()
        {
            if (dollyCamera.CameraPosition >= bossSplinePosition)
            {
                if (bossCamera.gameObject.activeSelf == false) bossCamera.gameObject.SetActive(true);
                atBoss = true;
            }
        }

        private void AllowEnemySpawnCheck()
        {
            if (dollyCamera.CameraPosition >= stopEnemySpawnSplinePosition)
            {
                if (gameVars.AllowEnemySpawn) gameVars.EnemySpawn(false);
            }
        }
    }
}