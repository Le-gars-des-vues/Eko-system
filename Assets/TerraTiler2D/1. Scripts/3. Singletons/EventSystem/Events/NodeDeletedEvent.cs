using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class NodeDeletedEvent : Event
    {
        public NodeData nodeData;

        public NodeDeletedEvent Init(NodeData nodeData)
        {
            this.nodeData = nodeData;

            base.Init();

            return this;
        }
    }
}
