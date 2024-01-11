using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class ExtractTexture2D_Node : Node
    {
        protected Port<Texture2D> textureInputPort;
        protected PortWithField<float> minPort;
        protected PortWithField<float> maxPort;

        protected Port<Texture2D> textureOutputPort;

        private NodePreview nodePreview;

        public ExtractTexture2D_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.ExtractTexture2D;
            SetTooltip("Extracts pixels from a Texture2D. Fetches all the pixels within Min and Max brightness, and makes all other pixels black.");
            searchMenuEntry = new string[] { "Texture2D" };
            nodePreview = new NodePreview(this, ExtractTexture);
        }

        protected override void InitializeInputPorts()
        {
            textureInputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Input, "Texture", PortCapacity.Single, true, "Texture2D to extract pixels from.");
            minPort = GeneratePortWithField<float>("Min brightness", PortDirection.Input, 0, "Min", PortCapacity.Single, false, "Minimum brightness of pixels to extract.");
            maxPort = GeneratePortWithField<float>("Max brightness", PortDirection.Input, 1, "Max", PortCapacity.Single, false, "Maximum brightness of pixels to extract.");
        }

        protected override void InitializeOutputPorts()
        {
            textureOutputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Output, "TextureOutput", PortCapacity.Multi, false, "Texture2D with extracted pixels.");
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

            return ExtractTexture();
        }

        private Texture2D ExtractTexture()
        {
            Texture2D inputTexture = (Texture2D)textureInputPort.GetPortVariable();
            float min = (float)minPort.GetPortVariable();
            float max = (float)maxPort.GetPortVariable();

            Texture2D extractedTexture = new Texture2D(inputTexture.width, inputTexture.height);
            Color[] extractedPixels = inputTexture.GetPixels();

            for (int i = 0; i < extractedPixels.Length; i++)
            {
                if (GetPixelBrightness(extractedPixels[i]) < min || GetPixelBrightness(extractedPixels[i]) > max)
                {
                    extractedPixels[i] = Color.black;
                }
            }

            extractedTexture.SetPixels(extractedPixels);
            extractedTexture.Apply();

            return extractedTexture;
        }

        private float GetPixelBrightness(Color pixel)
        {
            return ((pixel.r + pixel.g + pixel.b) / 3);
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
