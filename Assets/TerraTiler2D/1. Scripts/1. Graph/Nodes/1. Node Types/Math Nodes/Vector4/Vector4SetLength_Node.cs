using UnityEngine;

namespace TerraTiler2D
{
    public class Vector4SetLength_Node : Math_Node
    {
        private PortWithField<Vector4> portA;
        private PortWithField<float> portB;

        private Port<Vector4> outputPort;

        public Vector4SetLength_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector4SetLength;
            SetTooltip("Scales Vector4 A to length of float B.");
            searchMenuEntry = new string[] { "Math", "Vector4" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector4>("A", PortDirection.Input, Vector4.zero, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<float>("B", PortDirection.Input, 0, "B", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector4>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return ((Vector4)portA.GetPortVariable()).normalized * ((float)portB.GetPortVariable());
        }
    }
}
