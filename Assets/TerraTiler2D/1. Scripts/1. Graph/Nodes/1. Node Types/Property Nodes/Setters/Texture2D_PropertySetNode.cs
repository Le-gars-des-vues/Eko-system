using UnityEngine;

namespace TerraTiler2D
{
    public class Texture2D_PropertySetNode : PropertySetter_Node<Texture2D>
    {
        public Texture2D_PropertySetNode(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.Texture2DPropertySet;
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
