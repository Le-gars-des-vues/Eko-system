using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public abstract class Noisemap_Node : Node
    {
        protected PortWithField<bool> constantPort;
        protected PortWithField<Vector2> noisemapSizePort;
        protected Port<Gradient> gradientPort;

        protected Port<Texture2D> noisemapPort;

        protected Texture2D generatedNoisemap;

        private NodePreview nodePreview;

        //========== Initialization ==========

        public Noisemap_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            searchMenuEntry = new string[] { "Texture2D", "Noise" };

            nodePreview = new NodePreview(this, GetNoisemap);
        }

        protected override void InitializeInputPorts()
        {
            constantPort = GeneratePortWithField<bool>("Constant", PortDirection.Input, false, "Constant", PortCapacity.Single, false, "True: Pass the same texture to all output connections. False: Generate a new noisemap for each output connection.");

            noisemapSizePort = GeneratePortWithField<Vector2>("Size", PortDirection.Input, new Vector2(100, 100), "Size", PortCapacity.Single, false, "The size of the texture in pixels.");

            gradientPort = GeneratePort<Gradient>("Gradient", PortDirection.Input, "Gradient", PortCapacity.Single, false, "The colors the noisemap consists of.");
        }
        protected override void InitializeOutputPorts()
        {
            //Create and add an output port to the node
            noisemapPort = GeneratePort<Texture2D>("Texture2D", PortDirection.Output, "OutputTexture", PortCapacity.Multi, false);
            noisemapPort.SetOutputPortMethod(GetNoisemapOutput);
        }
        protected override void InitializeAdditionalElements()
        {

        }

        private object GetNoisemapOutput()
        {
            object constantVariable = constantPort.GetPortVariable();
            if (constantVariable == null)
            {
                constantVariable = true;
            }

            if ((bool)constantVariable && generatedNoisemap != null)
            {
                return generatedNoisemap;
            }

            return GetNoisemap();
        }

        //========== Node methods ==========

        protected abstract Texture2D GetNoisemap();
        protected Texture2D GetNoisemap(Noisemap noisemap)
        {
            if (noisemap == null)
            {
                Glob.GetInstance().DebugString("No instance of Noisemap was passed into the GetNoisemap function of base class Noisemap_Node. This function should not be called directly from the base class, only from a class that inherits from Noisemap_Node.", Glob.DebugCategories.Error, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                return null;
            }

            object generatePerConnectionVariable = constantPort.GetPortVariable();
            if (generatePerConnectionVariable != null)
            {
                noisemap.SetGeneratePerConnection((bool)generatePerConnectionVariable);
            }
            object noisemapSizeVariable = noisemapSizePort.GetPortVariable();
            if (noisemapSizeVariable != null)
            {
                noisemap.SetNoisemapSize((Vector2)noisemapSizeVariable);
            }
            object colorVariable = gradientPort.GetPortVariable();
            if (colorVariable != null)
            {
                noisemap.SetColoring((Gradient)colorVariable);
            }

            generatedNoisemap = noisemap.CreateNoisemap();

            if (nodePreview.ShouldPreview())
            {
                nodePreview.ShowTexture(generatedNoisemap);
            }

            return generatedNoisemap;
        }

        //========== Port data passing ==========

        public override void ResetNodeVariables()
        {
            generatedNoisemap = null;

            base.ResetNodeVariables();
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
