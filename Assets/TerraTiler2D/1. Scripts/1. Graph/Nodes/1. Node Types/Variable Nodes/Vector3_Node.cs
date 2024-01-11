using UnityEngine;

namespace TerraTiler2D
{
    public class Vector3_Node : Node
    {
        private Port<Vector3> vector3Port;

        private PortWithField<float> xFloatPort;
        private PortWithField<float> yFloatPort;
        private PortWithField<float> zFloatPort;

        public Vector3_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Vector3;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {
            xFloatPort = GeneratePortWithField<float>("X", PortDirection.Input, 0, "X", PortCapacity.Single, false);
            yFloatPort = GeneratePortWithField<float>("Y", PortDirection.Input, 0, "Y", PortCapacity.Single, false);
            zFloatPort = GeneratePortWithField<float>("Z", PortDirection.Input, 0, "Z", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            vector3Port = GeneratePort<Vector3>("", PortDirection.Output, "Vector3", PortCapacity.Multi, false);
            vector3Port.SetOutputPortMethod(GetVector3Output);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetVector3Output()
        {
            return new Vector3((float)xFloatPort.GetPortVariable(), (float)yFloatPort.GetPortVariable(), (float)zFloatPort.GetPortVariable());
        }

        public void SetValue(Vector3 newValue)
        {
            xFloatPort.SetValue(newValue.x);
            yFloatPort.SetValue(newValue.y);
            zFloatPort.SetValue(newValue.z);

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
