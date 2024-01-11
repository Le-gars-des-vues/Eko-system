using UnityEngine;

namespace TerraTiler2D
{
    public class Vector2ToVector3_Node : Node
    {
        private Port<Vector2> inputPort;
        private Port<Vector3> outputPort;

        public Vector2ToVector3_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector2ToVector3;
            SetTooltip("Converts a Vector2 to a Vector3.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<Vector2>("", PortDirection.Input, "Vector2", PortCapacity.Single, true);
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
            Vector2 inputVec = (Vector2)inputPort.GetPortVariable();
            Vector3 outputVec = new Vector3(inputVec.x, inputVec.y, 0);

            return outputVec;
        }
    }
}