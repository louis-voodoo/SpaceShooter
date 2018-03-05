using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Voodoo
{
    public class TimerManager : MonoBehaviour
    {
        private static GameObject timerManager;
        private static List<TimerBase> timerList;

        public static void Initialize()
        {
            if (timerManager != null) return;

            timerManager = new GameObject("Timer Manager");
            timerManager.AddComponent<TimerManager>();
            timerList = new List<TimerBase>();
        }

        public static Timer NewTimer(float intervalValue)
        {
            Initialize();

            Timer newTimer = new Timer(intervalValue);
            timerList.Add(newTimer);
            return newTimer;
        }

        protected void Update()
        {
            float deltaTime = Time.deltaTime;
            List<TimerBase> tmpTimerList = new List<TimerBase>(timerList);

            for (int i = 0; i < tmpTimerList.Count; ++i)
            {
                TimerBase timer = tmpTimerList[i];
                if (timer.Enabled == true)
                {
                    if (timer.Update(deltaTime) == false)
                    {
                        timerList.Remove(timer);
                    }
                }
            }
        }
    }

    public abstract class TimerBase
    {
        private bool enabled;
        public bool Enabled
        {
            get { return this.enabled; }
            set
            {
                if (value == this.enabled) return;   
                this.enabled = value;
                if (this.enabled == true) this.Started.Raise();
                else this.Elapsed.Raise();
            }
        }
        public float Interval;
        public bool AutoReset;
        public bool AutoDestroy;

        public event Action Started;
        public event Action<float> Updating;
        public event Action Elapsed;

        protected float timer;

        public virtual bool Update(float deltaTime)
        {
            this.timer += deltaTime;

            float value01 = this.timer.Convert01(0.0f, this.Interval);
            if (value01 > 1.0f) value01 = 1.0f;
            this.Updating.Raise(value01);

            if (this.timer > this.Interval) this.Enabled = false;

            return true;
        }

        public void Start()
        {
            this.Enabled = true;
        }

        public void Restart()
        {
            this.Enabled = true;
            this.timer = 0.0f;
        }

        public void Stop()
        {
            this.Enabled = false;
        }
    }

    public class Timer : TimerBase
    {
        public Timer(float intervalValue)
        {
            this.Interval = intervalValue;
        }

        public override bool Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (this.Enabled == false)
            {
                if (this.AutoReset == true)
                {
                    this.Restart();
                }
                else if (this.AutoDestroy == true)
                {
                    return false;
                }
            }
            return true;
        }
    }

    public static class ExtensionMethods
    {
        public static float Convert01(this float floatValue, float originalStart, float originalEnd)
        {
            float scale = 1.0f / (originalEnd - originalStart);
            return (floatValue - originalStart) * scale;
        }

        public static void Raise(this Action eventValue)
        {
            if (eventValue != null) eventValue();
        }

        public static void Raise<T>(this Action<T> eventValue, T parameter)
        {
            if (eventValue != null) eventValue(parameter);
        }
    }
}