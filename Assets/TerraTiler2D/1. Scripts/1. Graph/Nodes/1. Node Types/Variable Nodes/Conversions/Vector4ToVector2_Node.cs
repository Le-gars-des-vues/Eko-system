using UnityEngine;

namespace TerraTiler2D
{
    public class Vector4ToVector2_Node : Node
    {
        private Port<Vector4> inputPort;
        private Port<Vector2> outputPort;

        public Vector4ToVector2_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector4ToVector2;
            SetTooltip("Converts a Vector4 to a Vector2.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<Vector4>("", PortDirection.Input, "Vector4", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            outputPort = GeneratePort<Vector2>("", PortDirection.Output, "Vector2", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Vector4 inputVec = (Vector4)inputPort.GetPortVariable();
            Vector2 outputVec = new Vector2(inputVec.x, inputVec.y);

            return outputVec;
        }
    }
}