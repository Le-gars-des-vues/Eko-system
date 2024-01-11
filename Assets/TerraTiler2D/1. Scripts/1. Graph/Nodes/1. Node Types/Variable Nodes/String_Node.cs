using UnityEngine;

namespace TerraTiler2D
{
    public class String_Node : Node
    {
        private PortWithField<string> stringPort;

        public String_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.String;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            stringPort = GeneratePortWithField<string>("", PortDirection.Output, "", "StringOutput", PortCapacity.Multi, false);
        }

        protected override void InitializeAdditionalElements()
        {

        }
    }
}
