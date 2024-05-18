using UnityEngine;
using UnityEngine.Events;
using LukeMartin;

namespace KurtSingle
{
    /// <summary>
    /// Author: Kurt Single
    /// Description: This script demonstrates how to create a custom tick system in Unity
    /// All ticks can be subscribed to in the inspector or with AddListener
    /// </summary>
    public class GameTickSystem : TimerParent
    {
        // Events for each tick. Seven tick events in total - whole tick (one second) and then half that, quarter that etc.

        [SerializeField] float tickWholeInterval = 1f;
        [SerializeField] float randomTickChance = 0.25f;

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


        // The core loop. The timer runs over its interval (one second) and as it reaches 0.25, 0.5, 0.75 and 1, it runs each tick event. 
        // It then disables that tick event using a bool so it doesn't run again. 
        // When it reaches 1 it does the "TickWhole" event and resets all the event bools and timer
        protected override void Update()
        {
            base.Update();

            if (timer <= tickWholeInterval)
            {
                // Wait for 0.25 timer and then run Every Tick and Quarter Tick events
                if (timer >= tickWholeInterval * 0.25f && !onTickQuarterRun)
                {
                    OnEveryTick?.Invoke();

                    RandomTickCheck();

                    OnTickQuarter?.Invoke();
                    onTickQuarterRun = true;


                }

                // Wait for 0.5 timer and then run Every Tick and Half Tick events
                if (timer >= tickWholeInterval * 0.5f && !onTickHalfRun)
                {
                    OnEveryTick?.Invoke();
                    OnEveryHalfTick?.Invoke();

                    RandomTickCheck();

                    OnTickHalf?.Invoke();
                    onTickHalfRun = true;
                }

                // Wait for 0.75 timer and then run Every Tick and Three Quarter events
                if (timer >= tickWholeInterval * 0.75f && !onTickThreeQuarterRun)
                {
                    OnEveryTick?.Invoke();

                    RandomTickCheck();

                    OnTickThreeQuarter?.Invoke();
                    onTickThreeQuarterRun = true;
                }
            }


            //else
            //{
            //    OnEveryTick?.Invoke();
            //    OnEveryHalfTick?.Invoke();

            //    RandomTickCheck();

            //    OnTickWhole?.Invoke();
            //    onTickQuarterRun = false;
            //    onTickHalfRun = false;
            //    onTickThreeQuarterRun = false;
            //    onRandomTickRun = false;
            //}
        }

        // Once at 1 timer run Every Tick and Whole Tick events, then "reset" script
        protected override void OnTimerEnd()
        {
            OnEveryTick?.Invoke();
            OnEveryHalfTick?.Invoke();

            RandomTickCheck();

            OnTickWhole?.Invoke();
            onTickQuarterRun = false;
            onTickHalfRun = false;
            onTickThreeQuarterRun = false;
            onRandomTickRun = false;
        }


        // Every tick has a chance to run a "random tick event", useful for randomising enemy actions etc.
        // At the moment it only runs once per whole cycle to avoid spamming
        private void RandomTickCheck()
        {
            if (onRandomTickRun == true) return;

            if (UnityEngine.Random.Range(0f, 1f) < randomTickChance)
            {
                OnRandomTick?.Invoke();
            }
        }

    }

}