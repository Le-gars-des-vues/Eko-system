using UnityEngine;

namespace TerraTiler2D
{
    public class Vector3ToVector4_Node : Node
    {
        private Port<Vector3> inputPort;
        private Port<Vector4> outputPort;

        public Vector3ToVector4_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector3ToVector4;
            SetTooltip("Converts a Vector3 to a Vector4.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<Vector3>("", PortDirection.Input, "Vector3", PortCapacity.Single, true);
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
            Vector3 inputVec = (Vector3)inputPort.GetPortVariable();
            Vector4 outputVec = new Vector4(inputVec.x, inputVec.y, inputVec.z, 0);

            return outputVec;
        }
    }
}