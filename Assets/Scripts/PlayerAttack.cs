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
		[SerializeField] InputActionReference fire;
        [SerializeField] InputActionReference altFire;

        [Space(10)]
        //[SerializeField] float fireballCooldown = 0.5f;
        //[SerializeField] float fireballMoveSpeed = 0.03f;
        [SerializeField] string playerProjectileTag = "PlayerProjectile";
        [SerializeField] Transform cachedPlayerCamera;

        [SerializeField] GameObject fireballPrefab;
        [SerializeField] GameObject trackingFireballPrefab;
        [SerializeField] Transform projectilesHolder;

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
            Fireball();
            
        }

        private void PlayerAltAttacking(InputAction.CallbackContext obj)
        {
            TrackingFireball();
        }

        private void Fireball()
        {
            var newProjectile = Instantiate(fireballPrefab, projectilesHolder);
            newProjectile.transform.tag = playerProjectileTag;
            newProjectile.GetComponent<ProjectileFireball>().cachedPlayerCamera = cachedPlayerCamera;
            newProjectile.GetComponent<ProjectileFireball>().cachedUnitTransform = cachedTransform;

        }

        private void TrackingFireball()
        {
            var newProjectile = Instantiate(trackingFireballPrefab, projectilesHolder);
            newProjectile.transform.tag = playerProjectileTag;
            newProjectile.GetComponent<ProjectileTrackingFireball>().ProjectileMoveSpeed = 10f;
            newProjectile.GetComponent<ProjectileTrackingFireball>().cachedPlayerCamera = cachedPlayerCamera;
            newProjectile.GetComponent<ProjectileTrackingFireball>().cachedUnitTransform = cachedTransform;
        }
    }
}