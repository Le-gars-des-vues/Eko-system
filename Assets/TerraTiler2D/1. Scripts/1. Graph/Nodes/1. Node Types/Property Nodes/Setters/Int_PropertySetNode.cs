using UnityEngine;

namespace TerraTiler2D
{
    public class Int_PropertySetNode : PropertySetter_Node<int>
    {
        public Int_PropertySetNode(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.IntPropertySet;
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();
        }

        protected override void InitializeAdditionalElements()
        {

        }
    }
}
