using UnityEngine;

namespace TerraTiler2D
{
    public class GetSizeTexture2D_Node : Node
    {
        protected Port<Texture2D> textureInputPort;

        protected Port<Vector2> sizePort;

        public GetSizeTexture2D_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.GetSizeTexture2D;
            SetTooltip("Gets the size of a Texture2D in pixels.");
            searchMenuEntry = new string[] { "Texture2D" };
        }

        protected override void InitializeInputPorts()
        {
            textureInputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Input, "Texture", PortCapacity.Single, true, "Texture2D to get the size in pixels from.");
        }

        protected override void InitializeOutputPorts()
        {
            sizePort = GeneratePort<Vector2>("Vector2", PortDirection.Output, "Size", PortCapacity.Multi, false, "Vector2 size of the Texture2D in pixels.");
            sizePort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Texture2D texture = (Texture2D)textureInputPort.GetPortVariable();
            return new Vector2(texture.width, texture.height);
        }
    }
}
