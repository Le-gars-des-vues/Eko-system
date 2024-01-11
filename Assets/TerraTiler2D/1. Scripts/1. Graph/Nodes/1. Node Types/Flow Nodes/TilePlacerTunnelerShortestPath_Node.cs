using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class TilePlacerTunnelerShortestPath_Node : TilePlacer_Node
    {
        private PortWithField<int> tunnelWidthPort;

        private PortWithField<int> tunnelRandomizationPort;

        private PortWithField<bool> diagonalTunnelPort;
        private PortWithField<bool> multiTunnelPort;

        private Pathfinder_ShortestPath myPathfinder;

        //========== Initialization ==========

        public TilePlacerTunnelerShortestPath_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TilePlacerTunnelerShortestPath;
            SetTooltip("Places tiles to connect all shapes consisting out of 'Tile' using the shortest possible path. This tunneler has significantly better performance than the A* tunneler, but does not try to avoid obstacles.");
            searchMenuEntry = new string[] { "Flow" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            tunnelWidthPort = GeneratePortWithField<int>("Tunnel width", PortDirection.Input, 1, "TunnelWidth", PortCapacity.Single, false, "How wide should the tunnels be.");

            tunnelRandomizationPort = GeneratePortWithField<int>("Randomization", PortDirection.Input, 25, "Randomization", PortCapacity.Single, false, "How much should the direction of tunnels be randomized on a scale from 0 to 100.");

            diagonalTunnelPort = GeneratePortWithField<bool>("Diagonal tunnels", PortDirection.Input, false, "Diagonal", PortCapacity.Single, false, "Are tunnels allowed to go diagonal?");
            multiTunnelPort = GeneratePortWithField<bool>("Multi tunnel", PortDirection.Input, false, "Multi", PortCapacity.Single, false, "If true, a tunnel is created from each shape to all other shapes. If false, each shape will only create one tunnel to the next shape.");
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

            int tunnelRandomization = Mathf.Clamp((int)tunnelRandomizationPort.GetPortVariable(), 0, 100);

            bool multiTunnel = (bool)multiTunnelPort.GetPortVariable();
            bool allowDiagonal = (bool)diagonalTunnelPort.GetPortVariable();

            //Get all the unconnected shapes
            List<List<Vector2Int>> allShapes = GetAllShapes(tileLayer, tileIndex);

            //If there is only 1 shape or less, there is nothing to connect.
            if (allShapes.Count <= 1)
            {
                return tileLayer;
            }

            //Create a new A* pathfinder, and give it the tile weight dictionary
            myPathfinder = new Pathfinder_ShortestPath(tileLayer, allowDiagonal, tunnelRandomization);

            int multiTunnelToggle = allShapes.Count;

            //For every unconnected shape
            for (int startShape = 0; startShape < allShapes.Count - 1; startShape++)
            {
                if (!multiTunnel)
                {
                    multiTunnelToggle = startShape + 2;
                }

                //For every unconnected shape after the previous shape
                for (int targetShape = startShape + 1; targetShape < multiTunnelToggle; targetShape++)
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
