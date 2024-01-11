using UnityEngine;

namespace TerraTiler2D
{
    public class ToString_Node : Node
    {
        private Port<object> inputPort;
        private Port<string> stringPort;

        public ToString_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.ToString;
            SetTooltip("Converts a variable to a string.");
            searchMenuEntry = new string[] { "Variables", "Conversions" };
        }

        protected override void InitializeInputPorts()
        {
            inputPort = GeneratePort<object>("", PortDirection.Input, "Object", PortCapacity.Single, true, "The variable to convert to a string.");
        }

        protected override void InitializeOutputPorts()
        {
            stringPort = GeneratePort<string>("", PortDirection.Output, "String", PortCapacity.Multi, false);
            stringPort.SetOutputPortMethod(GetStringOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public string GetStringOutput()
        {
            object variable = inputPort.GetPortVariable();

            if (variable != null)
            {
                return inputPort.GetPortVariable().ToString();
            }
            else
            {
                return "NULL";
            }
        }
    }
}
