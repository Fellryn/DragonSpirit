using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Splines;
using Unity.Cinemachine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to move the main character with the new input system in Unity
	/// </summary>
	public class PlayerMovement : MonoBehaviour 
	{
		[SerializeField] InputActionReference movement;
        [SerializeField] float moveSpeed = 50f;

        [SerializeField] SplineContainer splineContainer;
        [SerializeField] CinemachineCamera splineCamera;
        [SerializeField] CinemachineSplineDolly dollyCamera;
        [SerializeField] Camera usedCamera;
        public bool playerMatchCameraMovement = true;
        [SerializeField] float cameraMoveModifier = 4f;
        private Vector3 lastPosition;
        private bool skippedFirstFrame = false;
        Vector3 screenPosition;
        [SerializeField] float distanceBetweenSplinePoints = 0.03f;

        public float moveModifier = 1f;
        [SerializeField] float correctingForce = 10f;

		private Rigidbody cachedRigidbody;






        private void Start()
        {
            cachedRigidbody = GetComponent<Rigidbody>();

            lastPosition = dollyCamera.transform.position;
        }


        private void Awake()
        {
            GameManager.OnGameStateChanged += PauseAction;
        }


        private void OnDestroy()
        {
            GameManager.OnGameStateChanged -= PauseAction;
        }


        private void PauseAction(GameManager.GameStates state)
        {
            if (GameManager.Instance.State == GameManager.GameStates.Paused)
            {

            }
            else
            {

            }
        }


        private void FixedUpdate()
        {
            screenPosition = usedCamera.WorldToViewportPoint(transform.position);

            Move();
            CameraMovePlayer();
            MatchSplineHeight();
            EnforceBounds();
        }


        private void Move()
        {
            Vector2 move = movement.action.ReadValue<Vector2>();
            if (move != new Vector2(0f, 0f))
            {
                move = CheckBounds(move);

                Vector3 movementModified = new Vector3(move.x * moveSpeed * moveModifier, 0, move.y * moveSpeed * moveModifier);

                cachedRigidbody.AddRelativeForce(movementModified, ForceMode.Force);
            }

            Vector3 rotationTarget = splineContainer.EvaluateTangent(0, dollyCamera.CameraPosition);

            transform.rotation = Quaternion.LookRotation(rotationTarget);
        }


        private void CameraMovePlayer()
        {
            if (playerMatchCameraMovement || skippedFirstFrame)
            {
                if (lastPosition == dollyCamera.transform.position) return;

                Vector3 forceToAdd = (dollyCamera.transform.position - lastPosition) * moveSpeed * cameraMoveModifier;

                cachedRigidbody.AddForce(forceToAdd);
                lastPosition = dollyCamera.transform.position;
            }

            skippedFirstFrame = true;
        }


        private void MatchSplineHeight()
        {
            Vector3[] fourPoints= { default, default, default, default };
            float[] fourPointsSplinePosition = { default, default, default, default };
            for (int i = 0; i < fourPoints.Length; i++)
            {
                fourPoints[i] = dollyCamera.Spline.EvaluatePosition(dollyCamera.CameraPosition + (i * distanceBetweenSplinePoints));
                fourPointsSplinePosition[i] = dollyCamera.CameraPosition + (i * distanceBetweenSplinePoints);
            }


            float[] distance = { default, default, default, default };
            for (int j = 0; j < fourPoints.Length; j++)
            {
                distance[j] = Vector3.Distance(transform.position, fourPoints[j]);
            }


            float ClosestPoint = Mathf.Min(distance);
            int positionToUseIndex = default;
            for (int k = 0; k < fourPoints.Length; k++)
            {
                if (ClosestPoint == distance[k])
                {
                    positionToUseIndex = k;
                }
            }


            Vector3 splineHeightY = dollyCamera.Spline.EvaluatePosition(fourPointsSplinePosition[positionToUseIndex]);
            Vector3 splinePosition = new Vector3(cachedRigidbody.position.x, splineHeightY.y, cachedRigidbody.position.z);
            splinePosition = Vector3.MoveTowards(transform.position, splinePosition, 0.01f);
            cachedRigidbody.MovePosition(splinePosition);
        }


        private Vector2 CheckBounds(Vector2 moveInputRaw)
        {
            Vector2 moveInputChecked = moveInputRaw;

            if (screenPosition.x > 0.95f)
            {
                moveInputChecked.x = Mathf.Clamp(moveInputRaw.x, -1f, 0f);
            }

            if (screenPosition.x < 0.05f)
            {
                moveInputChecked.x = Mathf.Clamp(moveInputRaw.x, 0f, 1f);
            }

            if (screenPosition.y > 0.95f)
            {
                moveInputChecked.y = Mathf.Clamp(moveInputRaw.y, -1f, 0f);
            }

            if (screenPosition.y < 0.05f)
            {
                moveInputChecked.y = Mathf.Clamp(moveInputRaw.y, 0f, 1f);
            }

            return moveInputChecked;
        }


        private void EnforceBounds()
        {

            if (screenPosition.x > 0.96f)
            {
                cachedRigidbody.AddRelativeForce(-correctingForce, 0, 0);
            }

            if (screenPosition.x < 0.04f)
            {
                cachedRigidbody.AddRelativeForce(correctingForce, 0, 0);
            }

            if (screenPosition.y > 0.96f)
            {
                cachedRigidbody.AddRelativeForce(0, 0, -correctingForce);
            }

            if (screenPosition.y < 0.04f)
            {
                cachedRigidbody.AddRelativeForce(0, 0, correctingForce);
            }
        }
    }
}