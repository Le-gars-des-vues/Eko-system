using UnityEngine;

namespace TerraTiler2D
{
    public class Vector4Clamp_Node : Math_Node
    {
        private PortWithField<Vector4> portA;
        private PortWithField<float> portMin;
        private PortWithField<float> portMax;

        private Port<Vector4> outputPort;

        public Vector4Clamp_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector4Clamp;
            SetTooltip("Clamps the length of Vector4 A between Min and Max. Returns Vector4 A if its length is within the Min and Max range.");
            searchMenuEntry = new string[] { "Math", "Vector4" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector4>("A", PortDirection.Input, Vector4.zero, "A", PortCapacity.Single, false);

            portMin = GeneratePortWithField<float>("Min", PortDirection.Input, 0, "Min", PortCapacity.Single, false);
            portMax = GeneratePortWithField<float>("Max", PortDirection.Input, 0, "Max", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector4>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Vector4 normalizedVec = ((Vector4)portA.GetPortVariable()).normalized;

            return normalizedVec * Mathf.Clamp(((Vector4)portA.GetPortVariable()).magnitude, (float)portMin.GetPortVariable(), (float)portMax.GetPortVariable());
        }
    }
}
