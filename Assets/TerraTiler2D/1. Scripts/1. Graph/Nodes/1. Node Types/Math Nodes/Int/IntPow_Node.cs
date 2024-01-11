using UnityEngine;

namespace TerraTiler2D
{
    public class IntPow_Node : Math_Node
    {
        private PortWithField<int> portA;
        private PortWithField<int> portB;

        private Port<float> outputPort;

        public IntPow_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IntPow;
            SetTooltip("Returns A raised to the power of B.");
            searchMenuEntry = new string[] { "Math", "int" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<int>("A", PortDirection.Input, 1, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<int>("B", PortDirection.Input, 5, "B", PortCapacity.Single, false);
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
            return (float)Mathf.Pow((int)portA.GetPortVariable(), (int)portB.GetPortVariable());
        }
    }
}
