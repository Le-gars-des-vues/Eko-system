using UnityEngine;

namespace TerraTiler2D
{
    /// <summary>
    /// A node that outputs a random float between input A and B.  	
    /// </summary>
    public class FloatBetween_Node : Node
    {
        private PortWithField<float> minPort;
        private PortWithField<float> maxPort;

        private Port<float> outputPort;

        public FloatBetween_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.FloatBetween;
            SetTooltip("Returns a random float within [Min...Max] (range is inclusive)");
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {
            minPort = GeneratePortWithField<float>("Min", PortDirection.Input, 0.0f, "Min", PortCapacity.Single, false);
            maxPort = GeneratePortWithField<float>("Max", PortDirection.Input, 1.0f, "Max", PortCapacity.Single, false);
        }

        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            outputPort = GeneratePort<float>("", PortDirection.Output, "Output", PortCapacity.Multi, false);
            outputPort.SetOutputPortMethod(GetFloatOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetFloatOutput()
        {
            return UnityEngine.Random.Range((float)minPort.GetPortVariable(), (float)maxPort.GetPortVariable());
        }

        public void SetValue(Vector2 newValue)
        {
            minPort.SetValue(newValue.x);
            maxPort.SetValue(newValue.y);

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
