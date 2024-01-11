using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class TilePlacerRectangles_Node : TilePlacerShape_Node
    {
        private PortWithField<Vector2> shapeSizePort;

        //========== Initialization ==========

        public TilePlacerRectangles_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TilePlacerRectangles;
            SetTooltip("Places rectangles in a tile layer.");
            searchMenuEntry = new string[] { "Flow" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            shapeSizePort = GeneratePortWithField<Vector2>("Size", PortDirection.Input, new Vector2(10, 10), "ShapeSize", PortCapacity.Single, false, "What size should the rectangles be.");
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
            Vector2 size = (Vector2)shapeSizePort.GetPortVariable();

            TileShape rectangle = new TileShape(size, position);

            for (int x = 0; x < (int)size.x; x++)
            {
                for (int y = 0; y < (int)size.y; y++)
                {
                    rectangle.shape[x, y] = tileIndex;
                }
            }
            
            return rectangle;
        }

        //========== Port data passing ==========

    }
}
