using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Tilemaps;
#if (UNITY_EDITOR)
using UnityEditor.UIElements;
#endif

namespace TerraTiler2D
{
    public struct TileMask
    {
        public TileMask(TileLayerMask targetLayer, List<TileBase> tilesToMask)
        {
            this.targetLayer = targetLayer;
            this.tilesToMask = tilesToMask;
        }

        public List<TileBase> tilesToMask;
        public TileLayerMask targetLayer;
    }

    public class TileMask_Node : Node
    {
        private Port<TileBase> tilePort;
        private Port<TileLayerMask> tileLayerMaskPort;

        private Port<TileMask> tileMaskPort;

        public TileMask_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TileMask;
            SetTooltip("Restricts placement of tiles to areas defined in this mask.");
            searchMenuEntry = new string[] { "Variables" };
        }

        protected override void InitializeInputPorts()
        {
            tileLayerMaskPort = GeneratePort<TileLayerMask>("Mask from", PortDirection.Input, "TileLayerMask", PortCapacity.Single, true, "The TileLayerMask to take the mask from.");

            tilePort = GeneratePort<TileBase>("Tiles", PortDirection.Input, "Tile", PortCapacity.Multi, false, "The tiles to mask from the TileLayerMask. Multiple connections to this port are possible.");
        }

        protected override void InitializeOutputPorts()
        {
            tileMaskPort = GeneratePort<TileMask>("Tile mask", PortDirection.Output, "TileMaskOutput", PortCapacity.Multi, false, "The created TileMask. A TileMask will only take the tiles from the 'Mask from' TileLayer with a type that was passed into the Tiles port, and ignore all the other tiles.");
            tileMaskPort.SetOutputPortMethod(GetOutput);
        }

        protected override void InitializeAdditionalElements()
        {

        }

        public object GetOutput()
        {
            TileMask tileMask = new TileMask((TileLayerMask)tileLayerMaskPort.GetPortVariable(), this.GetPortVariables<TileBase>((List<object>)tilePort.GetPortVariable()));

            return tileMask;
        }
    }
}