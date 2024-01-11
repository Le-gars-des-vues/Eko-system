using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class TilePlacerTriangles_Node : TilePlacerShape_Node
    {
        private PortWithField<Vector3> shapeSizePort;

        //========== Initialization ==========

        public TilePlacerTriangles_Node(string nodeName, Vector2 position, string guid = null) : base(nodeName, position, guid)
        {
            nodeType = Glob.NodeTypes.TilePlacerTriangles;
            SetTooltip("Places triangles in a tile layer.");
            searchMenuEntry = new string[] { "Flow" };
        }

        protected override void InitializeInputPorts()
        {
            base.InitializeInputPorts();

            shapeSizePort = GeneratePortWithField<Vector3>("Edge length", PortDirection.Input, new Vector3(5, 5, 5), "ShapeSize", PortCapacity.Single, false, "What should the lengths of each side of the triangle be.");
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
            Vector3 edgeLength = (Vector3)shapeSizePort.GetPortVariable();

            float lengthA = Mathf.Floor(edgeLength.x);
            float lengthB = Mathf.Floor(edgeLength.y);
            float lengthC = Mathf.Floor(edgeLength.z);

            if (lengthB + lengthC < lengthA)
            {
                Glob.GetInstance().DebugString("Node '" + GetTitle() + "' has invalid triangle lengths. Side X should be shorter than side Y and side Z combined. Attempting to rearrange the sides to fix the issue. The resulting triangles might look different from what you intended.", Glob.DebugCategories.Error, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
            }
            while (lengthB + lengthC < lengthA)
            {
                //The base side needs to be shorter than the other two combined to make a triangle.
                lengthA = lengthB;
                lengthB = lengthC;
                lengthC = lengthA;
            }
            if (lengthB + lengthC == lengthA)
            {
                Glob.GetInstance().DebugString("Failed to rearrange the sides. The best that could be done resulted in (Side Y + Side Z = Side X), but Side X should be shorter instead of equal.", Glob.DebugCategories.Error, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                return null;
            }

            //Always bottom left
            Vector2 positionC = new Vector2(0, 0);
            //Always at the end of side A
            Vector2 positionB = new Vector2(lengthA, 0);
            //Calculate the last point
            Vector2 positionA = getThirdPointOfTriangle(lengthA, lengthB, lengthC);

            //Calculate the width and the height of the 2D array
            float height = Mathf.Floor(Mathf.Max(Mathf.Ceil(positionA.y), Mathf.Ceil(positionB.y), Mathf.Ceil(positionC.y)));
            float width = Mathf.Floor(Mathf.Max(Mathf.Ceil(positionA.x), Mathf.Ceil(positionB.x), Mathf.Ceil(positionC.x)));

            //Offset the corner positions of the triangle to compensate for the array index starting at 0
            positionA.x -= 0.5f;
            positionB.x -= 1;

            TileShape triangle = new TileShape(new Vector2(width, height), position);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (ContainedInTriangle(new Vector2(x, y), positionA, positionB, positionC))
                    {
                        triangle.shape[x, y] = tileIndex;
                    }
                }
            }

            return triangle;
        }

        //a,b,c are the sides of the triangle
        private Vector2 getThirdPointOfTriangle(float a, float b, float c)
        {
            Vector2 result = new Vector2(0, 0);

            if (a > 0) {
                result.x = (c* c - b* b + a* a) / (2*a);
            }

            result.y = Mathf.Sqrt(c* c - result.x* result.x);
            return result;
        }

        private float sign(Vector2 p1, Vector2 p2, Vector2 p3)
        {
            return (p1.x - p3.x) * (p2.y - p3.y) - (p2.x - p3.x) * (p1.y - p3.y);
        }

        private bool ContainedInTriangle(Vector2 pt, Vector2 v1, Vector2 v2, Vector2 v3)
        {
            float d1, d2, d3;
            bool has_neg, has_pos;

            d1 = sign(pt, v1, v2);
            d2 = sign(pt, v2, v3);
            d3 = sign(pt, v3, v1);

            has_neg = (d1 <= 0) || (d2 <= 0) || (d3 <= 0);
            has_pos = (d1 > 0) || (d2 > 0) || (d3 > 0);

            return !(has_neg && has_pos);
        }

        //========== Port data passing ==========

    }
}
