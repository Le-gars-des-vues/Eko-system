using UnityEngine;

namespace TerraTiler2D
{
    public class Vector2Length_Node : Math_Node
    {
        private PortWithField<Vector2> portA;

        private Port<float> outputPort;

        public Vector2Length_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector2Length;
            SetTooltip("Returns the length of Vector2 A.");
            searchMenuEntry = new string[] { "Math", "Vector2" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector2>("A", PortDirection.Input, Vector2.zero, "A", PortCapacity.Single, false);
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
            return ((Vector2)portA.GetPortVariable()).magnitude;
        }
    }
}
