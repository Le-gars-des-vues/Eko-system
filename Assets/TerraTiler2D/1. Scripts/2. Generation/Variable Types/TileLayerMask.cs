using System;
using UnityEngine;

namespace TerraTiler2D
{
    public class TileLayerMask : TileLayer, ICloneable
    {
        public TileLayerMask(Vector2 layerSize, Vector2 tileSize) : base(layerSize, tileSize)
        {

        }

        public TileLayerMask(TileLayer other) : base(other)
        {

        }

        new public object Clone()
        {
            return new TileLayerMask(this);
        }
    }
}
