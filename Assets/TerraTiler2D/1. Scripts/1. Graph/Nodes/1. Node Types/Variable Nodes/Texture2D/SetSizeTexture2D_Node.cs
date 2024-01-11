using UnityEngine;

namespace TerraTiler2D
{
    public class SetSizeTexture2D_Node : Node
    {
        protected Port<Texture2D> textureInputPort;
        protected Port<Texture2D> textureOutputPort;

        protected PortWithField<Vector2> sizePort;

        public SetSizeTexture2D_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.SetSizeTexture2D;
            SetTooltip("Resizes a Texture2D.");
            searchMenuEntry = new string[] { "Texture2D" };
        }

        protected override void InitializeInputPorts()
        {
            textureInputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Input, "Texture", PortCapacity.Single, true, "The texture to resize.");
            sizePort = GeneratePortWithField<Vector2>("Vector2", PortDirection.Input, new Vector2(100, 100), "Size", PortCapacity.Single, false, "The size to resize the texture to.");
        }

        protected override void InitializeOutputPorts()
        {
            textureOutputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Output, "TextureOutput", PortCapacity.Multi, false, "The resized texture.");
            textureOutputPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            Texture2D texture = (Texture2D)textureInputPort.GetPortVariable();
            Vector2 size = (Vector2)sizePort.GetPortVariable();
            return Glob.GetInstance().ResizeTexture2D(texture, (int)size.x, (int)size.y);
        }
    }
}
