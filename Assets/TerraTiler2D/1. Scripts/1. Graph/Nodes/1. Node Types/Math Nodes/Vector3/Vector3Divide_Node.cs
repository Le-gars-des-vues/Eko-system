using UnityEngine;

namespace TerraTiler2D
{
    public class Vector3Divide_Node : Math_Node
    {
        private PortWithField<Vector3> portA;
        private PortWithField<float> portB;

        private Port<Vector3> outputPort;

        public Vector3Divide_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector3Divide;
            SetTooltip("Returns Vector3 A divided by float B.");
            searchMenuEntry = new string[] { "Math", "Vector3" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector3>("A", PortDirection.Input, Vector3.zero, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<float>("B", PortDirection.Input, 0, "B", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector3>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return (Vector3)((Vector3)portA.GetPortVariable() / (float)portB.GetPortVariable());
        }
    }
}
