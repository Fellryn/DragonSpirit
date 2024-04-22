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
        [SerializeField] PlayerPowerups playerPowerUps;
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

        [Header("General")]
        [SerializeField] float cooldownBetweenNormalAttack = 0.1f;
        [SerializeField] bool canNormalAttack = true;
        float timer = 0f;

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

        private void Update()
        {
            if (timer >= cooldownBetweenNormalAttack && !canNormalAttack)
            {
                canNormalAttack = true;
                timer = 0;
            } else if (!canNormalAttack)
            {
                timer += Time.deltaTime;
            }
        }


        private void PlayerAttacking(InputAction.CallbackContext obj)
        {
            if (canNormalAttack)
            {
                canNormalAttack = false;

                if (playerPowerUps.MultiShotActive)
                {
                    MultiFireball(playerPowerUps.UseMultiShot());
                } else
                {
                    Fireball();
                }
                
            }
            
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


        private void MultiFireball(int count)
        {
            float degreesBetweenFireball = (maxAngleOffset * 2f) / (count - 1f);
            float currentFireballDegrees = -maxAngleOffset;

            for (int i = 0; i < count; i++)
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