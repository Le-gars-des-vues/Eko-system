using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class NodeCreatedEvent : Event
    {
        public NodeData nodeData;

        public NodeCreatedEvent Init(NodeData nodeData)
        {
            this.nodeData = nodeData;

            base.Init();

            return this;
        }
    }
}
