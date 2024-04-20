using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using KurtSingle;


namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to attack as the player in Unity
    /// </summary>
    public class PlayerAttack : MonoBehaviour
    {
        [Header("Attack References")]
        [SerializeField] PlayerStats playerStats;
        [SerializeField] Transform cachedPlayerCamera;
        [SerializeField] Transform projectilesHolder;
        [SerializeField] InputActionReference fire;
        [SerializeField] InputActionReference altFire;
        //[SerializeField] string playerProjectileTag = "PlayerProjectile";

        [Header("Fireball Attack")]
        [SerializeField] float fireballMovespeed = 15f;
        [SerializeField] GameObject fireballPrefab;

        [Header("Tracking Fireball")]
        [SerializeField] float trackingFireballMovespeed = 17f;
        [SerializeField] GameObject trackingFireballPrefab;
        [SerializeField] float trackingFireballManaCost = 0.5f;

        [Header("Multi Fireball")]
        [SerializeField] float maxAngleOffset = 30f;
        public int multiFireballCount = 5;

        private Transform cachedTransform;


        private void Start()
        {
            cachedTransform = GetComponent<Transform>();
        }


        private void OnEnable()
        {
            fire.action.performed += PlayerAttacking;
            altFire.action.performed += PlayerAltAttacking;
        }


        private void OnDisable()
        {
            fire.action.performed -= PlayerAttacking;
            altFire.action.performed -= PlayerAltAttacking;
        }


        private void PlayerAttacking(InputAction.CallbackContext obj)
        {

            MultiFireball();
        }

        private void PlayerAltAttacking(InputAction.CallbackContext obj)
        {
                if (playerStats.PlayerUseMana(trackingFireballManaCost))
                {
                    TrackingFireball();
                }
        }


        private void Fireball()
        {
            var newProjectile = Instantiate(fireballPrefab, projectilesHolder);

            if (newProjectile.TryGetComponent(out ProjectileBase projectileBase))
            {
                projectileBase.Initalise(
                    playerTransform: cachedTransform,
                    firingUnitTransform: cachedTransform,
                    mainCameraTransform: cachedPlayerCamera,
                    projectileMoveSpeed: fireballMovespeed,
                    useRandomProjectileSpeed: false);
            }
        }


        private void MultiFireball()
        {
            float degreesBetweenFireball = (maxAngleOffset * 2f) / (multiFireballCount - 1f);
            float currentFireballDegrees = -maxAngleOffset;

            for (int i = 0; i < multiFireballCount; i++)
            {


                var newProjectile = Instantiate(fireballPrefab, projectilesHolder);

                if (newProjectile.TryGetComponent(out ProjectileBase projectileBase))
                {
                    projectileBase.Initalise(
                        playerTransform: cachedTransform,
                        firingUnitTransform: cachedTransform,
                        mainCameraTransform: cachedPlayerCamera,
                        projectileMoveSpeed: fireballMovespeed,
                        useRandomProjectileSpeed: false,
                        customRotationY: currentFireballDegrees);
                }

                currentFireballDegrees += degreesBetweenFireball;
            }
        }



        private void TrackingFireball()
        {
            var newProjectile = Instantiate(trackingFireballPrefab, projectilesHolder);

            if (newProjectile.TryGetComponent(out ProjectileBase projectileBase))
            {
                projectileBase.Initalise(
                    playerTransform: cachedTransform,
                    firingUnitTransform: cachedTransform,
                    mainCameraTransform: cachedPlayerCamera,
                    projectileMoveSpeed: trackingFireballMovespeed,
                    useRandomProjectileSpeed: false);
            }
        }
    }
}