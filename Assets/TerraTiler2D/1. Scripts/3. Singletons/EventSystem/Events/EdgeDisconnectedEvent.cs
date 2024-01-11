using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class EdgeDisconnectedEvent : Event
    {
        public NodeLinkData nodeLink;

        public EdgeDisconnectedEvent Init(NodeLinkData nodeLink)
        {
            this.nodeLink = nodeLink;

            base.Init();

            return this;
        }
    }
}
