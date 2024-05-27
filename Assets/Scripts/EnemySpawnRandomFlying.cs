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

        [SerializeField] int maxWyvernsToSpawn = 4;
        private int wyvernsPerWave = 0;
        private int wyvernsToSpawn = 0;
        private int wyvernsCooldown;
        [SerializeField] Transform cachedPlayerTransform;

        private void OnEnable()
        {
			gameTickSystem.OnEveryHalfTick.AddListener(AddTick);
        }

		private void OnDisable()
		{
			gameTickSystem.OnEveryHalfTick.RemoveListener(AddTick);
		}

		private void AddTick()
        {
			if (ticksRun >= ticksBetweenSpawns)
            {
				ChooseSpawn();
				ticksRun = 0;
            } else
            {


				ticksRun++;
                if (wyvernsToSpawn > 0)
                {
                    SpawnEnemy(1);
                    wyvernsToSpawn--;
                }

                if (wyvernsCooldown > 0)
                {
                    wyvernsCooldown--;
                }
            }
        }


		private void ChooseSpawn()
        {
			if (gameVars.AllowEnemySpawn)
            {
                if (randomEnemyHolder.childCount >= maxRandomEnemies) return;

                int randomEnemyIndex = Random.Range(0, flyingEnemyPrefab.Length);

                if (randomEnemyIndex == 1 && wyvernsCooldown == 0)
                {
                    if (wyvernsPerWave < maxWyvernsToSpawn)
                    {
                        wyvernsPerWave++;
                    }
                    wyvernsToSpawn = wyvernsPerWave;
                    wyvernsCooldown = 20;
                    SpawnEnemy(1);
                }
                else
                {
                    SpawnEnemy(randomEnemyIndex);
                }
            }
        }

        private void SpawnEnemy(int randomEnemyIndex)
        {
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