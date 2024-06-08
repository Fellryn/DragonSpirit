using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using KurtSingle;
using DG.Tweening;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to control the constraint that lets boss look at player in Unity
    /// It uses a LookAt constraint, which the weight of is set by tweening an object between 0 and 1 
    /// </summary>
    public class EnemyBossLookConstraint : MonoBehaviour
    {
        [SerializeField] LookAtConstraint headLookConstraint;
        [SerializeField] Transform bossLookTransform;
        [SerializeField] EnemyBoss enemyBoss;
        [SerializeField] EnemyBossAnimation enemyBossAnimation;

        public bool lookingAtPlayer = false;

        [SerializeField] float totalTurnTime = 1.25f;

        [SerializeField] Transform playerOneTransform;
        [SerializeField] Transform playerTwoTransform;
        private Transform playerToTarget;

        ConstraintSource constraintSource;

        private void Start()
        {

        }

        public void LookAtPlayer()
        {
            bossLookTransform.DOScale(1f, totalTurnTime * 0.5f).SetEase(Ease.InOutCubic).OnComplete(LookAwayFromPlayer);
        }

        private void LookAwayFromPlayer()
        {
            bossLookTransform.DOScale(0f, totalTurnTime * 0.5f).SetEase(Ease.InOutCubic);
        }

        public void GetPlayerTransforms(Transform playerOneT, Transform playerTwoT)
        {
            playerOneTransform = playerOneT;
            playerTwoTransform = playerTwoT;

            //PlayerStats[] playerStatsReferences = FindObjectsByType<PlayerStats>(FindObjectsInactive.Exclude, sortMode: FindObjectsSortMode.None);

            //foreach (PlayerStats playerStats in playerStatsReferences)
            //         {
            //	if (playerStats.isPlayerOne)
            //             {
            //		playerOneTransform = playerStats.transform;
            //	}

            //	if (!playerStats.isPlayerOne)
            //             {
            //		playerTwoTransform = playerStats.transform;
            //             }
            //         }
        }


        public void SetNewLookSource()
        {
            if (playerTwoTransform == null)
            {
                return;
            }

            if (!playerOneTransform.gameObject.activeSelf)
            {
                playerToTarget = playerTwoTransform;
            }
            else if (!playerTwoTransform.gameObject.activeSelf)
            {
                playerToTarget = playerOneTransform;
            }
            else
            {
                if (playerToTarget == playerOneTransform)
                {
                    playerToTarget = playerTwoTransform;
                } else
                {
                    playerToTarget = playerOneTransform;
                }

                //if (Random.Range(0, 2) == 0)
                //{
                //    playerToTarget = playerOneTransform;
                //}
                //else
                //{
                //    playerToTarget = playerTwoTransform;
                //}
            }

            headLookConstraint.RemoveSource(0);


            constraintSource.sourceTransform = playerToTarget;
            constraintSource.weight = 1f;

            headLookConstraint.AddSource(constraintSource);
        }


        private void Update()
        {
            if (headLookConstraint.weight != bossLookTransform.localScale.y)
            {
                headLookConstraint.weight = bossLookTransform.transform.localScale.y;
            }

            if (headLookConstraint.weight >= 0.75f)
            {
                lookingAtPlayer = true;
            }
            else
            {
                lookingAtPlayer = false;
            }

            if (headLookConstraint.weight <= 0.01f)
            {
                SetNewLookSource();
            }

            //if (enemyBossNew.)
        }

    }
}