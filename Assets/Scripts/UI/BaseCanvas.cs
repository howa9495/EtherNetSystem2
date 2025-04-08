using ECM.Components;
using PlayerSystems;
using System;
using System.Collections;
using UnityEngine;

namespace canvasSystem
{
    public class BaseCanvas : MonoBehaviour
    {
        protected float timeScale = 1f;
        protected bool timeStop = false;




        private void OnEnable()
        {
            if (timeStop) TimeStop(timeStop);
        }

        private void OnDisable()
        {
            if (timeStop) TimeStop(!timeStop);
        }

        virtual public void TimeStop(bool value)
        {
            float speed = value ? 0f : timeScale;
            Time.timeScale = speed;
        }
    }

    
}