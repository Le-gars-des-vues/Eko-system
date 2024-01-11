using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class InverseTexture2D_Node : Node
    {
        protected Port<Texture2D> textureInputPort;

        protected Port<Texture2D> textureOutputPort;

        private NodePreview nodePreview;

        public InverseTexture2D_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.InverseTexture2D;
            SetTooltip("Inverts the colors of a Texture2D.");
            searchMenuEntry = new string[] { "Texture2D" };
            nodePreview = new NodePreview(this, InverseTexture);
        }

        protected override void InitializeInputPorts()
        {
            textureInputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Input, "Texture", PortCapacity.Single, true, "Texture2D to invert the colors of.");
        }

        protected override void InitializeOutputPorts()
        {
            textureOutputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Output, "TextureOutput", PortCapacity.Multi, false, "Texture2D with inverted colors.");
            textureOutputPort.SetOutputPortMethod(GetTextureOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetTextureOutput()
        {
            if (nodePreview.ShouldPreview())
            {
                nodePreview.ShowTexture();
            }

            return InverseTexture();
        }

        private Texture2D InverseTexture()
        {
            Texture2D inputTexture = (Texture2D)textureInputPort.GetPortVariable();

            Texture2D inversedTexture = new Texture2D(inputTexture.width, inputTexture.height);
            Color[] inversedPixels = inputTexture.GetPixels();

            for (int i = 0; i < inversedPixels.Length; i++)
            {
                inversedPixels[i] = new Color(1.0f - inversedPixels[i].r, 1.0f - inversedPixels[i].g, 1.0f - inversedPixels[i].b);
            }

            inversedTexture.SetPixels(inversedPixels);
            inversedTexture.Apply();

            return inversedTexture;
        }

        //This node has a NodePreview, so we need to save a Preview_NodeData instead of the standard NodeData
        public override NodeData GetNodeData(NodeData nodeData = null)
        {
            return base.GetNodeData(new Preview_NodeData()
            {
                PreviewToggle = nodePreview.ShouldPreview()
            });
        }
        //This node has a NodePreview, so we need to load a Preview_NodeData instead of the standard NodeData, and apply the extra data to the NodePreview
        public override void LoadNodeData(NodeData data)
        {
            Preview_NodeData nodeData = data as Preview_NodeData;

            if (nodeData != null && nodePreview != null)
            {
                nodePreview.SetShouldPreview(nodeData.PreviewToggle);
            }
        }
    }
}
