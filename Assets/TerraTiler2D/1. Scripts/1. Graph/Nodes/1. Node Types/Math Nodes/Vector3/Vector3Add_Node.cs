using UnityEngine;

namespace TerraTiler2D
{
    public class Vector3Add_Node : Math_Node
    {
        private PortWithField<Vector3> portA;
        private PortWithField<Vector3> portB;

        private Port<Vector3> outputPort;

        public Vector3Add_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector3Add;
            SetTooltip("Adds Vector3 A to Vector3 B.");
            searchMenuEntry = new string[] { "Math", "Vector3" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector3>("A", PortDirection.Input, Vector3.zero, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<Vector3>("B", PortDirection.Input, Vector3.zero, "B", PortCapacity.Single, false);
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
            return (Vector3)portA.GetPortVariable() + (Vector3)portB.GetPortVariable();
        }
    }
}
