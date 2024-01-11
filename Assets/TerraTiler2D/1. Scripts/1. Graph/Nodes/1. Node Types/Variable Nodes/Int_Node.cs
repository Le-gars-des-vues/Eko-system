using UnityEngine;

namespace TerraTiler2D
{
    public class Int_Node : Node
    {
        private PortWithField<int> intPort;

        public Int_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Int;
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {

        }

        protected override void InitializeOutputPorts()
        {
            intPort = GeneratePortWithField<int>("", PortDirection.Output, 0, "IntOutput", PortCapacity.Multi, false);
        }

        protected override void InitializeAdditionalElements()
        {

        }
    }
}
