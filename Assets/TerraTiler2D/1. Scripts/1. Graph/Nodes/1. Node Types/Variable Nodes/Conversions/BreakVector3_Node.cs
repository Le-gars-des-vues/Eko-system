using UnityEngine;

namespace TerraTiler2D
{
    public class BreakVector3_Node : Node
    {
        private Port<Vector3> inputPort;

        private Port<float> outputPortX;
        private Port<float> outputPortY;
        private Port<float> outputPortZ;

        public BreakVector3_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.BreakVector3;
            SetTooltip("Breaks a Vector3 up into components.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<Vector3>("", PortDirection.Input, "Vector3", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            outputPortX = GeneratePort<float>("X", PortDirection.Output, "X", PortCapacity.Multi, false);
            outputPortX.SetOutputPortMethod(GetOutputX);

            outputPortY = GeneratePort<float>("Y", PortDirection.Output, "Y", PortCapacity.Multi, false);
            outputPortY.SetOutputPortMethod(GetOutputY);

            outputPortZ = GeneratePort<float>("Z", PortDirection.Output, "Z", PortCapacity.Multi, false);
            outputPortZ.SetOutputPortMethod(GetOutputZ);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutputX()
        {
            return ((Vector3)inputPort.GetPortVariable()).x;
        }
        public object GetOutputY()
        {
            return ((Vector3)inputPort.GetPortVariable()).y;
        }
        public object GetOutputZ()
        {
            return ((Vector3)inputPort.GetPortVariable()).z;
        }
    }
}
