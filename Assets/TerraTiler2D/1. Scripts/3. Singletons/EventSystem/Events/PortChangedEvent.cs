using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class PortChangedEvent : Event
    {
        public PortData portData;

        public PortChangedEvent Init(PortData portData)
        {
            this.portData = portData;

            base.Init();

            return this;
        }
    }
}
