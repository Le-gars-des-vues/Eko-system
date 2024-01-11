using UnityEngine;

namespace TerraTiler2D
{
    public class BreakVector2_Node : Node
    {
        private Port<Vector2> inputPort;

        private Port<float> outputPortX;
        private Port<float> outputPortY;

        public BreakVector2_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.BreakVector2;
            SetTooltip("Breaks a Vector2 up into components.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<Vector2>("", PortDirection.Input, "Vector2", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            outputPortX = GeneratePort<float>("X", PortDirection.Output, "X", PortCapacity.Multi, false);
            outputPortX.SetOutputPortMethod(GetOutputX);

            outputPortY = GeneratePort<float>("Y", PortDirection.Output, "Y", PortCapacity.Multi, false);
            outputPortY.SetOutputPortMethod(GetOutputY);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutputX()
        {
            return ((Vector2)inputPort.GetPortVariable()).x;
        }
        public object GetOutputY()
        {
            return ((Vector2)inputPort.GetPortVariable()).y;
        }
    }
}
