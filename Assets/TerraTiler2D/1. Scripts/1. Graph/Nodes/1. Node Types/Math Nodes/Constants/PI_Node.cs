using UnityEngine;

namespace TerraTiler2D
{
    public class PI_Node : Node
    {
        private Port<float> outputPort;

        public PI_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.PI;
            SetTooltip("Returns PI.");
            searchMenuEntry = new string[] { "Math", "Constants" };
        }

        protected override void InitializeInputPorts()
        {

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
            return Mathf.PI;
        }
    }
}
