using UnityEngine;

namespace TerraTiler2D
{
    public class FloatClamp_Node : Math_Node
    {
        private PortWithField<float> portA;
        private PortWithField<float> portMin;
        private PortWithField<float> portMax;

        private Port<float> outputPort;

        public FloatClamp_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.FloatClamp;
            SetTooltip("Clamps float A between Min and Max. Returns float A if it is within the Min and Max range.");
            searchMenuEntry = new string[] { "Math", "float" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<float>("A", PortDirection.Input, 0, "A", PortCapacity.Single, false);

            portMin = GeneratePortWithField<float>("Min", PortDirection.Input, 0, "Min", PortCapacity.Single, false);
            portMax = GeneratePortWithField<float>("Max", PortDirection.Input, 0, "Max", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<float>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return (float)Mathf.Clamp((float)portA.GetPortVariable(), (float)portMin.GetPortVariable(), (float)portMax.GetPortVariable());
        }
    }
}
