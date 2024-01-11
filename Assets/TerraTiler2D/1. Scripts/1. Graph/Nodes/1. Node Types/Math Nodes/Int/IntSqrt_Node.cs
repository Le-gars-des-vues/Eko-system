using UnityEngine;

namespace TerraTiler2D
{
    public class IntSqrt_Node : Math_Node
    {
        private PortWithField<int> portA;

        private Port<float> outputPort;

        public IntSqrt_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IntSqrt;
            SetTooltip("Returns the square root of integer A.");
            searchMenuEntry = new string[] { "Math", "int" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<int>("A", PortDirection.Input, 0, "A", PortCapacity.Single, false);
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
            return (float)Mathf.Sqrt((int)portA.GetPortVariable());
        }
    }
}
