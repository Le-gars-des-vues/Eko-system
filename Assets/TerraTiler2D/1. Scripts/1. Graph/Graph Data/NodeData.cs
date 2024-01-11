using System;
using UnityEngine;

namespace TerraTiler2D
{
    [Serializable]
    public class NodeData : ICloneable
    {
        public string GUID;
        public string NodeName;
        public Serializable.Vector2 Position;
        public Glob.NodeTypes NodeType;

        public virtual void CopyNodeDataFrom(NodeData otherData)
        {
            GUID = otherData.GUID;
            NodeName = otherData.NodeName;
            Position = otherData.Position;
            NodeType = otherData.NodeType;
        }

        public virtual object Clone()
        {
            return new NodeData()
            {
                GUID = this.GUID,
                NodeName = this.NodeName,
                Position = this.Position,
                NodeType = this.NodeType,
            };
        }
    }

    [Serializable]
    public class Preview_NodeData : NodeData
    {
        public bool PreviewToggle;

        public override void CopyNodeDataFrom(NodeData otherData)
        {
            base.CopyNodeDataFrom(otherData);

            PreviewToggle = (otherData as Preview_NodeData).PreviewToggle;
        }

        public override object Clone()
        {
            return new Preview_NodeData()
            {
                GUID = this.GUID,
                NodeName = this.NodeName,
                Position = this.Position,
                NodeType = this.NodeType,
                PreviewToggle = this.PreviewToggle,
            };
        }
    }
}