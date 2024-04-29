using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to control animation of enemy units in Unity
	/// </summary>
	public class EnemyAnimation : MonoBehaviour 
	{
		private Animator animator;

		/// <summary>
		/// Make sure to name either: Bat, Dinosaur, Plant, Wyvern, SeaMonster or Boss
		/// </summary>
		[SerializeField]
		string enemyName;
		[SerializeField]
		float maximumSpeedVariation = 0.2f;
		[SerializeField]
		float maximumOffset = 1f;

		private Rigidbody cachedRigidbody;

        private void Start()
        {
			animator = GetComponentInChildren<Animator>();
			cachedRigidbody = GetComponent<Rigidbody>();

            switch (enemyName)
            {
				case "Bat":
					FlyerAnimation();
					break;
				case "Wyvern":
					FlyerAnimation();
					break;
				case "Dinosaur":
					DinosaurAnimation();
					break;
				case "SeaMonster":
					DinosaurAnimation();
					break;
				default:

					break;
            }
        }


        private void Update()
        {
            if (enemyName == "Dinosaur" || enemyName == "SeaMonster")
            {
				DinosaurAnimation();
            }
        }


		public void AttackBegun()
        {
			animator.CrossFade("Attack", 0.1f, 0);
			animator.SetBool("Attacking", true);
        }

		public void AttackCompleted()
        {
			animator.SetBool("Attacking", false);
        }


        private void FlyerAnimation()
        {
			FlyingLoopRandomiser();
        }


		private void DinosaurAnimation()
        {
			if (animator.GetBool("Attacking") == false)
			{
				if (cachedRigidbody.angularVelocity.y <= -0.01f)
                {
					animator.SetBool("TurningLeft", true);
					animator.SetBool("TurningRight", false);
				}

				if (cachedRigidbody.angularVelocity.y >= 0.01f)
				{
					animator.SetBool("TurningLeft", false);
					animator.SetBool("TurningRight", true);
				}

				//if (cachedRigidbody.angularVelocity.y <= -0.01f && !turningRight)
				//{
				//	animator.CrossFade("TurnRight", 0.1f, 0);
				//	turningRight = true;
				//	turningLeft = false;
				//}
				//else if (cachedRigidbody.angularVelocity.y >= 0.01f && !turningLeft)
				//{
				//	animator.CrossFade("TurnLeft", 0.1f, 0);
				//	turningRight = false;
				//	turningLeft = true;
				//}
			}
        }




		private void FlyingLoopRandomiser()
        {
			float animSpeed = 1 + Random.Range(-maximumSpeedVariation, maximumSpeedVariation);
			animator.SetFloat("Speed", animSpeed);

			float animOffset = Random.Range(0f, maximumOffset);
			animator.SetFloat("Offset", animOffset);
		}

		public void OnDeath()
        {
			animator.enabled = false;
        }

    }
}