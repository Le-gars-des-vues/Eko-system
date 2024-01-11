using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class PortCreatedEvent : Event
    {
        public PortData portData;

        public PortCreatedEvent Init(PortData portData)
        {
            this.portData = portData;

            base.Init();

            return this;
        }
    }
}
