using UnityEngine;

namespace TerraTiler2D
{
    public class Vector3Dot_Node : Math_Node
    {
        private PortWithField<Vector3> portA;
        private PortWithField<Vector3> portB;

        private Port<float> outputPort;

        public Vector3Dot_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector3Dot;
            SetTooltip("Returns the dot product of Vector3 A and Vector3 B (A · B).");
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
            outputPort = GeneratePort<float>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return (float)Vector3.Dot((Vector3)portA.GetPortVariable(), (Vector3)portB.GetPortVariable());
        }
    }
}
