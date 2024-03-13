using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Cinemachine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to manage packs of enemies by using a spline in Unity
	/// </summary>
	/// 
	public class EnemySplineGroup : MonoBehaviour 
	{
		[SerializeField] CinemachineSplineCart splineCart;
		[SerializeField] CinemachineSplineDolly playerDollyCamera;
        [SerializeField] Transform enemyHolder;
        [SerializeField] float stopDistanceFromPlayer = 0.05f;
        
        public float moveSpeed = 0.01f;
        [SerializeField] GameObject enemyTypePrefab;
        [SerializeField] int numberOfEnemies;
        [SerializeField] Vector3 Offset = new Vector3(2f, -1f, 1.5f);
        [SerializeField] List<GameObject> enemyPack = new List<GameObject>();

        private SplineAutoDolly.FixedSpeed autodolly;


        private void Start()
        {
            autodolly = splineCart.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;

            for (int i = 0; i < numberOfEnemies; i++)
            {
                GameObject newEnemy = Instantiate(enemyTypePrefab, transform.position, transform.rotation, enemyHolder);
                enemyPack.Add(newEnemy);
                newEnemy.GetComponent<EnemyBase>().initialOffset = Offset * (i + 1);
                newEnemy.GetComponent<EnemyBase>().splineCart = this.splineCart;
            }

        }


        private void Update()
        {
            
            if (splineCart.SplinePosition < playerDollyCamera.CameraPosition + stopDistanceFromPlayer)
            {
                autodolly.Speed = moveSpeed;
                LetGroupAttack(true);
            } else
            {
                autodolly.Speed = -moveSpeed;
            }
        }


        public void LetGroupAttack(bool foo)
        {
            for (int i = 0; i < enemyPack.Count; i++)
            {
                if (enemyPack[i] != null)
                {
                    enemyPack[i].GetComponent<EnemyBase>().CanAttack = true;
                }
            }
        }


    }
}