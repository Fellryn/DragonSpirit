using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to control the boss animations in Unity
    /// It is closely linked to the constraint and "EnemyBoss" script
    /// </summary>
    public class EnemyBossAnimation : MonoBehaviour
    {
        public Animator animator;
        [SerializeField] EnemyBoss enemyBoss;

        [Header("Head State")]
        [SerializeField] int headLayer = 0;
        [SerializeField] string[] headStateNames;
        [SerializeField] float headStateTransitionTime = 0.1f;
        private int headLastIndexUsed = 0;

        [Header("Movement State")]
        //[SerializeField] int movementLayer = 1;
        //[SerializeField] string[] movementStateNames;
        //[SerializeField] float movementStateTransitionTime = 0.1f;

        [Header("Attack State")]
        [SerializeField] int attackLayer = 2;
        [SerializeField] string[] attackStateNames;
        [SerializeField] float attackStateTransitionTime = 0.1f;

        public void BossHeadMovementCompleted()
        {
            //string stateToChangeTo = headStateNames[Random.Range(0, headStateNames.Length)];

            int newHeadIndex;

            do
            {
                newHeadIndex = Random.Range(0, headStateNames.Length);


            } while (headLastIndexUsed == newHeadIndex);

            string stateToChangeTo = headStateNames[newHeadIndex];
            headLastIndexUsed = newHeadIndex;

            animator.CrossFade(stateToChangeTo, headStateTransitionTime, headLayer);
        }


        public void SetMoveBool(bool moving)
        {
            animator.SetBool("IsMoving", moving);
            if (moving) animator.CrossFade("SwimMove", 0.05f);

            
        }


        public void AttackBegun()
        {
            animator.CrossFade("AttackOpen", attackStateTransitionTime, attackLayer);
            //animator.CrossFade("AttackTwo", attackStateTransitionTime, attackLayer);
            animator.SetBool("Attacking", true);
        }


        public void AttackCompleted()
        {
            animator.SetBool("Attacking", false);
            animator.CrossFade("AttackClose", 0.1f, attackLayer);
            //enemyBossNew.canAttack = true;
        }


        public bool CheckAttackState()
        {
            return animator.GetBool("Attacking");
        }

    }
}