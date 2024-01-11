using UnityEngine;

namespace TerraTiler2D
{
    public class Vector3Clamp_Node : Math_Node
    {
        private PortWithField<Vector3> portA;
        private PortWithField<float> portMin;
        private PortWithField<float> portMax;

        private Port<Vector3> outputPort;

        public Vector3Clamp_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector3Clamp;
            SetTooltip("Clamps the length of Vector3 A between Min and Max. Returns Vector3 A if its length is within the Min and Max range.");
            searchMenuEntry = new string[] { "Math", "Vector3" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector3>("A", PortDirection.Input, Vector3.zero, "A", PortCapacity.Single, false);

            portMin = GeneratePortWithField<float>("Min", PortDirection.Input, 0, "Min", PortCapacity.Single, false);
            portMax = GeneratePortWithField<float>("Max", PortDirection.Input, 0, "Max", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector3>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Vector3 normalizedVec = ((Vector3)portA.GetPortVariable()).normalized;

            return normalizedVec * Mathf.Clamp(((Vector3)portA.GetPortVariable()).magnitude, (float)portMin.GetPortVariable(), (float)portMax.GetPortVariable());
        }
    }
}
