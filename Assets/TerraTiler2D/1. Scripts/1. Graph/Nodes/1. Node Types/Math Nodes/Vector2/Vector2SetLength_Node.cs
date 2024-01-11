using UnityEngine;

namespace TerraTiler2D
{
    public class Vector2SetLength_Node : Math_Node
    {
        private PortWithField<Vector2> portA;
        private PortWithField<float> portB;

        private Port<Vector2> outputPort;

        public Vector2SetLength_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector2SetLength;
            SetTooltip("Scales Vector2 A to length of float B.");
            searchMenuEntry = new string[] { "Math", "Vector2" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector2>("A", PortDirection.Input, Vector2.zero, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<float>("B", PortDirection.Input, 0, "B", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector2>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return ((Vector2)portA.GetPortVariable()).normalized * ((float)portB.GetPortVariable());
        }
    }
}
