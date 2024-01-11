#if (UNITY_EDITOR)
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class NodeSearchEntry : Button
    {
        private string[] group;
        private Glob.NodeTypes nodeType;
        private SearchWindowContext context;

        private string descriptionTooltip;
        private string inputTooltip;
        private string outputTooltip;

        public NodeSearchEntry(string name, string[] tooltip, Glob.NodeTypes nodeType, SearchWindowContext context, string[] group = null)
        {
            this.name = name;
            this.text = name;
            //this.tooltip = tooltip;
            this.group = group;
            this.nodeType = nodeType;
            this.context = context;

            this.style.minHeight = 20;

            AddTooltipComponent(tooltip);

            this.style.unityTextAlign = TextAnchor.MiddleLeft;

            this.clicked += OnSelectEntry;
        }
        private void AddTooltipComponent(string[] tooltip)
        {
            Image tooltipComponent = new Image();

            tooltipComponent.tooltip = ConstructTooltip(tooltip);
            descriptionTooltip = tooltip[0];
            inputTooltip = tooltip[1];
            outputTooltip = tooltip[2];

            tooltipComponent.image = Glob.GetInstance().SearchEntryTooltipIcon;
            tooltipComponent.style.width = 12;
            tooltipComponent.style.height = 12;
            tooltipComponent.style.alignSelf = Align.FlexEnd;
            tooltipComponent.style.marginTop = 3;

            this.Add(tooltipComponent);
        }
        private string ConstructTooltip(string[] tooltipSegments)
        {
            string tooltip = tooltipSegments[0];
            if (!string.IsNullOrEmpty(tooltipSegments[0]))
            {
                tooltip += "\n\n";
            }
            tooltip += tooltipSegments[1];
            tooltip += "\n\n";
            tooltip += tooltipSegments[2];

            return tooltip;
        }

        public void SetContext(SearchWindowContext context)
        {
            this.context = context;
        }

        public void OnSelectEntry()
        {
            //Get the mouse local position
            var viewOffsetPosition = context.screenMousePosition - Graph.Instance.position.position;
            var worldMousePosition = Graph.Instance.rootVisualElement.ChangeCoordinatesTo(Graph.Instance.rootVisualElement.parent, viewOffsetPosition);
            var localMousePosition = Graph.Instance.GetGraphView().contentViewContainer.WorldToLocal(worldMousePosition);
            //NOTE: Without this offset the node is placed 20 pixels below where the mouse button was pressed.
            var nodeOffset = new Vector2(0, -20);

            Graph.Instance.GetGraphView().CreateNode(nodeType, localMousePosition + nodeOffset);
        }

        public string[] GetGroup()
        {
            return group;
        }
        public string GetDescriptionTooltip()
        {
            return descriptionTooltip;
        }
        public string GetInputTooltip()
        {
            return inputTooltip;
        }
        public string GetOutputTooltip()
        {
            return outputTooltip;
        }
    }
}
#endif
