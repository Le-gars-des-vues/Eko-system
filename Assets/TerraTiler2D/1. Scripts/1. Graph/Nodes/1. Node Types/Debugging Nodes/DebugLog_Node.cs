using UnityEngine;

namespace TerraTiler2D
{
    public class DebugLog_Node : Flow_Node
    {
        private PortWithField<string> debugPort;

        public DebugLog_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.DebugLog;
            SetTooltip("Debugs a string in the console window.");
            searchMenuEntry = new string[] { "Debugging" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            debugPort = GeneratePortWithField<string>("Text", PortDirection.Input, "Debug", "Debug", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            Debug.Log(debugPort.GetPortVariable());

            base.ApplyBehaviour(flow, trickleDown);
        }

        public void SetValue(string newValue)
        {
            debugPort.SetValue(newValue);
        }
    }
}
