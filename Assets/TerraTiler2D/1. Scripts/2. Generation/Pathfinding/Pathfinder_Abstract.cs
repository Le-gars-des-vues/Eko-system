using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public abstract class Pathfinder_Abstract
    {
        protected enum TileDirection
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3,
            End = 4,

            UpRight = 5,
            DownRight = 6,
            UpLeft = 7,
            DownLeft = 8,
        }

        protected TileLayer tileLayer;

        public Pathfinder_Abstract(TileLayer tileLayer)
        {
            this.tileLayer = tileLayer;
        }

        protected virtual Vector2Int[] GetClosestShapeTiles(List<Vector2Int> startShape, List<Vector2Int> endShape)
        {
            Vector2 direction = GetDirectionToShape(startShape, endShape);

            Vector2Int startShapeClosestTile = startShape[0];
            Vector2Int endShapeClosestTile = endShape[0];

            for (int i = 1; i < startShape.Count; i++)
            {
                if (Vector2.Dot(startShape[i], direction) > Vector2.Dot(startShapeClosestTile, direction))
                {
                    
                    startShapeClosestTile = startShape[i];
                }
            }

            for (int i = 1; i < endShape.Count; i++)
            {
                if (Vector2.Dot(endShape[i], direction) < Vector2.Dot(endShapeClosestTile, direction))
                {
                    endShapeClosestTile = endShape[i];
                }
            }

            return new Vector2Int[] { startShapeClosestTile, endShapeClosestTile };
        }

        private Vector2 GetDirectionToShape(List<Vector2Int> startShape, List<Vector2Int> endShape)
        {
            Vector2 direction = new Vector2(0, 0);

            //TODO: Don't use the first tile to calculate the direction, try to find the average of all tiles.
            direction = endShape[0] - startShape[0];

            return direction.normalized;
        }

        public abstract List<Vector2Int> GetPath(List<Vector2Int> startShape, List<Vector2Int> endShape);
    }
}
