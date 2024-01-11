using UnityEngine;

namespace TerraTiler2D
{
    public class Bool_PropertySetNode : PropertySetter_Node<bool>
    {
        public Bool_PropertySetNode(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.BoolPropertySet;
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
