using UnityEngine;

namespace TerraTiler2D
{
    /// <summary>
    /// A node with a color field.
    /// </summary>
    public class Color_Node : Node
    {
        private PortWithField<Color> colorPort;

        public Color_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Color;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            //Create an input port.
            colorPort = GeneratePortWithField<Color>("Color", PortDirection.Output, default(Color), "ColorOutput", PortCapacity.Multi, false);
        }

        protected override void InitializeAdditionalElements()
        {

        }
    }
}
