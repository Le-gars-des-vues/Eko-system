using UnityEngine;

namespace TerraTiler2D
{
    public class Float_PropertySetNode : PropertySetter_Node<float>
    {
        public Float_PropertySetNode(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.FloatPropertySet;
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
