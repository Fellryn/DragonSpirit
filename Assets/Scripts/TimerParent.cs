using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LukeMartin;

namespace LukeMartin
{
    /// <summary>
    /// Author: Luke Martin
    /// Description: This timer parent script is used to demonstrate the four main concepts related to OOP
    /// it provides basic timer functionality and serves as the base class for specific timer implementations
    /// </summary>
    public abstract class TimerParent : MonoBehaviour
    {
        public float timer { get; private set; }
        [SerializeField] float timerInterval = 1f;
        [SerializeField] float minTimeCap = 0.1f;
        [SerializeField] bool reduceTimer = false;
        [SerializeField] private float decreaseTimeCapBy;
        [SerializeField] float secondsUntilMinTime = 1f;

        protected virtual void Start()
        {
            timer -= timerInterval;
            if (reduceTimer)
            {
                decreaseTimeCapBy = (timerInterval - minTimeCap) / secondsUntilMinTime;
                InvokeRepeating("DecreaseTimerInterval", 1f, 1f);
            }
        }
        protected virtual void Update()
        {
            if (timer >= timerInterval)
            {
                OnTimerEnd();
                ResetTimer();

                if (timerInterval <= minTimeCap)
                {
                    CancelInvoke();
                    timerInterval = minTimeCap;
                }
            }
            else
            {
                timer += Time.deltaTime;
            }
        }
        protected abstract void OnTimerEnd();
        protected virtual void ResetTimer()
        {
            timer -= timerInterval;
        }
        protected virtual void DecreaseTimerInterval()
        {
            timerInterval -= decreaseTimeCapBy;
        }
    }
}