using System;
using UnityEngine;
using UnityEngine.UIElements;
#if (UNITY_EDITOR)
using UnityEditor.UIElements;
#endif

namespace TerraTiler2D
{
    [Serializable]
    public class NoisemapDimension_NodeData : NodeData
    {
        public NoisemapDimension value;

        public override object Clone()
        {
            return new NoisemapDimension_NodeData()
            {
                GUID = this.GUID,
                NodeName = this.NodeName,
                Position = this.Position,
                NodeType = this.NodeType,
                value = this.value,
            };
        }
    }

    public enum NoisemapDimension
    {
        OnlyX,
        OnlyY,
        BothXAndY
    }

    public class NoisemapDimension_Node : Node
    {
        private NoisemapDimension value;
#if (UNITY_EDITOR)
        private EnumField valueField;
#endif

        public NoisemapDimension_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.NoisemapDimension;
            SetTooltip("Along which axes should the target noisemap generate noise.");
            searchMenuEntry = new string[] { "Texture2D", "Noise" };
        }

        protected override void InitializeInputPorts()
        {
#if (UNITY_EDITOR)
            valueField = new EnumField(value);
            valueField.RegisterValueChangedCallback(evt => SetValue((NoisemapDimension)evt.newValue));

            GetContainer(NodeContainers.InputContainer).Add(valueField);
#endif
        }

        protected override void InitializeOutputPorts()
        {
            //Create an input port.
            Port<NoisemapDimension> noisemapDimensionPort = GeneratePort<NoisemapDimension>("Dimension", PortDirection.Output, "NoisemapDimension", PortCapacity.Multi, false, "Along which axes should the target noisemap generate noise.");
            noisemapDimensionPort.SetOutputPortMethod(GetDimensionOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetDimensionOutput()
        {
            return value;
        }

        public void SetValue(NoisemapDimension newValue)
        {
            value = newValue;

#if (UNITY_EDITOR)
            valueField.SetValueWithoutNotify(value);
#endif
        }

        public override NodeData GetNodeData(NodeData nodeData = null)
        {
            return base.GetNodeData(new NoisemapDimension_NodeData()
            {
                value = this.value
            });
        }
        public override void LoadNodeData(NodeData data)
        {
            NoisemapDimension_NodeData nodeData = data as NoisemapDimension_NodeData;

            if (nodeData != null)
            {
                //if (nodeData.value != null)
                //{
                    SetValue(nodeData.value);
                //}
            }
        }
    }
}
