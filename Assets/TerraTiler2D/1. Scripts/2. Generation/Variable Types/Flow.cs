using System;
using System.Collections.Generic;

namespace TerraTiler2D
{
    [Serializable]
    public struct Flow
    {
        public enum Direction
        {
            Forwards,
            Backwards
        }

        public Flow(Direction direction)
        {
            this.direction = direction;
        }

        public Direction direction;

        //public List<Port_Abstract> ports;
    }
}
