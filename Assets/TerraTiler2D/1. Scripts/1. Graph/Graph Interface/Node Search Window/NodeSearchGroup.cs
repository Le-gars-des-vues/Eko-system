#if (UNITY_EDITOR)
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class NodeSearchGroup : Button
    {
        public string[] group;
        private NodeSearchWindow searchWindow;

        private List<VisualElement> groupElements = new List<VisualElement>();

        public NodeSearchGroup(string name, NodeSearchWindow searchWindow, string[] group = null, bool includeBackButton = true)
        {
            this.name = name;
            this.text = "     " + name;
            this.group = group;
            this.searchWindow = searchWindow;

            this.style.minHeight = 20;
            this.style.alignItems = Align.FlexStart;
            this.style.unityTextAlign = TextAnchor.MiddleLeft;

            Image groupIcon = new Image();
            groupIcon.image = Glob.GetInstance().NodeSearchGroupIcon;
            groupIcon.style.height = 10;
            groupIcon.style.width = 10;
            groupIcon.style.marginTop = 3;
            this.Add(groupIcon);

            if (includeBackButton)
            {
                Button backButton = new Button();
                //backButton.text = "     Back";
                //backButton.style.unityTextAlign = TextAnchor.MiddleLeft;
                backButton.style.minHeight = 20;
                backButton.style.alignItems = Align.FlexStart;

                Image backIcon = new Image();
                backIcon.image = Glob.GetInstance().NodeSearchGroupBackIcon;
                backIcon.style.height = 10;
                backIcon.style.width = 10;
                backIcon.style.marginTop = 3;
                backButton.Add(backIcon);

                backButton.clicked += ShowPreviousGroup;
                AddElement(backButton);
            }

            this.clicked += OnSelectGroup;
        }

        private void OnSelectGroup()
        {
            searchWindow.ShowNodeSearchGroup(this);
        }
        private void ShowPreviousGroup()
        {
            searchWindow.ShowNodeSearchGroup(group);
        } 

        public List<VisualElement> GetElements(bool trickleDown = false)
        {
            if (trickleDown)
            {
                List<VisualElement> elements = new List<VisualElement>();
                elements.AddRange(groupElements);
                foreach (VisualElement subGroup in groupElements)
                {
                    if (subGroup.GetType() == typeof(NodeSearchGroup))
                    {
                        elements.AddRange(((NodeSearchGroup)subGroup).GetElements(trickleDown));
                    }
                }

                return elements;
            }

            return groupElements;
        }

        public void AddElement(VisualElement element)
        {
            groupElements.Add(element);
        }
    }
}
#endif