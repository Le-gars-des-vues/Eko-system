using UnityEngine;

namespace TerraTiler2D
{
    public class IntModulo_Node : Math_Node
    {
        private PortWithField<int> portA;
        private PortWithField<int> portB;

        private Port<int> outputPort;

        public IntModulo_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IntModulo;
            SetTooltip("Returns the remainder of A divided by B.");
            searchMenuEntry = new string[] { "Math", "int" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<int>("A", PortDirection.Input, 10, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<int>("B", PortDirection.Input, 3, "B", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<int>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return (int)portA.GetPortVariable() % (int)portB.GetPortVariable();
        }
    }
}
