using UnityEngine;

namespace TerraTiler2D
{
    public class Vector4Length_Node : Math_Node
    {
        private PortWithField<Vector4> portA;

        private Port<float> outputPort;

        public Vector4Length_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector4Length;
            SetTooltip("Returns the length of Vector4 A.");
            searchMenuEntry = new string[] { "Math", "Vector4" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector4>("A", PortDirection.Input, Vector4.zero, "A", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<float>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return ((Vector4)portA.GetPortVariable()).magnitude;
        }
    }
}
