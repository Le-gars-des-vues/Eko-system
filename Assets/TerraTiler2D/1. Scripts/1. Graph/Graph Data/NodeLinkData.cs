using System;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class NodeLinkData : ICloneable
    {
        public string BaseNodeGuid;
        public string BasePortGuid;
        public string TargetNodeGuid;
        public string TargetPortGuid;

        public Type PortType;

        public object Clone()
        {
            return new NodeLinkData()
            {
                BaseNodeGuid = this.BaseNodeGuid,
                BasePortGuid = this.BasePortGuid,
                TargetNodeGuid = this.TargetNodeGuid,
                TargetPortGuid = this.TargetPortGuid,
                PortType = this.PortType
            };
        }
    }
}
