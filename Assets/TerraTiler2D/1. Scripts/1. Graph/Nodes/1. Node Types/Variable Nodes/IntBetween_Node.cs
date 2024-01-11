using UnityEngine;

namespace TerraTiler2D
{
    public class IntBetween_Node : Node
    {
        private PortWithField<int> minPort;
        private PortWithField<int> maxPort;

        private Port<int> outputPort;

        public IntBetween_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IntBetween;
            SetTooltip("Returns a random int within [Min...Max] (range is exclusive)");
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {
            minPort = GeneratePortWithField<int>("Min", PortDirection.Input, 0, "Min", PortCapacity.Single, false);
            maxPort = GeneratePortWithField<int>("Max", PortDirection.Input, 10, "Max", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<int>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            //Set the method for determining the value of this port.
            outputPort.SetOutputPortMethod(GetIntOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetIntOutput()
        {
            int min = (int)minPort.GetPortVariable();
            int max = (int)maxPort.GetPortVariable();

            return UnityEngine.Random.Range(min, max);
        }
    }
}



