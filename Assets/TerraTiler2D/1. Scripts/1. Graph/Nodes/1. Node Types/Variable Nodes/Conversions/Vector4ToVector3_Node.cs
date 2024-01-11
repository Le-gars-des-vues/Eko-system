using UnityEngine;

namespace TerraTiler2D
{
    public class Vector4ToVector3_Node : Node
    {
        private Port<Vector4> inputPort;
        private Port<Vector3> outputPort;

        public Vector4ToVector3_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector4ToVector3;
            SetTooltip("Converts a Vector4 to a Vector3.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<Vector4>("", PortDirection.Input, "Vector4", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            outputPort = GeneratePort<Vector3>("", PortDirection.Output, "Vector3", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Vector4 inputVec = (Vector4)inputPort.GetPortVariable();
            Vector3 outputVec = new Vector3(inputVec.x, inputVec.y, inputVec.z);

            return outputVec;
        }
    }
}
