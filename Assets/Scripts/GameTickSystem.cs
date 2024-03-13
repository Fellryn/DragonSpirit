using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using KurtSingle;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to create a custom tick system in Unity
    /// </summary>
    public class GameTickSystem : MonoBehaviour
    {
        [SerializeField] float tickWholeInterval = 1f;
        private float timer;

        public UnityEvent OnTickWhole;

        public UnityEvent OnTickQuarter;
        private bool onTickQuarterRun = false;

        public UnityEvent OnTickHalf;
        private bool onTickHalfRun = false;

        public UnityEvent OnTickThreeQuarter;
        private bool onTickThreeQuarterRun = false;

        public UnityEvent OnEveryTick;

        public UnityEvent OnEveryHalfTick;

        public UnityEvent OnRandomTick;
        private bool onRandomTickRun = false;

        private float randomTickChance;

        private void Update()
        {
            if (timer <= tickWholeInterval)
            {
                timer += Time.deltaTime * Time.timeScale;

                if (timer >= tickWholeInterval * 0.25f && !onTickQuarterRun)
                {
                    OnEveryTick?.Invoke();

                    RandomTickCheck();

                    OnTickQuarter?.Invoke();
                    onTickQuarterRun = true;


                }

                if (timer >= tickWholeInterval * 0.5f && !onTickHalfRun)
                {
                    OnEveryTick?.Invoke();
                    OnEveryHalfTick?.Invoke();

                    RandomTickCheck();

                    OnTickHalf?.Invoke();
                    onTickHalfRun = true;


                }

                if (timer >= tickWholeInterval * 0.75f && !onTickThreeQuarterRun)
                {
                    OnEveryTick?.Invoke();

                    RandomTickCheck();

                    OnTickThreeQuarter?.Invoke();
                    onTickThreeQuarterRun = true;
                }
            }
            else
            {
                OnEveryTick?.Invoke();
                OnEveryHalfTick?.Invoke();

                RandomTickCheck();

                OnTickWhole?.Invoke();
                onTickQuarterRun = false;
                onTickHalfRun = false;
                onTickThreeQuarterRun = false;
                onRandomTickRun = false;
                timer = 0f;

            }
        }

        private void RandomTickCheck()
        {
            if (onRandomTickRun == true) return;
            randomTickChance = 1f / 4;
            if (UnityEngine.Random.Range(0f, 1f) < randomTickChance)
            {
                OnRandomTick?.Invoke();
            }
        }

    }

}