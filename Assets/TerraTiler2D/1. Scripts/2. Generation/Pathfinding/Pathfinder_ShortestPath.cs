using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class Pathfinder_ShortestPath : Pathfinder_Abstract
    {
        private bool allowDiagonal;
        private int tunnelRandomization;

        public Pathfinder_ShortestPath(TileLayer tileLayer, bool allowDiagonal, int tunnelRandomization) : base(tileLayer)
        {
            this.allowDiagonal = allowDiagonal;
            this.tunnelRandomization = tunnelRandomization;
        }

        public override List<Vector2Int> GetPath(List<Vector2Int> startShape, List<Vector2Int> endShape)
        {
            Vector2Int[] closestTiles = GetClosestShapeTiles(startShape, endShape);

            Vector2Int startPosition = closestTiles[0];
            Vector2Int endPosition = closestTiles[1];

            List<Vector2Int> path = new List<Vector2Int>();
            path.Add(startPosition);

            Vector2Int previousTile = startPosition;

            TileDirection pathDirection = TileDirection.End;

            //As long as we have not reached the end position
            while (previousTile != endPosition)
            {
                pathDirection = handleDirection(pathDirection, previousTile, endPosition);

                if (pathDirection != TileDirection.End)
                {
                    Vector2Int newtile = AddTileToPath(previousTile, pathDirection);
                    path.Add(newtile);

                    previousTile = newtile;

                    if (pathDirection > TileDirection.End)
                    {
                        Vector2Int diagonaltile = AddTileToPath(previousTile, pathDirection, true);
                        path.Add(diagonaltile);

                        previousTile = diagonaltile;
                    }

                    //If the path direction should be randomized, and we have not arrive at the end position yet
                    if (tunnelRandomization > 0)
                    {
                        Vector2Int randomizedTile = RandomizePath(previousTile, pathDirection);

                        if (randomizedTile.x != Glob.GetInstance().InvalidTileIndex)
                        {
                            path.Add(randomizedTile);

                            previousTile = randomizedTile;
                        }
                    }
                }
            }

            return path;
        }

        private TileDirection handleDirection(TileDirection currentDirection, Vector2Int previousTile, Vector2Int endPosition)
        {
            if (previousTile == endPosition)
            {
                return TileDirection.End;
            }

            //Calculate the x and y distance to the end position
            int xDist = endPosition.x - previousTile.x;
            int yDist = endPosition.y - previousTile.y;

            //If there is no current direction
            if (currentDirection == TileDirection.End)
            {
                //If diagonals are not allowed
                if (!allowDiagonal)
                {
                    //Move along the axis with the shortest distance to the end position, and do the other axis afterwards
                    if ((Mathf.Abs(xDist) <= Mathf.Abs(yDist) && xDist != 0) || yDist == 0 && xDist != 0)
                    {
                        if (Mathf.Sign(xDist) > 0)
                        {
                            return TileDirection.Right;
                        }
                        else
                        {
                            return TileDirection.Left;
                        }
                    }
                    else if (yDist != 0)
                    {
                        if (Mathf.Sign(yDist) > 0)
                        {
                            return TileDirection.Up;
                        }
                        else
                        {
                            return TileDirection.Down;
                        }
                    }
                    else
                    {
                        return TileDirection.End;
                    }
                }
                //If diagonals are allowed
                else
                {
                    if (xDist != 0 && yDist != 0)
                    {
                        if (xDist > 0 && yDist > 0)
                        {
                            return TileDirection.UpRight;
                        }
                        else if (xDist > 0 && yDist < 0)
                        {
                            return TileDirection.DownRight;
                        }
                        else if (xDist < 0 && yDist > 0)
                        {
                            return TileDirection.UpLeft;
                        }
                        else if (xDist < 0 && yDist < 0)
                        {
                            return TileDirection.DownLeft;
                        }
                        else
                        {
                            return TileDirection.End;
                        }
                    }
                    else if (xDist > 0)
                    {
                        return TileDirection.Right;
                    }
                    else if (xDist < 0)
                    {
                        return TileDirection.Left;
                    }
                    else if (yDist > 0)
                    {
                        return TileDirection.Up;
                    }
                    else if (yDist < 0)
                    {
                        return TileDirection.Down;
                    }
                    else
                    {
                        return TileDirection.End;
                    }
                }
            }
            else if (currentDirection == TileDirection.Right)
            {
                if (xDist > 0)
                {
                    return TileDirection.Right;
                }
                else
                {
                    return handleDirection(TileDirection.End, previousTile, endPosition);
                }
            }
            else if (currentDirection == TileDirection.Left)
            {
                if (xDist < 0)
                {
                    return TileDirection.Left;
                }
                else
                {
                    return handleDirection(TileDirection.End, previousTile, endPosition);
                }
            }
            else if (currentDirection == TileDirection.Up)
            {
                if (yDist > 0)
                {
                    return TileDirection.Up;
                }
                else
                {
                    return handleDirection(TileDirection.End, previousTile, endPosition);
                }
            }
            else if (currentDirection == TileDirection.Down)
            {
                if (yDist < 0)
                {
                    return TileDirection.Down;
                }
                else
                {
                    return handleDirection(TileDirection.End, previousTile, endPosition);
                }
            }
            else if (currentDirection == TileDirection.UpRight)
            {
                if (xDist > 0 && yDist > 0)
                {
                    return TileDirection.UpRight;
                }
                else
                {
                    return handleDirection(TileDirection.End, previousTile, endPosition);
                }
            }
            else if (currentDirection == TileDirection.DownRight)
            {
                if (xDist > 0 && yDist < 0)
                {
                    return TileDirection.DownRight;
                }
                else
                {
                    return handleDirection(TileDirection.End, previousTile, endPosition);
                }
            }
            else if (currentDirection == TileDirection.UpLeft)
            {
                if (xDist < 0 && yDist > 0)
                {
                    return TileDirection.UpLeft;
                }
                else
                {
                    return handleDirection(TileDirection.End, previousTile, endPosition);
                }
            }
            else if (currentDirection == TileDirection.DownLeft)
            {
                if (xDist < 0 && yDist < 0)
                {
                    return TileDirection.DownLeft;
                }
                else
                {
                    return handleDirection(TileDirection.End, previousTile, endPosition);
                }
            }
            else
            {
                return TileDirection.End;
            }
        }

        private Vector2Int AddTileToPath(Vector2Int previousTile, TileDirection tileDirection, bool diagonalToggle = false)
        {
            switch (tileDirection)
            {
                case TileDirection.Up:
                    return new Vector2Int(previousTile.x, previousTile.y + 1);
                case TileDirection.Left:
                    return new Vector2Int(previousTile.x - 1, previousTile.y);
                case TileDirection.Down:
                    return new Vector2Int(previousTile.x, previousTile.y - 1);
                case TileDirection.Right:
                    return new Vector2Int(previousTile.x + 1, previousTile.y);

                case TileDirection.UpRight:
                    if (diagonalToggle)
                    {
                        return new Vector2Int(previousTile.x, previousTile.y + 1);
                    }
                    else
                    {
                        return new Vector2Int(previousTile.x + 1, previousTile.y);
                    }
                case TileDirection.DownRight:
                    if (diagonalToggle)
                    {
                        return new Vector2Int(previousTile.x, previousTile.y - 1);
                    }
                    else
                    {
                        return new Vector2Int(previousTile.x + 1, previousTile.y);
                    }
                case TileDirection.UpLeft:
                    if (diagonalToggle)
                    {
                        return new Vector2Int(previousTile.x, previousTile.y + 1);
                    }
                    else
                    {
                        return new Vector2Int(previousTile.x - 1, previousTile.y);
                    }
                case TileDirection.DownLeft:
                    if (diagonalToggle)
                    {
                        return new Vector2Int(previousTile.x, previousTile.y - 1);
                    }
                    else
                    {
                        return new Vector2Int(previousTile.x - 1, previousTile.y);
                    }

                case TileDirection.End:
                    break;
                default:
                    break;
            }

            return new Vector2Int(Glob.GetInstance().InvalidTileIndex, Glob.GetInstance().InvalidTileIndex);
        }

        private Vector2Int RandomizePath(Vector2Int previousTile, TileDirection pathDirection)
        {
            //Should we make the path swerve a bit?
            if (Random.Range(0, 150) < tunnelRandomization)
            {
                int stepDirection = Random.Range(0, 2);
                if (stepDirection == 0)
                {
                    stepDirection = -1;
                }

                //If the path is traversing along the X axis
                if (pathDirection == TileDirection.Left || pathDirection == TileDirection.Right || pathDirection == TileDirection.UpRight || pathDirection == TileDirection.DownRight)
                {
                    //If the tile we want to add to the path is outside of the bounds.
                    if (previousTile.y + stepDirection >= tileLayer.generatedTiles.GetLength(1) ||
                        previousTile.y + stepDirection < 0)
                    {
                        return new Vector2Int(Glob.GetInstance().InvalidTileIndex, Glob.GetInstance().InvalidTileIndex);
                    }

                    //Update the previous tile
                    return new Vector2Int(previousTile.x, previousTile.y + stepDirection);
                }
                //If the path is traversing along the Y axis
                else if (pathDirection == TileDirection.Up || pathDirection == TileDirection.Down || pathDirection == TileDirection.UpLeft || pathDirection == TileDirection.DownLeft)
                {
                    //If the tile we want to add to the path is outside of the bounds.
                    if (previousTile.x + stepDirection >= tileLayer.generatedTiles.GetLength(0) ||
                        previousTile.x + stepDirection < 0)
                    {
                        return new Vector2Int(Glob.GetInstance().InvalidTileIndex, Glob.GetInstance().InvalidTileIndex);
                    }

                    //Update the previous tile
                    return new Vector2Int(previousTile.x + stepDirection, previousTile.y);
                }
            }

            return new Vector2Int(Glob.GetInstance().InvalidTileIndex, Glob.GetInstance().InvalidTileIndex);
        }
    }
}
