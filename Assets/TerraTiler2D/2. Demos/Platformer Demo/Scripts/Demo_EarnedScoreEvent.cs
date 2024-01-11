using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class Demo_EarnedScoreEvent : Event
    {
        public int score;

        public Demo_EarnedScoreEvent Init(int score)
        {
            this.score = score;

            base.Init();

            return this;
        }
    }
}