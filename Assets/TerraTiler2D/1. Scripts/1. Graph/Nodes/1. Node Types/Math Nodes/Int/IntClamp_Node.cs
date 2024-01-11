using UnityEngine;

namespace TerraTiler2D
{
    public class IntClamp_Node : Math_Node
    {
        private PortWithField<int> portA;
        private PortWithField<int> portMin;
        private PortWithField<int> portMax;

        private Port<int> outputPort;

        public IntClamp_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IntClamp;
            SetTooltip("Clamps integer A between Min and Max. Returns integer A if it is within the Min and Max range.");
            searchMenuEntry = new string[] { "Math", "int" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<int>("A", PortDirection.Input, 0, "A", PortCapacity.Single, false);

            portMin = GeneratePortWithField<int>("Min", PortDirection.Input, 0, "Min", PortCapacity.Single, false);
            portMax = GeneratePortWithField<int>("Max", PortDirection.Input, 0, "Max", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<int>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            return (int)Mathf.Clamp((int)portA.GetPortVariable(), (int)portMin.GetPortVariable(), (int)portMax.GetPortVariable());
        }
    }
}
