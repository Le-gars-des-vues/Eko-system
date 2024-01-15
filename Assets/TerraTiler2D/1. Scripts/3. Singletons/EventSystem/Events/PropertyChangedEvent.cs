using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class PropertyChangedEvent : Event
    {
        public PropertyData_Abstract propertyData;

        public PropertyChangedEvent Init(PropertyData_Abstract propertyData)
        {
            this.propertyData = propertyData;

            base.Init();

            return this;
        }
    }
}