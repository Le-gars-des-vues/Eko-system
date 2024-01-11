using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class PortDeletedEvent : Event
    {
        public PortData portData;

        public PortDeletedEvent Init(PortData portData)
        {
            this.portData = portData;

            base.Init();

            return this;
        }
    }
}
