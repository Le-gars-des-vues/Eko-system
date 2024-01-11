using UnityEngine;

namespace TerraTiler2D
{
    /// <summary>
    /// A node with a toggleable bool output port.  	
    /// </summary>
    public class Bool_Node : Node
    {
        private PortWithField<bool> boolPort;

        public Bool_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Bool;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            boolPort = GeneratePortWithField<bool>("", PortDirection.Output, true, "BoolOutput", PortCapacity.Multi, false);
        }

        protected override void InitializeAdditionalElements()
        {

        }
    }
}