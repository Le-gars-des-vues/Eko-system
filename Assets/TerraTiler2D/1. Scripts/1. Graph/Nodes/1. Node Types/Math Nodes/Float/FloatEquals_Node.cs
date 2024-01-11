using UnityEngine;

namespace TerraTiler2D
{
    public class FloatEquals_Node : Math_Node
    {
        private PortWithField<float> portA;
        private PortWithField<float> portB;

        private Port<bool> outputPort;

        public FloatEquals_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.FloatEquals;
            SetTooltip("Returns true if float A is equal to float B (A == B).");
            searchMenuEntry = new string[] { "Math", "float" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<float>("A", PortDirection.Input, 0, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<float>("B", PortDirection.Input, 0, "B", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<bool>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return (bool)((float)portA.GetPortVariable() == (float)portB.GetPortVariable());
        }
    }
}
