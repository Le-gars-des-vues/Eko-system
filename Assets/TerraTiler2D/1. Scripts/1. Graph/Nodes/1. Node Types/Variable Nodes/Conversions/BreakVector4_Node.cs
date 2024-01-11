using UnityEngine;

namespace TerraTiler2D
{
    public class BreakVector4_Node : Node
    {
        private Port<Vector4> inputPort;

        private Port<float> outputPortX;
        private Port<float> outputPortY;
        private Port<float> outputPortZ;
        private Port<float> outputPortW;

        public BreakVector4_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.BreakVector4;
            SetTooltip("Breaks a Vector4 up into components.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<Vector4>("", PortDirection.Input, "Vector4", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            outputPortX = GeneratePort<float>("X", PortDirection.Output, "X", PortCapacity.Multi, false);
            outputPortX.SetOutputPortMethod(GetOutputX);

            outputPortY = GeneratePort<float>("Y", PortDirection.Output, "Y", PortCapacity.Multi, false);
            outputPortY.SetOutputPortMethod(GetOutputY);

            outputPortZ = GeneratePort<float>("Z", PortDirection.Output, "Z", PortCapacity.Multi, false);
            outputPortZ.SetOutputPortMethod(GetOutputZ);

            outputPortW = GeneratePort<float>("W", PortDirection.Output, "W", PortCapacity.Multi, false);
            outputPortW.SetOutputPortMethod(GetOutputW);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutputX()
        {
            return ((Vector4)inputPort.GetPortVariable()).x;
        }
        public object GetOutputY()
        {
            return ((Vector4)inputPort.GetPortVariable()).y;
        }
        public object GetOutputZ()
        {
            return ((Vector4)inputPort.GetPortVariable()).z;
        }
        public object GetOutputW()
        {
            return ((Vector4)inputPort.GetPortVariable()).w;
        }
    }
}
