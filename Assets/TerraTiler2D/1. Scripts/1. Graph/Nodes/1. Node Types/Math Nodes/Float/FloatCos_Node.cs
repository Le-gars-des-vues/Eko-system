using UnityEngine;

namespace TerraTiler2D
{
    public class FloatCos_Node : Math_Node
    {
        private PortWithField<float> portA;

        private Port<float> outputPort;

        public FloatCos_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.FloatCos;
            SetTooltip("Returns the cosine of angle A.");
            searchMenuEntry = new string[] { "Math", "float" };
        }

        protected override void InitializeInputPorts()
        {
            portA = GeneratePortWithField<float>("A", PortDirection.Input, 0, "A", PortCapacity.Single, false);
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
            return (float)Mathf.Cos((float)portA.GetPortVariable());
        }
    }
}
