using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KurtSingle;

namespace KurtSingle
{
	/// <summary>
	/// Author: Kurt Single
	/// Description: This script demonstrates how to keep spawning random flying enemies in Unity
	/// </summary>
	public class EnemySpawnRandomFlying : MonoBehaviour 
	{
		[SerializeField] GameVars gameVars;
		[SerializeField] GameTickSystem gameTickSystem;
		[SerializeField] GameObject[] flyingEnemyPrefab;
		[SerializeField] Transform randomEnemyHolder;
		[SerializeField] int maxRandomEnemies = 10;

		[SerializeField] float distanceSpawnRange = 60f;
		[SerializeField] float distanceXSpawn = 30f;
		[SerializeField] float distanceYSpawn = 6f;

		[SerializeField] Vector3 moveOffsetMin;
		[SerializeField] Vector3 moveOffsetMax;

		[SerializeField] int ticksBetweenSpawns = 10;
		private int ticksRun = 0;

		[SerializeField] Transform cachedPlayerTransform;

        private void OnEnable()
        {
			gameTickSystem.OnRandomTick.AddListener(AddTick);
        }

		private void OnDisable()
		{
			gameTickSystem.OnRandomTick.RemoveListener(AddTick);
		}

		private void AddTick()
        {
			if (ticksRun >= ticksBetweenSpawns)
            {
				SpawnEnemies();
				ticksRun = 0;
            } else
            {
				ticksRun += 1;
            }
        }


		private void SpawnEnemies()
        {
			if (gameVars.AllowEnemySpawn)
            {
				if (randomEnemyHolder.childCount >= maxRandomEnemies) return;

				int randomEnemyIndex = Random.Range(0, flyingEnemyPrefab.Length);

				Vector3 spawnLocation = new Vector3(distanceXSpawn * Random.Range(-1f, 1f), distanceYSpawn * Random.Range(-1f, 0f), distanceSpawnRange) + cachedPlayerTransform.position;
				var newEnemy = Instantiate(flyingEnemyPrefab[randomEnemyIndex], spawnLocation, Quaternion.identity, randomEnemyHolder);
				if (newEnemy.TryGetComponent<EnemyMobile>(out EnemyMobile enemyMobile))
				{
					enemyMobile.ChangeMoveTarget(new Vector3(Random.Range(moveOffsetMin.x, moveOffsetMax.x), Random.Range(moveOffsetMin.y, moveOffsetMax.y), Random.Range(moveOffsetMin.z, moveOffsetMax.z)));
					enemyMobile.useSpline = false;
                }
            }
        }
		
	}
}