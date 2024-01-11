#if (UNITY_EDITOR)
using UnityEngine;

namespace TerraTiler2D
{
    public class Minimap : UnityEditor.Experimental.GraphView.MiniMap
    {
        public Minimap(Graph graph, Vector2 minimapSize, Vector2 minimapSpacing) : base()
        {
            //Create a new minimap, and make it non-movable.
            anchored = true;
            //Align the minimap to the right side of the window.
            //TODO: Minimap aligns incorrectly upon loading another GraphData object.
            //TODO: Keep the minimap aligned correctly when the graph window gets resized.
            var minimapPosition = new Vector2(graph.position.width - minimapSize.x - minimapSpacing.x, minimapSpacing.y);

            //Position the minimap in the top right corner.
            SetPosition(new Rect(minimapPosition.x, minimapPosition.y, minimapSize.x, minimapSize.y));
            //Add the minimap to the graph.
            graph.GetGraphView().Add(this);
        }
    }
}
#endif
