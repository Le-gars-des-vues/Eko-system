using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class TilePlacerTunnelerAStar_Node : TilePlacer_Node
    {
        private PortWithField<int> tunnelWidthPort;
        private PortWithField<float> tunnelWeightPort;

        private Pathfinder_AStar myPathfinder;

        //========== Initialization ==========

        public TilePlacerTunnelerAStar_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TilePlacerTunnelerAStar;
            SetTooltip("Connects all shapes consisting out of 'Tile' using the A* path finding algorithm. NOTE: The A* algorithm is heavy on the performance of your graph. It is advised to not use this node for worlds larger than 100x100 tiles.");
            searchMenuEntry = new string[] { "Flow" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            tunnelWidthPort = GeneratePortWithField<int>("Tunnel width", PortDirection.Input, 1, "TunnelWidth", PortCapacity.Single, false, "How wide should the tunnels be.");

            tunnelWeightPort = GeneratePortWithField<float>("Tunnel weight", PortDirection.Input, 2, "TunnelWeight", PortCapacity.Single, false, "How hard should it be for the A* algorithm to place new tunnels. A higher value will result in less connecting tunnels. Can not be lower than 1.");
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

        public override TileLayer ApplyBehaviourOnTileLayer(TileLayer tileLayer)
        {
            base.ApplyBehaviourOnTileLayer(tileLayer);

            int tileIndex = tileLayer.GetIndexByTile(GetTile());

            int tunnelWidth = (int)tunnelWidthPort.GetPortVariable();
            tunnelWidth = Mathf.Max(1, tunnelWidth);
            float tunnelWeight = (float)tunnelWeightPort.GetPortVariable();
            tunnelWeight = Mathf.Max(1, tunnelWeight);

            //Get all the unconnected shapes
            List<List<Vector2Int>> allShapes = GetAllShapes(tileLayer, tileIndex);
            //If there is only 1 shape or less, there is nothing to connect.
            if (allShapes.Count <= 1)
            {
                return tileLayer;
            }

            TileMask tileMask = GetTileMask(tileLayer);

            //Assign a weight to every tile index
            Dictionary<int, float> tileWeightDictionary = new Dictionary<int, float>();
            //Give the passed in tile the lowest possible weight
            tileWeightDictionary.Add(tileIndex, 1);

            //Give all the other tile indexes the passed in weight
            var tileIndexes = tileLayer.GetTileIndexDictionary().GetEnumerator();
            while (tileIndexes.MoveNext())
            {
                //If the tile index is not the one we are placing
                if (tileIndexes.Current.Key != tileIndex)
                {
                    tileWeightDictionary.Add(tileIndexes.Current.Key, tunnelWeight);
                }
            }

            //Create a new A* pathfinder, and give it the tile weight dictionary
            myPathfinder = new Pathfinder_AStar(tileLayer, tileWeightDictionary, tileMask);

            //For every unconnected shape
            for (int startShape = 0; startShape < allShapes.Count - 1; startShape++)
            {
                //For every unconnected shape after the previous shape
                for (int targetShape = startShape + 1; targetShape < allShapes.Count; targetShape++)
                {
                    //Generate a path from this shape to the next
                    List<Vector2Int> path = myPathfinder.GetPath(allShapes[startShape], allShapes[targetShape]);

                    //For every tile in the path
                    for (int i = 0; i < path.Count; i++)
                    {
                        //Set the generated tile at that index to the tileIndex.
                        generatedTiles[path[i].x, path[i].y] = tileIndex;

                        //If the tunnel should be more than 1 tile wide
                        if (tunnelWidth > 1)
                        {
                            //Add the tiles to the left and right of the previous tile to the path to make it wider
                            for (int j = -Mathf.FloorToInt(tunnelWidth * 0.5f); j < Mathf.CeilToInt(tunnelWidth * 0.5f); j++)
                            {
                                //Add the tiles above and below the previous tile to the path to make it wider
                                for (int k = -Mathf.FloorToInt(tunnelWidth * 0.5f); k < Mathf.CeilToInt(tunnelWidth * 0.5f); k++)
                                {
                                    if (path[i].x + j >= 0 &&
                                        path[i].x + j < generatedTiles.GetLength(0) &&
                                        path[i].y + k >= 0 &&
                                        path[i].y + k < generatedTiles.GetLength(1))
                                    {
                                        generatedTiles[path[i].x + j, path[i].y + k] = tileIndex;
                                    }
                                }
                            }
                        }
                    }

                    //Apply the generatedTiles to the tileLayer
                    ApplyChanges(tileLayer);
                }
            }

            return tileLayer;
        }

        protected override TileMask GetTileMask(TileLayer tileLayer)
        {
            TileMask tileMask = base.GetTileMask(tileLayer);

            //If the TileMask does not contain the TileBase connected to the tilePort
            if (!tileMask.tilesToMask.Contains(GetTile()))
            {
                //If the TileMask is the same size as the tileLayer
                if (tileMask.targetLayer.generatedTiles.GetLength(0) == tileLayer.generatedTiles.GetLength(0))
                {
                    if (tileMask.targetLayer.generatedTiles.GetLength(1) == tileLayer.generatedTiles.GetLength(1))
                    {
                        //Check if all the tileIndexes are the same between the TileMask and the tileLayer
                        for (int x = 0; x < tileLayer.generatedTiles.GetLength(0); x++)
                        {
                            for (int y = 0; y < tileLayer.generatedTiles.GetLength(1); y++)
                            {
                                //If 1 tile is not the same, they are different layers
                                if (tileMask.targetLayer.generatedTiles[x, y] != tileLayer.generatedTiles[x, y])
                                {
                                    //Return the TileMaskObject
                                    return tileMask;
                                }
                            }
                        }

                        //The TileMask references the same layer as the defaultLayer, but does not include the target TileBase object.
                        //This causes odd behaviour that produces ugly pathfinding, because the pathfinder node will try to connect shapes consisting of the target TileBase, but also try to avoid all of the target TileBase objects when calculating a path.
                        //To fix this, we simply add the target TileBase to the TileMask. This can be done safely, because there is no harm in replacing tiles with the exact same tile.
                        List<TileBase> newTilesToMask = new List<TileBase>();
                        newTilesToMask.AddRange(tileMask.tilesToMask);
                        newTilesToMask.Add(GetTile());

                        return new TileMask(tileMask.targetLayer, newTilesToMask);
                    }
                }
            }

            return tileMask;
        }

        // ===== Get unconnected shapes =====

        private List<List<Vector2Int>> GetAllShapes(TileLayer tileLayer, int tileIndex)
        {
            //Create a list to hold all the shapes
            List<List<Vector2Int>> allShapes = new List<List<Vector2Int>>();

            //Create a dictionary to lookup to which shape a tile belongs (this is better performance than checking if a shape contains a specific tile)
            Dictionary<Vector2Int, List<Vector2Int>> shapeDictionary = new Dictionary<Vector2Int, List<Vector2Int>>();

            //For every tile along the Y axis
            for (int y = 0; y < tileLayer.generatedTiles.GetLength(1); y++)
            {
                //For every tile along the X axis
                for (int x = 0; x < tileLayer.generatedTiles.GetLength(0); x++)
                {
                    //If the tile is of the target tileIndex
                    if (tileLayer.generatedTiles[x, y] == tileIndex)
                    {
                        //Add the tile to a shape
                        AddToShape(tileLayer, tileIndex, allShapes, shapeDictionary, new Vector2Int(x,y));
                    }
                }
            }

            return allShapes;
        }

        private void AddToShape(TileLayer tileLayer, int tileIndex, List<List<Vector2Int>> allShapes, Dictionary<Vector2Int, List<Vector2Int>> shapeDictionary, Vector2Int tilePosition)
        {
            //Prepare a variable to hold the bordering shapes, but dont create it yet to save on memory usage
            List<List<Vector2Int>> borderingShapes = null;

            //Check for each neighboring tile if they could be part of a shape
            bool leftIsPart = IsPartOfShape(tileLayer, tileIndex, tilePosition.x - 1, tilePosition.y);
            bool downIsPart = IsPartOfShape(tileLayer, tileIndex, tilePosition.x, tilePosition.y - 1);
            bool rightIsPart = IsPartOfShape(tileLayer, tileIndex, tilePosition.x + 1, tilePosition.y);
            bool upIsPart = IsPartOfShape(tileLayer, tileIndex, tilePosition.x, tilePosition.y + 1);

            //If at least 1 neighboring tile can be part of a shape
            if (leftIsPart || downIsPart || rightIsPart || upIsPart)
            {
                //Prepare variables to hold the bordering shapes, but dont create them yet to save on memory usage
                List<Vector2Int> shapeLeft;
                List<Vector2Int> shapeDown;
                List<Vector2Int> shapeRight;
                List<Vector2Int> shapeUp;

                //If the left neighbor can be part of a shape, and is already part of a shape
                if (leftIsPart && shapeDictionary.TryGetValue(new Vector2Int(tilePosition.x - 1, tilePosition.y), out shapeLeft))
                {
                    //Initialize the borderingShapes list
                    if (borderingShapes == null)
                    {
                        borderingShapes = new List<List<Vector2Int>>();
                    }

                    //If this shape is not yet registered as a bordering shape
                    if (!borderingShapes.Contains(shapeLeft))
                    {
                        borderingShapes.Add(shapeLeft);
                    }
                }
                //If the bottom neighbor can be part of a shape, and is already part of a shape
                if (downIsPart && shapeDictionary.TryGetValue(new Vector2Int(tilePosition.x, tilePosition.y - 1), out shapeDown))
                {
                    //Initialize the borderingShapes list
                    if (borderingShapes == null)
                    {
                        borderingShapes = new List<List<Vector2Int>>();
                    }

                    //If this shape is not yet registered as a bordering shape
                    if (!borderingShapes.Contains(shapeDown))
                    {
                        borderingShapes.Add(shapeDown);
                    }
                }
                //If the right neighbor can be part of a shape, and is already part of a shape
                if (rightIsPart && shapeDictionary.TryGetValue(new Vector2Int(tilePosition.x + 1, tilePosition.y), out shapeRight))
                {
                    //Initialize the borderingShapes list
                    if (borderingShapes == null)
                    {
                        borderingShapes = new List<List<Vector2Int>>();
                    }

                    //If this shape is not yet registered as a bordering shape
                    if (!borderingShapes.Contains(shapeRight))
                    {
                        borderingShapes.Add(shapeRight);
                    }
                }
                //If the top neighbor can be part of a shape, and is already part of a shape
                if (upIsPart && shapeDictionary.TryGetValue(new Vector2Int(tilePosition.x, tilePosition.y + 1), out shapeUp))
                {
                    //Initialize the borderingShapes list
                    if (borderingShapes == null)
                    {
                        borderingShapes = new List<List<Vector2Int>>();
                    }

                    //If this shape is not yet registered as a bordering shape
                    if (!borderingShapes.Contains(shapeUp))
                    {
                        borderingShapes.Add(shapeUp);
                    }
                }
            }

            //If there are no bordering shapes
            if (borderingShapes == null || borderingShapes.Count <= 0)
            {
                //Create a new shape
                List<Vector2Int> newShape = new List<Vector2Int>();
                //Add this tile to the new shape
                newShape.Add(tilePosition);

                //Add the new shape to the shape list
                allShapes.Insert(Random.Range(0, allShapes.Count), newShape);

                //Add the tile position to the dictionary so that shape lookups become easier
                shapeDictionary.Add(tilePosition, newShape);
            }
            //If there is exactly 1 bordering shape
            else if (borderingShapes.Count == 1)
            {
                //Add this tile to the bordering shape
                borderingShapes[0].Add(tilePosition);

                //Add the tile position to the dictionary so that shape lookups become easier
                shapeDictionary.Add(tilePosition, borderingShapes[0]);
            }
            //If there is more than 1 bordering shape
            else
            {
                //Add this tile to the first bordering shape
                borderingShapes[0].Add(tilePosition);

                //Add the tile position to the dictionary so that shape lookups become easier
                shapeDictionary.Add(tilePosition, borderingShapes[0]);

                //Merge all of the bordering shapes
                MergeShapes(allShapes, borderingShapes, shapeDictionary);
            }
        }

        private void MergeShapes(List<List<Vector2Int>> allShapes, List<List<Vector2Int>> shapesToMerge, Dictionary<Vector2Int, List<Vector2Int>> shapeDictionary)
        {
            List<Vector2Int> newShape = new List<Vector2Int>();

            //For every shape to merge
            for (int i = 0; i < shapesToMerge.Count; i++)
            {
                //For every tile in the shape to merge
                for (int j = 0; j < shapesToMerge[i].Count; j++)
                {
                    //Update the dictionary entry
                    shapeDictionary.Remove(shapesToMerge[i][j]);
                    shapeDictionary.Add(shapesToMerge[i][j], newShape);
                }

                //Add all the tiles of the shape to the new shape
                newShape.AddRange(shapesToMerge[i]);

                //Remove the merged shape
                allShapes.Remove(shapesToMerge[i]);
            }
            //Add the new shape
            allShapes.Insert(Random.Range(0, allShapes.Count), newShape);
        }

        private bool IsPartOfShape(TileLayer tileLayer, int tileIndex, int xPos, int yPos)
        {
            //If the neighbor is a valid tile
            if (xPos >= 0 && xPos < tileLayer.generatedTiles.GetLength(0) &&
                yPos >= 0 && yPos < tileLayer.generatedTiles.GetLength(1))
            {
                //If a neighboring tile is a null tile
                if (tileLayer.generatedTiles[xPos, yPos] == tileIndex)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
