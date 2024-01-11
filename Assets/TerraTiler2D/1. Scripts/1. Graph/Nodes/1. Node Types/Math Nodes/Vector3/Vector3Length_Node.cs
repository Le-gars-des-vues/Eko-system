using UnityEngine;

namespace TerraTiler2D
{
    public class Vector3Length_Node : Math_Node
    {
        private PortWithField<Vector3> portA;

        private Port<float> outputPort;

        public Vector3Length_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector3Length;
            SetTooltip("Returns the length of Vector3 A.");
            searchMenuEntry = new string[] { "Math", "Vector3" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector3>("A", PortDirection.Input, Vector3.zero, "A", PortCapacity.Single, false);
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
            return ((Vector3)portA.GetPortVariable()).magnitude;
        }
    }
}
