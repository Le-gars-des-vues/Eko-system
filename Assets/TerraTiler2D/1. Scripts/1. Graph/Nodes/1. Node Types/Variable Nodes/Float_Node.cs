using UnityEngine;

namespace TerraTiler2D
{
    /// <summary>
    /// A node with a float field.
    /// </summary>
    public class Float_Node : Node
    {
        private PortWithField<float> floatPort;

        //========== Initialization ==========

        public Float_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Float;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {

        }
        protected override void InitializeOutputPorts()
        {
            floatPort = GeneratePortWithField<float>("", PortDirection.Output, 0, "FloatOutput", PortCapacity.Multi, false);
        }
        protected override void InitializeAdditionalElements()
        {

        }

        //========== Port data passing ==========


        //========== NodeData saving and loading ==========

    }
}
