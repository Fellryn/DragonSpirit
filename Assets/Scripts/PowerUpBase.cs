using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using DG.Tweening;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to create a parent powerup class in Unity
	/// </summary>
	public class PowerUpBase : MonoBehaviour 
	{
		private Camera cachedPlayerCamera;
		private Transform cachedPlayerTransform;
		private Rigidbody cachedRigidbody;

		[SerializeField] GameObject powerupVisualEffect;
		[SerializeField] float moveSpeed = 15f;
		[SerializeField] float moveAcceleration = 0.15f;
		private float originaMoveSpeed;
		[SerializeField] float chanceToChangePosition = 0.01f;
		[SerializeField] float timeBeforeMoveFromScreen = 10f;
		[SerializeField] float timeBeforeChangePosition = 3f;
		bool movingFromScreen = false;
		float timer = 0f;
		float timerSinceLastMove = 0f;
		Vector3 screenPosition;

		private Vector3 randomMoveTarget;



        private void OnEnable()
        {

			cachedRigidbody = GetComponent<Rigidbody>();
            cachedPlayerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
			cachedPlayerTransform = FindAnyObjectByType<PlayerStats>().GetComponent<Transform>();

            randomMoveTarget = cachedPlayerCamera.ViewportToWorldPoint(new Vector3(Random.Range(0.2f, 0.8f), Random.Range(0.5f, 1f), 30f));
			originaMoveSpeed = moveSpeed;

			transform.DORotate(new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f)), 1f).SetEase(Ease.Linear).SetRelative().SetLoops(-1, LoopType.Incremental);
        }



        private void FixedUpdate()
        {
            screenPosition = cachedPlayerCamera.WorldToViewportPoint(transform.position);

            ChangePositionCheck();

            Move();

            CheckBounds();
        }

        private void Move()
        {
            float distanceFromMoveTarget = Vector3.Distance(transform.position, randomMoveTarget);
            if (distanceFromMoveTarget < 5)
            {
                moveSpeed = originaMoveSpeed * distanceFromMoveTarget / 5;
            }
            else
            {
                moveSpeed += moveAcceleration;
                moveSpeed = Mathf.Clamp(moveSpeed, 0, originaMoveSpeed);
            }

            Vector3 moveTarget = Vector3.MoveTowards(transform.position, randomMoveTarget, moveSpeed * Time.deltaTime);
            ////moveTarget.y = cachedPlayerTransform.position.y;



            //Quaternion rotateTarget = Quaternion.RotateTowards(cachedRigidbody.rotation, Quaternion.LookRotation(cachedRigidbody.position - cachedPlayerTransform.position), rotationSpeed * Time.deltaTime);
            cachedRigidbody.MovePosition(moveTarget);
            //cachedRigidbody.MoveRotation(rotateTarget);



        }

        private void ChangePositionCheck()
        {
            if (!movingFromScreen)
            {

                if (timer < timeBeforeMoveFromScreen)
                {
                    timer += Time.deltaTime;
                    timerSinceLastMove += Time.deltaTime;

                    if (Random.Range(0f, 1f) < chanceToChangePosition && timerSinceLastMove > timeBeforeChangePosition)
                    {
                        ChangePosition();
                    }

                }
                else
                {
                    //randomMoveTarget = cachedPlayerCamera.ViewportToWorldPoint(new Vector3(Random.Range(0.2f, 0.8f), Random.Range(-1f, -0.5f), cachedPlayerCamera.transform.position.y - cachedPlayerTransform.position.y));
                    movingFromScreen = true;
                }

            }
        }

        private void ChangePosition()
        {
            randomMoveTarget = cachedPlayerCamera.ViewportToWorldPoint(new Vector3(Random.Range(0.2f, 0.8f), Random.Range(0.7f, 1.5f), cachedPlayerCamera.transform.position.y - cachedPlayerTransform.position.y));
            randomMoveTarget.y = cachedPlayerTransform.position.y;
            timerSinceLastMove = 0f;
        }


        private void CheckBounds()
		{
			if (timer < timeBeforeMoveFromScreen / 2) return;

            if (screenPosition.y < 0.05f)
            {
                ChangePosition();
            }

			if (screenPosition.y < -0.1f)
			{

				Destroy(gameObject);
			}
		}


		private void OnDestroy()
		{
			DOTween.Kill(transform);

			if (!gameObject.scene.isLoaded) return;

			Instantiate(powerupVisualEffect, transform.position, Quaternion.identity);
		}

	}
}