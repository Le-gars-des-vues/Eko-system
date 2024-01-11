using UnityEngine;

namespace TerraTiler2D
{
    public class IntGreaterThan_Node : Math_Node
    {
        private PortWithField<int> portA;
        private PortWithField<int> portB;

        private Port<bool> outputPort;

        public IntGreaterThan_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IntGreaterThan;
            SetTooltip("Returns true if integer A is greater than integer B (A > B).");
            searchMenuEntry = new string[] { "Math", "int" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<int>("A", PortDirection.Input, 0, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<int>("B", PortDirection.Input, 0, "B", PortCapacity.Single, false);
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
            return (bool)((int)portA.GetPortVariable() > (int)portB.GetPortVariable());
        }
    }
}
