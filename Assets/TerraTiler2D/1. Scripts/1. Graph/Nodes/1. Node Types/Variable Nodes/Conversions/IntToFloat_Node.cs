using UnityEngine;

namespace TerraTiler2D
{
    public class IntToFloat_Node : Node
    {
        private Port<int> inputPort;
        private Port<float> outputPort;

        public IntToFloat_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IntToFloat;
            SetTooltip("Turns an integer into a float.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<int>("", PortDirection.Input, "Input", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            outputPort = GeneratePort<float>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return (float)(int)inputPort.GetPortVariable();
        }
    }
}