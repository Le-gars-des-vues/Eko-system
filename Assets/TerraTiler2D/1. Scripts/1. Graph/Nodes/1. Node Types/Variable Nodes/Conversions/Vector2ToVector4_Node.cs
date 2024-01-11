using UnityEngine;

namespace TerraTiler2D
{
    public class Vector2ToVector4_Node : Node
    {
        private Port<Vector2> inputPort;
        private Port<Vector4> outputPort;

        public Vector2ToVector4_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector2ToVector4;
            SetTooltip("Converts a Vector2 to a Vector4.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<Vector2>("", PortDirection.Input, "Vector2", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            outputPort = GeneratePort<Vector4>("", PortDirection.Output, "Vector4", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Vector2 inputVec = (Vector2)inputPort.GetPortVariable();
            Vector4 outputVec = new Vector4(inputVec.x, inputVec.y, 0, 0);

            return outputVec;
        }
    }
}