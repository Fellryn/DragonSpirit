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
        [SerializeField] PlayerStats playerStats;

		[SerializeField] InputActionReference fire;
        [SerializeField] InputActionReference altFire;

        [Space(10)]
        [SerializeField] float fireballMovespeed = 15f;
        [SerializeField] float trackingFireballMovespeed = 17f;
        [SerializeField] string playerProjectileTag = "PlayerProjectile";
        [SerializeField] Transform cachedPlayerCamera;

        [SerializeField] GameObject fireballPrefab;
        [SerializeField] GameObject trackingFireballPrefab;
        [SerializeField] Transform projectilesHolder;

        bool powerupOneActive = false;

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
            if (playerStats.powerupAttackTracking)
            {
                TrackingFireball();
                playerStats.RemoveScore(2);
            } else
            {
                Fireball();
            }
        }

        private void PlayerAltAttacking(InputAction.CallbackContext obj)
        {
            //TrackingFireball();
        }


        private void Fireball()
        {


            var newProjectile = Instantiate(fireballPrefab, projectilesHolder);

            //newProjectile.GetComponent<ProjectileFireball>().cachedPlayerCamera = cachedPlayerCamera;
            //newProjectile.GetComponent<ProjectileFireball>().cachedUnitTransform = cachedTransform;

            if (newProjectile.TryGetComponent(out ProjectileBase projectileBase))
            {
                projectileBase.Initalise(
                    playerTransform: transform,
                    firingUnitTransform: transform,
                    mainCameraTransform: cachedPlayerCamera,
                    projectileMoveSpeed: fireballMovespeed);
            }


        }

        

        private void TrackingFireball()
        {
            var newProjectile = Instantiate(trackingFireballPrefab, projectilesHolder);
            //newProjectile.transform.tag = playerProjectileTag;
            //newProjectile.GetComponent<ProjectileTrackingFireball>().ProjectileMoveSpeed = 12.5f;
            //newProjectile.GetComponent<ProjectileTrackingFireball>().cachedPlayerCamera = cachedPlayerCamera;
            //newProjectile.GetComponent<ProjectileTrackingFireball>().cachedUnitTransform = cachedTransform;

            if (newProjectile.TryGetComponent(out ProjectileBase projectileBase))
            {
                projectileBase.Initalise(
                    playerTransform: transform,
                    firingUnitTransform: transform,
                    mainCameraTransform: cachedPlayerCamera,
                    projectileMoveSpeed: trackingFireballMovespeed,
                    useRandomProjectileSpeed: false);
            }
        }
    }
}