using UnityEngine;

namespace TerraTiler2D
{
    public class CeilToInt_Node : Node
    {
        private Port<float> inputPort;
        private Port<int> outputPort;

        public CeilToInt_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.CeilToInt;
            SetTooltip("Rounds a float up to the nearest integer.");
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
            return Mathf.CeilToInt((float)inputPort.GetPortVariable());
        }
    }
}
