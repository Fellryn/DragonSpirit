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

        public UnityEvent OnEveryTickInterval;

        private void Update()
        {
            if (timer <= tickWholeInterval)
            {
                timer += Time.deltaTime * Time.timeScale;

                if (timer >= tickWholeInterval * 0.25f && !onTickQuarterRun)
                {
                    OnEveryTickInterval?.Invoke();

                    OnTickQuarter?.Invoke();
                    onTickQuarterRun = true;


                }

                if (timer >= tickWholeInterval * 0.5f && !onTickHalfRun)
                {
                    OnEveryTickInterval?.Invoke();

                    OnTickHalf?.Invoke();
                    onTickHalfRun = true;


                }

                if (timer >= tickWholeInterval * 0.75f && !onTickThreeQuarterRun)
                {
                    OnEveryTickInterval?.Invoke();

                    OnTickThreeQuarter?.Invoke();
                    onTickThreeQuarterRun = true;
                }
            }
            else
            {
                OnEveryTickInterval?.Invoke();

                OnTickWhole?.Invoke();
                onTickQuarterRun = false;
                onTickHalfRun = false;
                onTickThreeQuarterRun = false;
                timer = 0f;

            }
        }

    }
}