using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
        [SerializeField] GameVars gameVars;

        [SerializeField] GameTickSystem gameTickSystem;

		[SerializeField] CinemachineSplineCart splineCart;
		[SerializeField] CinemachineSplineDolly playerDollyCamera;
        [SerializeField] Transform enemyHolder;


        [Header("Setup")]
        [SerializeField] float stopDistanceFromPlayer = 0.05f;
        public float moveSpeed = 0.005f;
        [SerializeField] GameObject enemyTypePrefab;
        [SerializeField] int maxNumberOfEnemies = 5;
        [SerializeField] int numberToIncreasePerWave = 1;
        [SerializeField] int numberOfEnemies = 1;

        [SerializeField] Vector3 wholeGroupOffsetMax;
        [SerializeField] Vector3 offsetMin = new Vector3(2f, -1f, 1f);
        [SerializeField] Vector3 offsetMax = new Vector3(4f, -3f, 3f);
        [SerializeField] List<GameObject> enemyPack = new List<GameObject>();


        [Header("Detaching")]
        [SerializeField] float stayTime = 15f;
        private float currentTicks = 0f;

        [SerializeField] Transform enemyRandomHolder;
        [SerializeField] Vector3 moveOffsetMin;
        [SerializeField] Vector3 moveOffsetMax;

        private SplineAutoDolly.FixedSpeed autodolly;


        private void OnEnable()
        {
            gameTickSystem.OnEveryHalfTick.AddListener(DoChecks);
        }


        private void OnDisable()
        {
            gameTickSystem.OnEveryHalfTick.RemoveListener(DoChecks);
        }


        private void Start()
        {
            autodolly = splineCart.AutomaticDolly.Method as SplineAutoDolly.FixedSpeed;

            gameVars.EnemySpawn(true);

            CreatePack();

        }


        private void CreatePack()
        {
            if (!gameVars.AllowEnemySpawn) return;

            currentTicks = 0;

            if (numberOfEnemies < maxNumberOfEnemies)
            {
                numberOfEnemies += numberToIncreasePerWave;
            }

            Vector3 randomGroupOffset = new Vector3(Random.Range(0, wholeGroupOffsetMax.x), Random.Range(0, wholeGroupOffsetMax.y), Random.Range(0, wholeGroupOffsetMax.z));
            Vector3 randomUnitOffset = new Vector3(Random.Range(offsetMin.x, offsetMax.x), Random.Range(offsetMin.y, offsetMax.y), Random.Range(offsetMin.z, offsetMax.z));

            for (int i = 0; i < numberOfEnemies; i++)
            {
                GameObject newEnemy = Instantiate(enemyTypePrefab, transform.position, transform.rotation, enemyHolder);
                enemyPack.Add(newEnemy);
                newEnemy.GetComponent<EnemyMobile>().initialMoveTarget = randomGroupOffset + (randomUnitOffset * (i + 1));
                newEnemy.GetComponent<EnemyMobile>().useSpline = true;
                newEnemy.GetComponent<EnemyMobile>().splineCart = splineCart;
            }
        }


        public void DoChecks()
        {
            DistanceFromPlayerCheck();
            GroupStatusCheck();
            TimerCheck();
        }


        private void DistanceFromPlayerCheck()
        {
            if (splineCart.SplinePosition < playerDollyCamera.CameraPosition + stopDistanceFromPlayer)
            {
                autodolly.Speed = moveSpeed;
                LetGroupAttack(true);
            }
            else
            {
                autodolly.Speed = -moveSpeed * 2;
            }
        }


        private void GroupStatusCheck()
        {
            int packCount = enemyPack.Count;

            for (int i = 0; i < enemyPack.Count; i++)
            {
                if (enemyPack[i] == null) packCount -= 1;
            }

            if (packCount <= 0) ResetPackAndSpline();
        }

        private void TimerCheck()
        {
            if (currentTicks >= stayTime * 2)
            {
                for (int i = 0; i < enemyPack.Count; i++)
                {
                    if (enemyPack[i] != null)
                    {
                        if (enemyPack[i].TryGetComponent<EnemyMobile>(out EnemyMobile enemyMobile))
                        {
                            enemyPack[i].transform.SetParent(enemyRandomHolder);
                            enemyMobile.ChangeMoveTarget(new Vector3(Random.Range(moveOffsetMin.x, moveOffsetMax.x), Random.Range(moveOffsetMin.y, moveOffsetMax.y), Random.Range(moveOffsetMin.z, moveOffsetMax.z)));
                            enemyMobile.useSpline = false;
                            enemyMobile.DoPlayerDistanceCheck();
                        }
                    }
                }

                ResetPackAndSpline();

            }
            else
            {
                currentTicks += 1;
            }
        }


        private async void ResetPackAndSpline()
        {
            enemyPack.Clear();
            splineCart.SplinePosition = playerDollyCamera.CameraPosition + 0.1f;

            await Task.Delay(10);

            CreatePack();
        }


        private void LetGroupAttack(bool foo)
        {
            for (int i = 0; i < enemyPack.Count; i++)
            {
                if (enemyPack[i] != null)
                {
                    //if (LetGroupMoveSideways) enemyPack[i].GetComponent<EnemyBat>().BeginSidewaysMovement();
                    enemyPack[i].GetComponent<EnemyRangedAttack>().canAttack = true;
                }
            }


        }




    }
}