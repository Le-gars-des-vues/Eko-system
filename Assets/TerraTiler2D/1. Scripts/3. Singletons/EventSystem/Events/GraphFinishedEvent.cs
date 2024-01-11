using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class GraphFinishedEvent : Event
    {
        public GraphOutput generatedOutput;

        public GraphFinishedEvent Init(GraphOutput generatedOutput)
        {
            this.generatedOutput = generatedOutput;

            base.Init();

            return this;
        }
    }
}