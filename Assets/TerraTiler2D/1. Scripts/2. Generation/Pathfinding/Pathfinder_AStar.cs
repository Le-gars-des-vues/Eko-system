using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class Pathfinder_AStar : Pathfinder_Abstract
    {
        private class AStar_Tile
        {
            public AStar_Tile(Vector2Int myPosition, float tileWeight, bool isOccupied = false)
            {
                this.myPosition = myPosition;
                this.tileWeight = tileWeight;
                this.isOccupied = isOccupied;
            }

            public Vector2Int myPosition;

            public AStar_Tile parentTile;

            public float tileWeight;
            public bool isOccupied = false;

            public float costEstimate;
            public float costCurrent;
            public float costTotal;

            private AStar_Tile upConnection;
            private AStar_Tile leftConnection;
            private AStar_Tile downConnection;
            private AStar_Tile rightConnection;

            public void AddConnection(AStar_Tile connection, TileDirection direction)
            {
                switch (direction)
                {
                    case TileDirection.Up:
                        upConnection = connection;
                        break;
                    case TileDirection.Left:
                        leftConnection = connection;
                        break;
                    case TileDirection.Down:
                        downConnection = connection;
                        break;
                    case TileDirection.Right:
                        rightConnection = connection;
                        break;
                    default:
                        break;
                }
            }

            public AStar_Tile GetConnection(TileDirection direction)
            {
                switch (direction)
                {
                    case TileDirection.Up:
                        return upConnection;
                    case TileDirection.Left:
                        return leftConnection;
                    case TileDirection.Down:
                        return downConnection;
                    case TileDirection.Right:
                        return rightConnection;
                    default:
                        return null;
                }
            }
        }

        private Dictionary<int, float> tileWeightDictionary;
        private TileMask tileMask;

        private List<AStar_Tile> _todoList = new List<AStar_Tile>();
        private List<AStar_Tile> _doneList = new List<AStar_Tile>();
        private AStar_Tile _activeTile;

        private bool isDone = true;
        private List<AStar_Tile> _lastPathFound = new List<AStar_Tile>();

        public Pathfinder_AStar(TileLayer tileLayer, Dictionary<int, float> tileWeightDictionary, TileMask tileMask) : base(tileLayer)
        {
            this.tileWeightDictionary = tileWeightDictionary;
            this.tileMask = tileMask;
        }

        public override List<Vector2Int> GetPath(List<Vector2Int> startShape, List<Vector2Int> endShape)
        {
            Vector2Int[] closestTiles = GetClosestShapeTiles(startShape, endShape);

            Vector2Int startPosition = closestTiles[0];
            Vector2Int endPosition = closestTiles[1];

            //Reset the pathfinder
            resetPathFinder();

            //Create an AStar_Tile object for every tile
            AStar_Tile[,] allTiles = new AStar_Tile[tileLayer.generatedTiles.GetLength(0), tileLayer.generatedTiles.GetLength(1)];
            for (int x = 0; x < allTiles.GetLength(0); x++)
            {
                for (int y = 0; y < allTiles.GetLength(1); y++)
                {
                    //Set the weight of the tile based on the dictionary
                    bool isOccupied = false;

                    //If the tile mask does not allow us to overwrite the tile at position (x,y), set the tile to unpassable.
                    if (!tileMask.tilesToMask.Contains(tileMask.targetLayer.GetTileByIndex(tileMask.targetLayer.generatedTiles[x,y])))
                    {
                        isOccupied = true;
                    }

                    allTiles[x,y] = new AStar_Tile(new Vector2Int(x, y), tileWeightDictionary[tileLayer.generatedTiles[x, y]], isOccupied);
                }
            }

            //Connect all the AStar_Tiles together
            for (int x = 0; x < allTiles.GetLength(0); x++)
            {
                for (int y = 0; y < allTiles.GetLength(1); y++)
                {
                    if (x-1 >= 0)
                    {
                        allTiles[x, y].AddConnection(allTiles[x-1, y], TileDirection.Left);
                    }
                    if (y-1 >= 0)
                    {
                        allTiles[x, y].AddConnection(allTiles[x, y-1], TileDirection.Down);
                    }
                    if (x+1 < allTiles.GetLength(0))
                    {
                        allTiles[x, y].AddConnection(allTiles[x+1, y], TileDirection.Right);
                    }
                    if (y+1 < allTiles.GetLength(1))
                    {
                        allTiles[x, y].AddConnection(allTiles[x, y+1], TileDirection.Up);
                    }
                }
            }

            //Create tiles for the start and end position
            AStar_Tile startTile = allTiles[startPosition.x, startPosition.y];
            AStar_Tile endTile = allTiles[endPosition.x, endPosition.y];

            //Add the start tile to the todo list, so that it will be processed upon calling FindPath()
            _todoList.Add(startTile);

            //Call FindPath() to get the path from startTile to endTile
            List<AStar_Tile> pathHolder = FindPath(startTile, endTile);

            //Convert the list from AStar_Tile to Vector2Int
            List<Vector2Int> path = new List<Vector2Int>();
            for (int i = 0; i < pathHolder.Count; i++)
            {
                path.Add(pathHolder[i].myPosition);
            }

            return path;
        }

        //Finds a path on a grid using an A* algorithm.
        //Takes a start tile, and adds all connected tiles to the _todoList. 
        //It then calls this function again, but adds all the tiles connected to the first tile in the _todoList.
        //It keeps doing this until the end tile is found, at which point the shortest path is calculated.
        private List<AStar_Tile> FindPath(AStar_Tile startTile, AStar_Tile endTile)
        {
            //are we able to find a path??
            if (isDone || startTile == null || endTile == null || _todoList.Count == 0)
            {
                isDone = true;
                return _lastPathFound;
            }

            //we are not done, start and end are set and there is at least 1 item on the open list...
            //check if we were already processing tiles
            _activeTile = _todoList[0];
            _todoList.RemoveAt(0);

            //and move that tile to the closed list (one way or another, we are done with it...)
            _doneList.Add(_activeTile);

            //Is this our end tile? Yay, we are done!
            if (_activeTile == endTile)
            {
                _lastPathFound.Clear();

                AStar_Tile tile = endTile;
                if (endTile.isOccupied) //If the end tile is occupied.
                {
                    tile = tile.parentTile; //Skip the end tile, get the path to the closest tile instead
                }

                while (tile != null)
                {
                    _lastPathFound.Insert(0, tile);
                    tile = tile.parentTile;
                }

                return _lastPathFound;
            }
            else
            {
                //get all connected tiles and process them
                for (int i = 0; i < (int)TileDirection.End; i++)
                {
                    AStar_Tile connectedTile = _activeTile.GetConnection((TileDirection)i);

                    if (connectedTile == null)
                    {
                        continue;
                    }

                    //If the connected tile is not occupied, or if it is our end tile.
                    if (!connectedTile.isOccupied || connectedTile == endTile)
                    {
                        //If the connected tile is not in the _doneList, and the connected tile is not on the _todoList.
                        if (!_doneList.Contains(connectedTile) && !_todoList.Contains(connectedTile))
                        {
                            //Add the connected tile to the _todoList with the current tile as its parent.
                            //Calculate the cost of traversing this tile and store it.
                            connectedTile.parentTile = _activeTile;
                            connectedTile.costEstimate = Vector2.Distance(endTile.myPosition, connectedTile.myPosition);
                            connectedTile.costCurrent = _activeTile.costCurrent + connectedTile.tileWeight;
                            connectedTile.costTotal = connectedTile.costEstimate + connectedTile.costCurrent;

                            _todoList.Add(connectedTile);
                        }
                        //If a new cheaper path to get to this tile is found, update the information.
                        else if (connectedTile.costCurrent > _activeTile.costCurrent + connectedTile.tileWeight)
                        {
                            connectedTile.costCurrent = _activeTile.costCurrent + connectedTile.tileWeight;
                            connectedTile.costTotal = connectedTile.costEstimate + connectedTile.costCurrent;
                            connectedTile.parentTile = _activeTile;
                        }
                    }
                }

                SortTileListByCost(_todoList);

                //Check the next tile in the _todoList.
                _lastPathFound = FindPath(_activeTile, endTile);
            }

            isDone = true;
            return _lastPathFound;
        }

        private void resetPathFinder()
        {
            //If the pathfinder is still busy generating a path
            if (!isDone)
            {
                return;
            }

            _todoList.Clear();
            _doneList.Clear();
            _lastPathFound.Clear();

            _activeTile = null;

            isDone = false;
        }

        void SortTileListByCost(List<AStar_Tile> tileList)
        {
            List<AStar_Tile> valueHolder = new List<AStar_Tile>();
            float highestCost = 9999999999;
            int highestCostIndex = -1;
            for (int i = 0; i < tileList.Count; i++)
            {
                highestCost = 9999999999;
                highestCostIndex = -1;
                for (int j = 0; j < tileList.Count; j++)
                {
                    if (!valueHolder.Contains(tileList[j]))
                    {
                        if (tileList[j].costTotal <= highestCost)
                        {
                            highestCost = tileList[j].costTotal;
                            highestCostIndex = j;
                        }
                    }
                }
                if (highestCostIndex != -1)
                {
                    valueHolder.Add(tileList[highestCostIndex]);
                }
            }

            tileList.Clear();
            tileList.AddRange(valueHolder);
        }
    }
}
