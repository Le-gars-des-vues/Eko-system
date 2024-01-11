using UnityEngine;

namespace TerraTiler2D
{
    public class Rect_Node : Node
    {
        private Port<Rect> rectPort;

        private PortWithField<float> xPositionPort;
        private PortWithField<float> yPositionPort;

        private PortWithField<float> rectWidthPort;
        private PortWithField<float> rectHeightPort;

        public Rect_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Rect;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {
            xPositionPort = GeneratePortWithField<float>("X", PortDirection.Input, 0, "X", PortCapacity.Single, false);
            yPositionPort = GeneratePortWithField<float>("Y", PortDirection.Input, 0, "Y", PortCapacity.Single, false);
            rectWidthPort = GeneratePortWithField<float>("Width", PortDirection.Input, 10, "Width", PortCapacity.Single, false);
            rectHeightPort = GeneratePortWithField<float>("Height", PortDirection.Input, 10, "Height", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            rectPort = GeneratePort<Rect>("", PortDirection.Output, "Rect", PortCapacity.Multi, false);
            rectPort.SetOutputPortMethod(GetRectOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetRectOutput()
        {
            return new Rect((float)xPositionPort.GetPortVariable(), (float)yPositionPort.GetPortVariable(), (float)rectWidthPort.GetPortVariable(), (float)rectHeightPort.GetPortVariable());
        }

        public void SetValue(Rect newValue)
        {
            xPositionPort.SetValue(newValue.x);
            yPositionPort.SetValue(newValue.y);

            rectWidthPort.SetValue(newValue.width);
            rectHeightPort.SetValue(newValue.height);

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
