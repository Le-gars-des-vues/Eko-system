using UnityEngine;

namespace TerraTiler2D
{
    public class Vector4_Node : Node
    {
        private Port<Vector4> vector4Port;

        private PortWithField<float> xFloatPort;
        private PortWithField<float> yFloatPort;
        private PortWithField<float> zFloatPort;
        private PortWithField<float> wFloatPort;

        public Vector4_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector4;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {
            xFloatPort = GeneratePortWithField<float>("X", PortDirection.Input, 0, "X", PortCapacity.Single, false);
            yFloatPort = GeneratePortWithField<float>("Y", PortDirection.Input, 0, "Y", PortCapacity.Single, false);
            zFloatPort = GeneratePortWithField<float>("Z", PortDirection.Input, 0, "Z", PortCapacity.Single, false);
            wFloatPort = GeneratePortWithField<float>("W", PortDirection.Input, 0, "W", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            vector4Port = GeneratePort<Vector4>("", PortDirection.Output, "Vector4", PortCapacity.Multi, false);
            vector4Port.SetOutputPortMethod(GetVector4Output);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetVector4Output()
        {
            return new Vector4((float)xFloatPort.GetPortVariable(), (float)yFloatPort.GetPortVariable(), (float)zFloatPort.GetPortVariable(), (float)wFloatPort.GetPortVariable());
        }

        public void SetValue(Vector4 newValue)
        {
            xFloatPort.SetValue(newValue.x);
            yFloatPort.SetValue(newValue.y);
            zFloatPort.SetValue(newValue.z);
            wFloatPort.SetValue(newValue.w);

            ////If the output is connected to any other node, tell the connected nodes the value has changed.
            //var edges = (outputContainer.ElementAt(0) as Port).connections.GetEnumerator();
            //if (edges.MoveNext())
            //{
            //    if (edges.Current.input != null)
            //    {
            //        (edges.Current.input.node as TT_Node).SetPortVariable(edges.Current.input, newValue);
            //    }
            //}
        }
    }
}
