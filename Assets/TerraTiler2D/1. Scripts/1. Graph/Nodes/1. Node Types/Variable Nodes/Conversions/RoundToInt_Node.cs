using UnityEngine;

namespace TerraTiler2D
{
    public class RoundToInt_Node : Node
    {
        private Port<float> inputPort;
        private Port<int> outputPort;

        public RoundToInt_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.RoundToInt;
            SetTooltip("Rounds a float to the nearest integer.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<float>("", PortDirection.Input, "Input", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            outputPort = GeneratePort<int>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return Mathf.RoundToInt((float)inputPort.GetPortVariable());
        }
    }
}
