using UnityEngine;

namespace TerraTiler2D
{
    public class Vector2Clamp_Node : Math_Node
    {
        private PortWithField<Vector2> portA;
        private PortWithField<float> portMin;
        private PortWithField<float> portMax;

        private Port<Vector2> outputPort;

        public Vector2Clamp_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector2Clamp;
            SetTooltip("Clamps the length of Vector2 A between Min and Max. Returns Vector2 A if its length is within the Min and Max range.");
            searchMenuEntry = new string[] { "Math", "Vector2" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<Vector2>("A", PortDirection.Input, Vector2.zero, "A", PortCapacity.Single, false);

            portMin = GeneratePortWithField<float>("Min", PortDirection.Input, 0, "Min", PortCapacity.Single, false);
            portMax = GeneratePortWithField<float>("Max", PortDirection.Input, 0, "Max", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<Vector2>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Vector2 normalizedVec = ((Vector2)portA.GetPortVariable()).normalized;

            return normalizedVec * Mathf.Clamp(((Vector2)portA.GetPortVariable()).magnitude, (float)portMin.GetPortVariable(), (float)portMax.GetPortVariable());
        }
    }
}
