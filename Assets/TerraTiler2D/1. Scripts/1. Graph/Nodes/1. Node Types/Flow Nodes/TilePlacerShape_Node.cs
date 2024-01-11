using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class TileShape
    {
        public TileShape(Vector2 size, Vector2 position)
        {
            shape = new int[(int)size.x, (int)size.y];

            for (int x = 0; x < (int)size.x; x++)
            {
                for (int y = 0; y < (int)size.y; y++)
                {
                    shape[x, y] = Glob.GetInstance().InvalidTileIndex;
                }
            }

            this.position = position;
        }

        public int[,] shape;

        public Vector2 position;
    }

    public abstract class TilePlacerShape_Node : TilePlacer_Node
    {
        private PortWithField<int> shapeCountPort;
        private PortWithField<Vector2> minShapeDistancePort;
        private Port<Vector2> shapePositionPort;
        private PortWithField<float> rotationPort;

        //========== Initialization ==========

        public TilePlacerShape_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {

        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            shapeCountPort = GeneratePortWithField<int>("Shape count", PortDirection.Input, 10, "ShapeCount", PortCapacity.Single, false, "The amount of shapes that will be placed.");

            minShapeDistancePort = GeneratePortWithField<Vector2>("Shape distance", PortDirection.Input, new Vector2(-1, -1), "ShapeDist", PortCapacity.Single, false, "What is the minimum distance that should be kept between shapes? If both axes have a negative value, the shapes are allowed to overlap completely.");

            shapePositionPort = GeneratePort<Vector2>("Position", PortDirection.Input, "Pos", PortCapacity.Single, false, "On what position should the shapes be placed. No connection places them randomly.");

            rotationPort = GeneratePortWithField<float>("Rotation", PortDirection.Input, 0, "Rot", PortCapacity.Single, false, "What should the rotation of the shapes be in degrees. NOTE: This is a grid-bound rotation, meaning that the resulting shapes are likely different than expected.");
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
            //Let the base class add the tile index to the tile layer, and prepare an empty 2D tile index array
            base.ApplyBehaviourOnTileLayer(tileLayer);

            int tileIndex = tileLayer.GetIndexByTile(GetTile());

            TileMask tileMask = GetTileMask(tileLayer);

            //Get the amount of shapes that should be placed
            int shapeCount = (int)shapeCountPort.GetPortVariable();
            //Holds the amount of shapes that have been placed
            int placedShapes = 0;
            //Keeps track of how many times this node failed to place a shape in a row
            int failSafeCount = 0;
            //As long as this node has not placed enough shapes yet
            while (placedShapes < shapeCount)
            {
                //If this node succesfully placed a shape
                if (PlaceShape(tileIndex, tileMask))
                {
                    //Add 1 placed shape
                    placedShapes++;
                    //Reset the fail safe
                    failSafeCount = 0;
                }
                //If this node failed to place a shape
                else
                {
                    //Add 1 to the fail safe
                    failSafeCount++;
                    //If this node failed to place a shape 50 times in a row
                    if (failSafeCount >= 500)
                    {
                        //Warn the user
                        Glob.GetInstance().DebugString("Node '" + GetTitle() + "' attempted to place " + shapeCount + " shapes that do not overlap, but failed to place more than " + placedShapes + ".", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                        //Exit out of this while loop
                        break;
                    }
                }
            }

            ApplyChanges(tileLayer);

            return tileLayer;
        }

        protected abstract TileShape CreateShape(int tileIndex, Vector2 position);

        protected virtual bool PlaceShape(int tileIndex, TileMask tileMask)
        {
            //Try to get a position
            object positionObject = shapePositionPort.GetPortVariable();
            //If no Vector2 is connected to the port
            if (positionObject == null)
            {
                //Generate a random position within the TileShape
                positionObject = new Vector2(Random.Range(0, generatedTiles.GetLength(0)), Random.Range(0, generatedTiles.GetLength(1)));
            }
            Vector2 position = (Vector2)positionObject;

            //Let sub-classes create a shape
            TileShape shape = CreateShape(tileIndex, position);

            //Get the rotation
            float rotation = (float)rotationPort.GetPortVariable();
            //If the shape should be rotated at least 1 degree
            if (rotation % 360 != 0)
            {
                //Rotate the shape
                shape = RotateTileShape(shape, rotation, tileIndex);
            }

            //Get the minimum distance each shape should be from each other
            Vector2 shapeDistance = (Vector2)minShapeDistancePort.GetPortVariable();

            //If the shape does not overlap any other shapes
            if (IsShapeValid(shape, shapeDistance, tileMask))
            {
                //add the shape to the tileLayer
                addShape(shape);
                return true;
            }

            return false;
        }
        private void addShape(TileShape shape)
        {
            for (int x = 0; x < shape.shape.GetLength(0); x++)
            {
                for (int y = 0; y < shape.shape.GetLength(1); y++)
                {
                    if (shape.shape[x,y] != Glob.GetInstance().InvalidTileIndex)
                    {
                        if (shape.position.x + x < generatedTiles.GetLength(0) && shape.position.y + y < generatedTiles.GetLength(1))
                        {
                            generatedTiles[((int)shape.position.x) + x, ((int)shape.position.y) + y] = shape.shape[x, y];
                        }
                    }
                }
            }
        }

        protected bool IsShapeValid(TileShape shape, Vector2 shapeDistance, TileMask tileMask)
        {
            //If the shape does not completely fall within the tile layer
            if (!IsShapeContained(shape, tileMask))
            {
                return false;
            }
            //If the tile violates the overlap rule
            else if (DoesShapeOverlap(shape, shapeDistance))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        protected bool DoesShapeOverlap(TileShape shape, Vector2 shapeDistance)
        {
            //If the shape is not allowed to overlap
            if (shapeDistance.x >= 0 || shapeDistance.y >= 0)
            {
                //For every tile in the shape
                for (int x = 0; x < shape.shape.GetLength(0); x++)
                {
                    for (int y = 0; y < shape.shape.GetLength(1); y++)
                    {
                        //If the shape tile is not an invalid tile
                        if (shape.shape[x, y] != Glob.GetInstance().InvalidTileIndex)
                        {
                            //If the tile falls within the generatedTiles array
                            if (shape.position.x + x < generatedTiles.GetLength(0) && shape.position.y + y < generatedTiles.GetLength(1))
                            {
                                //For every tile in the generatedTiles array that is within shapeDistance of the tile
                                for (int xDist = (int)-Mathf.Max(shapeDistance.x, 0); xDist <= (int)Mathf.Max(shapeDistance.x, 0); xDist++)
                                {
                                    for (int yDist = (int)-Mathf.Max(shapeDistance.y, 0); yDist <= (int)Mathf.Max(shapeDistance.y, 0); yDist++)
                                    {
                                        //If the tile falls within the generatedTiles array
                                        if ((int)shape.position.x + x + xDist < generatedTiles.GetLength(0) &&
                                            (int)shape.position.x + x + xDist >= 0 &&
                                            (int)shape.position.y + y + yDist < generatedTiles.GetLength(1) &&
                                            (int)shape.position.y + y + yDist >= 0)
                                        {
                                            //If the generatedTiles array already contains a valid tile at position [x,y]
                                            if (generatedTiles[(int)shape.position.x + x + xDist, (int)shape.position.y + y + yDist] != Glob.GetInstance().InvalidTileIndex)
                                            {
                                                //The shapes overlap
                                                return true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            //The shape does not violate the overlap rule
            return false;
        }
        protected bool IsShapeContained(TileShape shape, TileMask tileMask)
        {
            //For every tile in the shape
            for (int x = 0; x < shape.shape.GetLength(0); x++)
            {
                for (int y = 0; y < shape.shape.GetLength(1); y++)
                {
                    //If the tile is a valid tile
                    if (shape.shape[x, y] != Glob.GetInstance().InvalidTileIndex)
                    {
                        //If the tile falls outside the generatedTiles array bounds
                        if ((int)shape.position.x + x >= generatedTiles.GetLength(0) ||
                            (int)shape.position.x + x < 0 ||
                            (int)shape.position.y + y >= generatedTiles.GetLength(1) ||
                            (int)shape.position.y + y < 0)
                        {
                            //The shape is not contained
                            return false;
                        }

                        if (!tileMask.tilesToMask.Contains(tileMask.targetLayer.GetTileByIndex(tileMask.targetLayer.generatedTiles[(int)shape.position.x + x, (int)shape.position.y + y])))
                        {
                            //The shape is placed on a tile that is not part of the tile mask
                            return false;
                        }
                    }
                }
            }

            //The shape is contained
            return true;
        }


        private TileShape RotateTileShape(TileShape shape, float degrees, int tileIndex)
        {
            if (degrees % 90 == 0)
            {
                return RotateTileShapeBy90(shape, degrees, tileIndex);
            }

            //Create an empty dictionary to hold the rotated shape
            List<Vector2> rotatedTileIndexes = new List<Vector2>();

            //Prepare default values for the rotated shape size.
            int highestX = 0;
            int lowestX = 0;
            int highestY = 0;
            int lowestY = 0;

            //For every row of the unrotated shape
            for (int x = 0; x < shape.shape.GetLength(0); x++)
            {
                //For every column of the unrotated shape
                for (int y = 0; y < shape.shape.GetLength(1); y++)
                {
                    //If the unrotated shape holds a tile at this index
                    if (shape.shape[x, y] != Glob.GetInstance().InvalidTileIndex)
                    {
                        //Rotate the index
                        Vector2 rotatedCoordinates = Quaternion.AngleAxis(degrees, Vector3.forward) * new Vector2(x, y);
                        rotatedCoordinates.x = Mathf.Ceil(rotatedCoordinates.x);
                        rotatedCoordinates.y = Mathf.Ceil(rotatedCoordinates.y);
                        //Add the rotated index to the dictionary
                        rotatedTileIndexes.Add(new Vector2((int)rotatedCoordinates.x, (int)rotatedCoordinates.y));

                        //If the rotated index exceeds any of the boundaries, resize
                        if (rotatedCoordinates.x < lowestX)
                        {
                            lowestX = Mathf.CeilToInt(rotatedCoordinates.x);
                        }
                        if (rotatedCoordinates.x > highestX)
                        {
                            highestX = Mathf.CeilToInt(rotatedCoordinates.x);
                        }
                        if (rotatedCoordinates.y < lowestY)
                        {
                            lowestY = Mathf.CeilToInt(rotatedCoordinates.y);
                        }
                        if (rotatedCoordinates.y > highestY)
                        {
                            highestY = Mathf.CeilToInt(rotatedCoordinates.y);
                        }
                    }
                }
            }

            int rotatedWidth = highestX - lowestX + 1;
            int rotatedHeight = highestY - lowestY + 1;
            TileShape rotatedRectangle = new TileShape(new Vector2(rotatedWidth, rotatedHeight), shape.position);

            //For every row of the unrotated shape
            for (int x = lowestX; x <= highestX; x++)
            {
                //For every column of the unrotated shape
                for (int y = lowestY; y <= highestY; y++)
                {
                    if (rotatedTileIndexes.Contains(new Vector2(x, y)))
                    {
                        rotatedRectangle.shape[x - lowestX, y - lowestY] = tileIndex;

                        HandleNeighborFill(x - lowestX, y - lowestY, tileIndex, rotatedRectangle, true);
                    }
                }
            }

            return rotatedRectangle;
        }
        private TileShape RotateTileShapeBy90(TileShape shape, float degrees, int tileIndex)
        {
            Vector2 rotatedTileShapeSize = new Vector2(shape.shape.GetLength(0), shape.shape.GetLength(1));

            degrees = degrees % 360;

            if (degrees <= 90)
            {
                rotatedTileShapeSize = new Vector2(rotatedTileShapeSize.y, rotatedTileShapeSize.x);
            }
            //If the shape is being rotated by 180 degrees
            else if (degrees <= 180)
            {
                //No need to resize the tile shape
            }
            //If the shape is being rotated by 270 degrees
            else if (degrees <= 270)
            {
                rotatedTileShapeSize = new Vector2(rotatedTileShapeSize.y, rotatedTileShapeSize.x);
            }

            TileShape rotatedTileShape = new TileShape(rotatedTileShapeSize, shape.position);

            for (int x = 0; x < shape.shape.GetLength(0); x++)
            {
                for (int y = 0; y < shape.shape.GetLength(1); y++)
                {
                    if (degrees <= 90)
                    {
                        rotatedTileShape.shape[(int)rotatedTileShapeSize.x - 1 - y, x] = shape.shape[x, y];
                    }
                    //If the shape is being rotated by 180 degrees
                    else if (degrees <= 180)
                    {
                        rotatedTileShape.shape[(int)rotatedTileShapeSize.x - 1 - x, (int)rotatedTileShapeSize.y - 1 - y] = shape.shape[x, y];
                    }
                    //If the shape is being rotated by 270 degrees
                    else if (degrees <= 270)
                    {
                        rotatedTileShape.shape[y, (int)rotatedTileShapeSize.y - 1 - x] = shape.shape[x, y];
                    }
                }
            }

            return rotatedTileShape;
        }

        private void HandleNeighborFill(int xIndex, int yIndex, int fillIndex, TileShape shape, bool isBaseTile = false)
        {
            //If this is not the base tile, and it is already a valid tile
            if (!isBaseTile && shape.shape[xIndex, yIndex] != Glob.GetInstance().InvalidTileIndex)
            {
                return;
            }

            bool aboveNeighborIsNull = IsNullTile(xIndex, yIndex + 1, shape);
            bool belowNeighborIsNull = IsNullTile(xIndex, yIndex - 1, shape);
            bool leftNeighborIsNull = IsNullTile(xIndex - 1, yIndex, shape);
            bool rightNeighborIsNull = IsNullTile(xIndex + 1, yIndex, shape);

            //If this is the base tile
            if (isBaseTile)
            {
                //Let all the neighbors who are still null check if they should fill
                if (aboveNeighborIsNull)
                {
                    HandleNeighborFill(xIndex, yIndex + 1, fillIndex, shape);
                }
                if (belowNeighborIsNull)
                {
                    HandleNeighborFill(xIndex, yIndex - 1, fillIndex, shape);
                }
                if (leftNeighborIsNull)
                {
                    HandleNeighborFill(xIndex - 1, yIndex, fillIndex, shape);
                }
                if (rightNeighborIsNull)
                {
                    HandleNeighborFill(xIndex + 1, yIndex, fillIndex, shape);
                }
            }
            else if (!aboveNeighborIsNull && !belowNeighborIsNull && !leftNeighborIsNull && !rightNeighborIsNull)
            {
                //No null tile neighbors were detected, so we will fill this tile.
                shape.shape[xIndex, yIndex] = fillIndex;
            }
        }

        private bool IsNullTile(int xIndex, int yIndex, TileShape shape)
        {
            //If the neighbor is a valid tile
            if (xIndex >= 0 && xIndex < shape.shape.GetLength(0) &&
                yIndex >= 0 && yIndex < shape.shape.GetLength(1))
            {
                //If a neighboring tile is a null tile
                if (shape.shape[xIndex, yIndex] == Glob.GetInstance().InvalidTileIndex)
                {
                    return true;
                }
            }

            return false;
        }
        //========== Port data passing ==========

    }
}
