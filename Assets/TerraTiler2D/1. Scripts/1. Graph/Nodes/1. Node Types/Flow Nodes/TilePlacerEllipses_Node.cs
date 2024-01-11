using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class TilePlacerEllipses_Node : TilePlacerShape_Node
    {
        private PortWithField<Vector2> shapeSizePort;

        //========== Initialization ==========

        public TilePlacerEllipses_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TilePlacerEllipses;
            SetTooltip("Places ellipses in a tile layer.");
            searchMenuEntry = new string[] { "Flow" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            shapeSizePort = GeneratePortWithField<Vector2>("Size", PortDirection.Input, new Vector2(10, 10), "ShapeSize", PortCapacity.Single, false, "What size should the ellipses be.");
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

        protected override TileShape CreateShape(int tileIndex, Vector2 position)
        {
            //Get the size of the ellipse
            Vector2 size = (Vector2)shapeSizePort.GetPortVariable();

            //Floor the size
            size.x = Mathf.Floor(size.x);
            size.y = Mathf.Floor(size.y);

            //Create an empty TileShape with the required size
            TileShape ellipse = new TileShape(size, position);

            //For every tile in the TileShape
            for (int x = 0; x < size.x; x++)
            {
                for (int y = 0; y < size.y; y++)
                {
                    //Is the tile contained within the ellipse
                    if (ContainedInEllipse(size, new Vector2(x, y)))
                    {
                        //Place a tile
                        ellipse.shape[x, y] = tileIndex;
                    }
                }
            }

            return ellipse;
        }

        private bool ContainedInEllipse(Vector2 ellipseSize, Vector2 location)
        {
            Vector2 center = new Vector2(
                (ellipseSize.x * 0.5f) - 0.5f,
                (ellipseSize.y * 0.5f) - 0.5f);

            double _xRadius = ellipseSize.x * 0.5f;
            double _yRadius = ellipseSize.y * 0.5f;

            if (_xRadius <= 0.0 || _yRadius <= 0.0)
                return false;

            Vector2 normalized = new Vector2(location.x - center.x,
                                         location.y - center.y);

            return ((double)(normalized.x * normalized.x)
                     / (_xRadius * _xRadius)) + ((double)(normalized.y * normalized.y) / (_yRadius * _yRadius))
                <= 1.0;
        }

        //========== Port data passing ==========

    }
}
