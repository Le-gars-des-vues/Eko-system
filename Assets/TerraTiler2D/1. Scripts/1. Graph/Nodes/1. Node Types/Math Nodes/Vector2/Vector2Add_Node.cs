using UnityEngine;

namespace TerraTiler2D
{
    public class Vector2Add_Node : Math_Node
    {
        private PortWithField<Vector2> portA;
        private PortWithField<Vector2> portB;

        private Port<Vector2> outputPort;

        public Vector2Add_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector2Add;
            SetTooltip("Adds Vector2 A to Vector2 B.");
            searchMenuEntry = new string[] { "Math", "Vector2" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector2>("A", PortDirection.Input, Vector2.zero, "A", PortCapacity.Single, false);
            portB = GeneratePortWithField<Vector2>("B", PortDirection.Input, Vector2.zero, "B", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector2>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return (Vector2)portA.GetPortVariable() + (Vector2)portB.GetPortVariable();
        }
    }
}
