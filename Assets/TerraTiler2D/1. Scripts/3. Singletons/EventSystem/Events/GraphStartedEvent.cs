using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class GraphStartedEvent : Event
    {
        new public GraphStartedEvent Init()
        {
            base.Init();

            return this;
        }
    }
}