using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class EdgeConnectedEvent : Event
    {
        public NodeLinkData nodeLink;

        public EdgeConnectedEvent Init(NodeLinkData nodeLink)
        {
            this.nodeLink = nodeLink;

            base.Init();

            return this;
        }
    }
}
