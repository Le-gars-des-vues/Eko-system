using UnityEngine;

namespace TerraTiler2D
{
    public class IfBranch_Node : Flow_Node
    {
        private PortWithField<bool> togglePort;

        private Port<Flow> truePort;
        private Port<Flow> falsePort;

        public IfBranch_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IfBranch;
            SetTooltip("Continues graph execution along one of two paths, based on the input boolean.");
            searchMenuEntry = new string[] { "Control" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            togglePort = GeneratePortWithField<bool>("", PortDirection.Input, true, "Toggle", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();

#if (UNITY_EDITOR)
            outputPort.port.portName = "True";
            outputPort.tooltip = "";
#endif

            truePort = outputPort;

            falsePort = GeneratePort<Flow>("False", PortDirection.Output, "False", PortCapacity.Single, false);
            falsePort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            bool toggle = (bool)togglePort.GetPortVariable();

            if (toggle)
            {
                outputPort = truePort;
            }
            else
            {
                outputPort = falsePort;
            }

            base.ApplyBehaviour(flow, trickleDown);
        }
    }
}
