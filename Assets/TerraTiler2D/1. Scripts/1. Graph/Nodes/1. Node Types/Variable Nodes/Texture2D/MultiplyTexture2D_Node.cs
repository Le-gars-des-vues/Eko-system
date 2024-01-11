using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class MultiplyTexture2D_Node : Node
    {
        protected Port<Texture2D> texturePortA;
        protected Port<Texture2D> texturePortB;

        protected Port<Texture2D> textureOutputPort;

        private NodePreview nodePreview;

        public MultiplyTexture2D_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.MultiplyTexture2D;
            SetTooltip("Multiplies Texture A by Texture B.");
            searchMenuEntry = new string[] { "Texture2D" };
            nodePreview = new NodePreview(this, MultiplyTextures);
        }

        protected override void InitializeInputPorts()
        {
            texturePortA = GeneratePort<Texture2D>("A", PortDirection.Input, "A", PortCapacity.Single, true, "Texture A of the A*B texture multiplication.");
            texturePortB = GeneratePort<Texture2D>("B", PortDirection.Input, "B", PortCapacity.Single, true, "Texture B of the A*B texture multiplication.");
        }

        protected override void InitializeOutputPorts()
        {
            textureOutputPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Output, "Output", PortCapacity.Multi, false, "Texture A multiplied by Texture B.");
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

            return MultiplyTextures();
        }

        private Texture2D MultiplyTextures()
        {
            Texture2D textureA = (Texture2D)texturePortA.GetPortVariable();
            Texture2D textureB = (Texture2D)texturePortB.GetPortVariable();

            if (textureA.width != textureB.width ||
                textureA.height != textureB.height)
            {
                Glob.GetInstance().DebugString("The textures passed into node '" + GetTitle() + "' are not the same size. Resizing input texture B to the same size as input texture A.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Default);
                textureB = Glob.GetInstance().ResizeTexture2D(textureB, textureA.width, textureA.height);
            }

            Texture2D multipliedTexture = new Texture2D(textureA.width, textureA.height);
            Color[] multipliedPixels = new Color[multipliedTexture.width * multipliedTexture.height];

            for (int i = 0; i < multipliedTexture.height; i++)
            {
                for (int j = 0; j < multipliedTexture.width; j++)
                {
                    Color multipliedPixel = MultiplyPixels(textureA.GetPixel(j, i), textureB.GetPixel(j, i));
                    //Debug.Log("i: " + i + ", j: " + j + ", index: " + ((i * multipliedTexture.height) + j));
                    multipliedPixels[(i * multipliedTexture.width) + j] = multipliedPixel;
                }
            }
            multipliedTexture.SetPixels(multipliedPixels);
            multipliedTexture.Apply();

            return multipliedTexture;
        }
        private Color MultiplyPixels(Color A, Color B)
        {
            return new Color(A.r * B.r, A.g * B.g, A.b * B.b, A.a * B.a);
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
