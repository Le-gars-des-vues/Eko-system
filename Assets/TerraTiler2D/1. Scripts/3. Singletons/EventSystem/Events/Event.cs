using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace TerraTiler2D
{
    [Serializable]
    public class Event
    {
        private float startTime;

        public Event Init()
        {
            //startTime = Time.time;
            return this;
        }
    }
}