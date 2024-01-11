using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class NodeChangedEvent : Event
    {
        public NodeData nodeData;

        public NodeChangedEvent Init(NodeData nodeData)
        {
            this.nodeData = nodeData;

            base.Init();

            return this;
        }
    }
}
