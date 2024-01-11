using UnityEngine;

namespace TerraTiler2D
{
    public class Vector2_Node : Node
    {
        private Port<Vector2> vector2Port;

        private PortWithField<float> xFloatPort;
        private PortWithField<float> yFloatPort;

        public Vector2_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector2;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {
            xFloatPort = GeneratePortWithField<float>("X", PortDirection.Input, 0, "X", PortCapacity.Single, false);
            yFloatPort = GeneratePortWithField<float>("Y", PortDirection.Input, 0, "Y", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            vector2Port = GeneratePort<Vector2>("", PortDirection.Output, "Vector2", PortCapacity.Multi, false);
            vector2Port.SetOutputPortMethod(GetVector2Output);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetVector2Output()
        {
            return new Vector2((float)xFloatPort.GetPortVariable(), (float)yFloatPort.GetPortVariable());
        }

        public void SetValue(Vector2 newValue)
        {
            xFloatPort.SetValue(newValue.x);
            yFloatPort.SetValue(newValue.y);
        }
    }
}
