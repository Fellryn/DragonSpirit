using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;
using DG.Tweening;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to create a powerup pickup in Unity
    /// When a powerup is spawned, it finds a target position in front of the player
    /// It maintains this position until a variable time, and then floats down off-screen
    /// The player pickup code is found in PlayerPowerups.cs, which is on the player object
    /// </summary>
    public class PowerUpBase : MonoBehaviour
    {
        private Camera cachedPlayerCamera;
        private Transform cachedPlayerTransform;
        private Rigidbody cachedRigidbody;
        private GameTickSystem gameTickSystem;

        /// <summary>
        /// Must be exact certain names: "LifePowerUp", "MultiShotPowerUp", "TrackingPowerUp"
        /// </summary>
        [Tooltip("Must be exact certain names: LifePowerUp, MultiShotPowerUp, TrackingPowerUp")]
        public string powerUpName = "LifePowerUp";
        [SerializeField] GameObject powerupVisualEffect;
        [SerializeField] float moveSpeed = 15f;
        [SerializeField] Renderer cachedRenderer;
        [SerializeField] Renderer cachedRendererSub;
        
        [Header("Timings")]
        [SerializeField] float chanceToChangePosition = 0.25f;
        [SerializeField] float timeBeforeMoveFromScreen = 15f;
        [SerializeField] float timeBeforeAllowedChangePosition = 3f;
        [SerializeField] float timeBeforeMustChangePosition = 6f;
        bool movingFromScreen = false;
        float timer = 0f;
        float timerSinceLastMove = 0f;
        
        Vector3 screenPosition;
        Vector3 offsetMoveTarget;



        private void OnEnable()
        {
            
            cachedRigidbody = GetComponent<Rigidbody>();
            cachedPlayerCamera = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            cachedPlayerTransform = FindAnyObjectByType<PlayerStats>().GetComponent<Transform>();
            gameTickSystem = FindAnyObjectByType<GameTickSystem>();

            ChangePosition();
            

            //transform.DORotate(new Vector3(Random.Range(0, 360f), Random.Range(0, 360f), Random.Range(0, 360f)), 1f).SetEase(Ease.Linear).SetRelative().SetLoops(-1, LoopType.Incremental);
            cachedRigidbody.maxLinearVelocity = 15f;
            cachedRigidbody.AddForce(Vector3.up * 2, ForceMode.Impulse);

            gameTickSystem.OnEveryHalfTick.AddListener(DoChecks);

        }

        private void OnDisable()
        {
            gameTickSystem.OnEveryHalfTick.RemoveListener(DoChecks);
        }


        private void FixedUpdate()
        {
            Move();
        }

        public void PassPlayerTransform(Transform playerTransform)
        {
            cachedPlayerTransform = playerTransform;
        }


        public void BeginDestruction()
        {
            transform.tag = "Untagged";

            cachedRigidbody.isKinematic = true;
            transform.DOMove(cachedPlayerTransform.position, 0.1f);
            cachedRenderer.material.DOFade(0, 0.5f).OnComplete(() => Destroy(gameObject));
            cachedRendererSub.material.DOFade(0, 0.5f);
        }


        //private void Destroy()
        //{
        //    Destroy(gameObject);
        //}


        private void OnDestroy()
        {
            DOTween.Kill(transform);
            DOTween.Kill(cachedRenderer.material);
            DOTween.Kill(cachedRendererSub.material);

            if (!gameObject.scene.isLoaded) return;

            if (powerupVisualEffect != null)
            {
                Instantiate(powerupVisualEffect, transform.position, Quaternion.identity);
            }

        }

        private void DoChecks()
        {
            screenPosition = cachedPlayerCamera.WorldToViewportPoint(transform.position);

            if (!movingFromScreen) ChangePositionCheck();

            if (movingFromScreen) CheckBounds();
        }


        private void Move()
        {
            Vector3 finalMoveTarget = new Vector3(offsetMoveTarget.x, offsetMoveTarget.y, cachedPlayerCamera.transform.position.z + offsetMoveTarget.z);
            finalMoveTarget.y = 0;

            if (movingFromScreen) finalMoveTarget.z = transform.position.z;

            transform.LookAt(finalMoveTarget);
            cachedRigidbody.AddForce(transform.forward * moveSpeed * 0.1f, ForceMode.Impulse);
        }

        private void ChangePositionCheck()
        {
            if (timer < timeBeforeMoveFromScreen)
            {
                timer += 0.5f;
                timerSinceLastMove += 0.5f;

                if ((Random.Range(0f, 1f) < chanceToChangePosition && timerSinceLastMove > timeBeforeAllowedChangePosition) || timerSinceLastMove > timeBeforeMustChangePosition)
                {
                    ChangePosition();
                }

            }
            else
            {
                movingFromScreen = true;
            }

        }

        private void ChangePosition()
        {
            Vector3 randomMoveTarget = cachedPlayerCamera.ViewportToWorldPoint(new Vector3(Random.Range(0.2f, 0.8f), Random.Range(0.5f, 1f), cachedPlayerCamera.transform.position.y - cachedPlayerTransform.position.y));
            randomMoveTarget.y = cachedPlayerTransform.position.y;

            offsetMoveTarget = randomMoveTarget - cachedPlayerCamera.transform.position;
            //storeTargetPosition = randomMoveTarget - cachedPlayerCamera.transform.position;

            timerSinceLastMove = 0f;
        }


        private void CheckBounds()
        {


            //if (screenPosition.y < 0.05f)
            //{
            //    ChangePosition();
            //}

            if (screenPosition.y < -0.1f)
            {

                Destroy(gameObject);
            }
        }




    }
}