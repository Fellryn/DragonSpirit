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
        //[SerializeField]
        PlayerInput _playerInput;
        //[SerializeField]
        InputAction attackAction;
        //[SerializeField]
        InputAction altAttackAction;
    //[SerializeField] string playerProjectileTag = "PlayerProjectile";

    [Header("Fireball Attack")]
        [SerializeField] float fireballMovespeed = 15f;
        [SerializeField] GameObject fireballPrefab;

        [Header("Tracking Fireball")]
        [SerializeField] float trackingFireballMovespeed = 17f;
        [SerializeField] GameObject trackingFireballPrefab;
        public float trackingFireballManaCost = 1f;

        [Header("Multi Fireball")]
        [SerializeField] float maxAngleOffset = 30f;

        [Header("General")]
        [SerializeField] float cooldownBetweenNormalAttack = 0.1f;
        [SerializeField] bool canNormalAttack = true;
        float timer = 0f;
        bool attackButtonHeld = false;

        private Transform cachedTransform;



        private void Start()
        {
            cachedTransform = GetComponent<Transform>();
        }


        private void OnEnable()
        {
            _playerInput = GetComponent<PlayerInput>();
            attackAction = _playerInput.actions.FindAction("Fire");
            altAttackAction = _playerInput.actions.FindAction("AltFire");

            attackAction.started += PlayerAttacking;
            attackAction.canceled += PlayerStopAttacking;
            altAttackAction.performed += PlayerAltAttacking;
        }

        private void OnDisable()
        {
            attackAction.started -= PlayerAttacking;
            attackAction.canceled -= PlayerStopAttacking;
            altAttackAction.performed -= PlayerAltAttacking;
        }

        private void Update()
        {
            if (attackButtonHeld && canNormalAttack)
            {
                PlayerPerformAttack();
            }

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
            attackButtonHeld = true;
        }

        private void PlayerStopAttacking(InputAction.CallbackContext obj)
        {
            attackButtonHeld = false;
        }

        private void PlayerPerformAttack()
        {
                canNormalAttack = false;

                if (playerPowerUps.MultiShotActive)
                {
                    MultiFireball(playerPowerUps.UseMultiShot());
                }
                else
                {
                    Fireball();
                }
        }

        private void PlayerAltAttacking(InputAction.CallbackContext obj)
        {
            if (playerPowerUps.TrackingPowerUpActive && playerPowerUps.UseTrackingAbility())
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
                    useRandomProjectileSpeed: false,
                    playerOneProjectile: playerStats.isPlayerOne);
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
                        customRotationY: currentFireballDegrees,
                        playerOneProjectile: playerStats.isPlayerOne);
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
                    useRandomProjectileSpeed: false,
                    playerOneProjectile: playerStats.isPlayerOne);
            }
        }
    }
}