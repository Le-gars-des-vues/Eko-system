using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    /// <summary>
    /// Base class for nodes that apply behaviour on a TileLayer.  	
    /// </summary>
    public abstract class FlowTileLayer_Node : Flow_Node
    {
        private Port<TileLayer> tileLayerPort;
        private Port<TileLayer> tileLayerOutputPort;

        private Port<TileLayerMask> maskableTileLayerOutputPort;

        private TileLayer outputTileLayer;
        private TileLayerMask maskableOutputTileLayer;

        private NodePreview nodePreview;

        //========== Initialization ==========

        public FlowTileLayer_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodePreview = new NodePreview(this, GetPreviewTexture);
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            tileLayerPort = GeneratePort<TileLayer>("Tile layer", PortDirection.Input, "TileLayer", PortCapacity.Single, true);
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();

            tileLayerOutputPort = GeneratePort<TileLayer>("Tile layer", PortDirection.Output, "TileLayerOutput", PortCapacity.Multi, false, "Returns the TileLayer object with all changes applied to it, including any changes done further down the flow. The value of this port will change after this node is finished applying its functionality.");
            tileLayerOutputPort.SetOutputPortMethod(GetTileLayerOutput);

            maskableTileLayerOutputPort = GeneratePort<TileLayerMask>("Layer mask", PortDirection.Output, "TileLayerMaskOutput", PortCapacity.Multi, false, "Returns the TileLayer object with only the changes applied to it by this node and previous nodes. The value of this port will remain the same after this node is finished applying its functionality.");
            maskableTileLayerOutputPort.SetOutputPortMethod(GetMaskableTileLayerOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        //========== Node methods ==========

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            outputTileLayer = null; //Set the outputTileLayer to null, so that this node can be called again in the future. This is neccesary for loops.
            outputTileLayer = GetTileLayerOutput();

            if (nodePreview.ShouldPreview())
            {
                //Preview the results
                nodePreview.ShowTexture(outputTileLayer.GetTileLayerPreviewTexture());
            }

            //Pass flow on to the next node
            base.ApplyBehaviour(flow, trickleDown);
        }

        //Apply changes to the input TileLayer object, and return the results
        public abstract TileLayer ApplyBehaviourOnTileLayer(TileLayer tileLayer);

        //========== Port data passing ==========

        public TileLayer GetTileLayerOutput()
        {
            //If an output has already been generated
            if (outputTileLayer != null)
            {
                //Return the saved results
                return outputTileLayer;
            }

            //Get the output TileLayer of the previous node
            TileLayer inputTileLayer = (TileLayer)tileLayerPort.GetPortVariable();

            //Apply behaviour on the TileLayer
            TileLayer changedTileLayer = ApplyBehaviourOnTileLayer((TileLayer)inputTileLayer);

            maskableOutputTileLayer = new TileLayerMask((TileLayer)changedTileLayer.Clone());

            if (nodePreview.ShouldPreview())
            {
                nodePreview.ShowTexture(changedTileLayer.GetTileLayerPreviewTexture());
            }

            //Return the results
            return changedTileLayer;
        }

        public TileLayerMask GetMaskableTileLayerOutput()
        {
            //If an output has already been generated
            if (maskableOutputTileLayer == null)
            {
                //Apply this nodes functionality on the input TileLayer
                GetTileLayerOutput();
            }

            //Return a clone of the saved results
            return (TileLayerMask)maskableOutputTileLayer.Clone();
        }

        public override void ResetNodeVariables()
        {
            outputTileLayer = null;

            base.ResetNodeVariables();
        }

        private Texture2D GetPreviewTexture()
        {
            //TODO: Preview is called twice
            return GetTileLayerOutput().GetTileLayerPreviewTexture();
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
