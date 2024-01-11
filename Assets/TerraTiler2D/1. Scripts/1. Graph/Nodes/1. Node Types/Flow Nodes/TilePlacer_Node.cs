using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public abstract class TilePlacer_Node : FlowTileLayer_Node
    {
        private Port<TileMask> tileMaskPort;
        private PortWithField<TileBase> tilePort;

        protected int[,] generatedTiles;

        //========== Initialization ==========

        public TilePlacer_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {

        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            tileMaskPort = GeneratePort<TileMask>("Tile mask", PortDirection.Input, "TileMask", PortCapacity.Single, false, "If defined, this node will only replace existing tiles that are part of the Tile mask.");

            tilePort = GeneratePortWithField<TileBase>("Tile", PortDirection.Input, null, "Tile", PortCapacity.Single, false, "The tile to place.");
        }

        protected override void InitializeOutputPorts()
        {
            base.InitializeOutputPorts();
        }

        protected override void InitializeAdditionalElements()
        {
            base.InitializeAdditionalElements();
        }

        //========== Node methods ==========

        public override void ApplyBehaviour(Flow flow, bool trickleDown = true, bool waitingOnResult = false)
        {
            base.ApplyBehaviour(flow, trickleDown);
        }

        protected TileBase GetTile()
        {
            return (TileBase)tilePort.GetPortVariable();
        }
        protected virtual TileMask GetTileMask(TileLayer defaultLayer)
        {
            //Get the tile mask from the tileMaskPort
            object tileMaskObject = tileMaskPort.GetPortVariable();
            //If there is a connection to the TileMaskPort
            if (tileMaskObject != null)
            {
                return (TileMask)tileMaskObject;
            }

            //If no tile mask was passed in, generate a default tile mask
            List<TileBase> tilesToMask = new List<TileBase>();
            tilesToMask.AddRange(defaultLayer.GetTileIndexDictionary().Values);

            return new TileMask(new TileLayerMask(defaultLayer), tilesToMask);
        }

        public override TileLayer ApplyBehaviourOnTileLayer(TileLayer tileLayer)
        {
            //Add the tile and its index to the dictionary.
            tileLayer.AddTileIndex(GetTile());

            //Create an empty 2D array to hold the indexes of placed shapes.
            generatedTiles = new int[tileLayer.generatedTiles.GetLength(0), tileLayer.generatedTiles.GetLength(1)];
            //Set all the indexes to invalid
            for (int x = 0; x < generatedTiles.GetLength(0); x++)
            {
                for (int y = 0; y < generatedTiles.GetLength(1); y++)
                {
                    generatedTiles[x, y] = Glob.GetInstance().InvalidTileIndex;
                }
            }

            return tileLayer;
        }
        protected void ApplyChanges(TileLayer tileLayer)
        {
            TileMask tileMask = GetTileMask(tileLayer);
            int tileIndex = tileLayer.GetIndexByTile(GetTile());

            //For every generated tile along the Y axis
            for (int y = 0; y < Mathf.Min(tileMask.targetLayer.generatedTiles.GetLength(1), tileLayer.generatedTiles.GetLength(1)); y++)
            {
                //For every generated tile along the X axis
                for (int x = 0; x < Mathf.Min(tileMask.targetLayer.generatedTiles.GetLength(0), tileLayer.generatedTiles.GetLength(0)); x++)
                {
                    //If this node generated a tile on position [x,y]
                    if (generatedTiles[x,y] != Glob.GetInstance().InvalidTileIndex)
                    {
                        //If the tile mask is empty, or
                        //If the tile mask contains the tile currently at position [x,y]
                        if (tileMask.tilesToMask.Count == 0 || tileMask.tilesToMask.Contains(tileMask.targetLayer.GetTileByIndex(tileMask.targetLayer.generatedTiles[x, y])))
                        {
                            //Overwrite the tile
                            tileLayer.generatedTiles[x, y] = tileIndex;
                        }
                    }
                }
            }
        }

        //========== Port data passing ==========

    }
}
