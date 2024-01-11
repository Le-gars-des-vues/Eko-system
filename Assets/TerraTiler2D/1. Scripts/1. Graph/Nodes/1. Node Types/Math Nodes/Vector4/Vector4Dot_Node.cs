using UnityEngine;

namespace TerraTiler2D
{
    public class Vector4Dot_Node : Math_Node
    {
        private PortWithField<Vector4> portA;
        private PortWithField<Vector4> portB;

        private Port<float> outputPort;

        public Vector4Dot_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector4Dot;
            SetTooltip("Returns the dot product of Vector4 A and Vector4 B (A · B).");
            searchMenuEntry = new string[] { "Math", "Vector3" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector4>("A", PortDirection.Input, Vector4.zero, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<Vector4>("B", PortDirection.Input, Vector4.zero, "B", PortCapacity.Single, false);
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
            return (float)Vector3.Dot((Vector4)portA.GetPortVariable(), (Vector4)portB.GetPortVariable());
        }
    }
}
