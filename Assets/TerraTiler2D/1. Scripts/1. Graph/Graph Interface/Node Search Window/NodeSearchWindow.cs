#if (UNITY_EDITOR)
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class NodeSearchWindow
    {
        private const float searchWindowWidth = 250;
        private const float searchWindowHeight = 350;
        private const float searchWindowHeaderSize = 35;

        private VisualElement searchMenu;
        private ScrollView searchMenuContent;
        private TextField searchBar;

        private NodeSearchGroup mainGroup;

        public NodeSearchWindow()
        {

        }

        private VisualElement Init(SearchWindowContext context, Graph window)
        {
            CreateMenuContainer();
            AddMenuHeader();
            AddSearchBar();
            AddContentScroll();
            FillMenu(context);

            window.rootVisualElement.Add(searchMenu);
            searchMenu.visible = false;

            return searchMenu;
        }
        private void CreateMenuContainer()
        {
            searchMenu = new VisualElement();

            searchMenu.style.width = searchWindowWidth;
            searchMenu.style.height = searchWindowHeight;
            searchMenu.focusable = true;

            searchMenu.style.color = Glob.GetInstance().UITextColor;

            searchMenu.style.backgroundColor = Glob.GetInstance().UIContainerColor;

            searchMenu.style.borderTopLeftRadius = Glob.GetInstance().UIContainerCornerRadius;
            searchMenu.style.borderTopRightRadius = Glob.GetInstance().UIContainerCornerRadius;
            searchMenu.style.borderBottomLeftRadius = Glob.GetInstance().UIContainerCornerRadius;
            searchMenu.style.borderBottomRightRadius = Glob.GetInstance().UIContainerCornerRadius;

            searchMenu.style.borderLeftColor = Glob.GetInstance().UIContainerDepthBorderColorSide;
            searchMenu.style.borderRightColor = Glob.GetInstance().UIContainerDepthBorderColorSide;
            searchMenu.style.borderTopColor = Glob.GetInstance().UIContainerDepthBorderColorTop;
            searchMenu.style.borderBottomColor = Glob.GetInstance().UIContainerDepthBorderColorBottom;

            searchMenu.style.borderLeftWidth = Glob.GetInstance().UIContainerDepthBorderWidth;
            searchMenu.style.borderRightWidth = Glob.GetInstance().UIContainerDepthBorderWidth;
            searchMenu.style.borderTopWidth = Glob.GetInstance().UIContainerDepthBorderWidth;
            searchMenu.style.borderBottomWidth = Glob.GetInstance().UIContainerDepthBorderWidth;

            searchMenu.style.paddingLeft = Glob.GetInstance().UIContainerOuterPadding;
            searchMenu.style.paddingRight = Glob.GetInstance().UIContainerOuterPadding;
            searchMenu.style.paddingBottom = Glob.GetInstance().UIContainerOuterPadding;

            //Close the menu when the user clicks outside the element
            searchMenu.RegisterCallback<BlurEvent>(evt => { handleLostFocus(evt); });
        }
        private void AddMenuHeader()
        {
            TextElement menuHeader = new TextElement();
            menuHeader.text = "Create node";
            menuHeader.style.backgroundColor = Glob.GetInstance().UIContainerColor;
            menuHeader.style.unityTextAlign = TextAnchor.MiddleCenter;
            menuHeader.style.height = searchWindowHeaderSize;

            menuHeader.style.color = Glob.GetInstance().UITextColor;

            menuHeader.style.borderTopLeftRadius = Glob.GetInstance().UIContainerCornerRadius;
            menuHeader.style.borderTopRightRadius = Glob.GetInstance().UIContainerCornerRadius;

            searchMenu.Add(menuHeader);
        }
        private void AddSearchBar()
        {
            searchBar = new TextField("Search");
            searchBar.style.unityTextAlign = TextAnchor.MiddleLeft;
            searchBar.SetValueWithoutNotify("Search");

            searchBar.style.paddingLeft = Glob.GetInstance().UIContainerOuterPadding;
            searchBar.style.paddingRight = Glob.GetInstance().UIContainerOuterPadding;
            searchBar.style.paddingBottom = Glob.GetInstance().UIContainerOuterPadding;

            //Disable the label
            searchBar.labelElement.visible = false;
            searchBar.labelElement.focusable = false;
            searchBar.labelElement.pickingMode = PickingMode.Ignore;

            //Do a search when the value changes
            searchBar.RegisterValueChangedCallback(handleSearch);

            //Find the text input field
            var searchBarChildren = searchBar.Children().GetEnumerator();
            while (searchBarChildren.MoveNext())
            {
                if (searchBarChildren.Current != searchBar.labelElement)
                {
                    //Stretch the input field to the entire width
                    searchBarChildren.Current.StretchToParentWidth();
                    //Select all text in the field upon selection
                    searchBarChildren.Current.RegisterCallback<FocusInEvent>(evt => { searchBar.SelectAll(); handleSearch(searchBar.value); });
                    break;
                }
            }

            //searchBar.style.color = Glob.GetInstance().UITextColor;

            //searchBar.style.borderTopLeftRadius = Glob.GetInstance().UIContainerCornerRadius;
            //searchBar.style.borderTopRightRadius = Glob.GetInstance().UIContainerCornerRadius;
            //searchBar.style.borderBottomLeftRadius = Glob.GetInstance().UIContainerCornerRadius;
            //searchBar.style.borderBottomRightRadius = Glob.GetInstance().UIContainerCornerRadius;

            //Close the menu when the user clicks outside the element
            searchBar.RegisterCallback<BlurEvent>(evt => { handleLostFocus(evt); });

            searchMenu.Add(searchBar);
        }
        private void AddContentScroll()
        {
            searchMenuContent = new ScrollView(ScrollViewMode.Vertical);
            searchMenuContent.style.backgroundColor = Glob.GetInstance().UISubContainerColor;
            searchMenuContent.touchScrollBehavior = ScrollView.TouchScrollBehavior.Clamped;

            searchMenuContent.style.borderTopLeftRadius = Glob.GetInstance().UIContainerCornerRadius;
            searchMenuContent.style.borderTopRightRadius = Glob.GetInstance().UIContainerCornerRadius;
            searchMenuContent.style.borderBottomLeftRadius = Glob.GetInstance().UIContainerCornerRadius;
            searchMenuContent.style.borderBottomRightRadius = Glob.GetInstance().UIContainerCornerRadius;

            searchMenuContent.style.borderLeftColor = Glob.GetInstance().UIContainerDepthBorderColorSide;
            searchMenuContent.style.borderRightColor = Glob.GetInstance().UIContainerDepthBorderColorSide;
            searchMenuContent.style.borderTopColor = Glob.GetInstance().UIContainerDepthBorderColorBottom;
            searchMenuContent.style.borderBottomColor = Glob.GetInstance().UIContainerDepthBorderColorTop;

            searchMenuContent.style.borderLeftWidth = Glob.GetInstance().UIContainerDepthBorderWidth;
            searchMenuContent.style.borderRightWidth = Glob.GetInstance().UIContainerDepthBorderWidth;
            searchMenuContent.style.borderTopWidth = Glob.GetInstance().UIContainerDepthBorderWidth;
            searchMenuContent.style.borderBottomWidth = Glob.GetInstance().UIContainerDepthBorderWidth;

            searchMenuContent.style.paddingLeft = Glob.GetInstance().UIContainerOuterPadding;
            searchMenuContent.style.paddingRight = Glob.GetInstance().UIContainerOuterPadding;
            searchMenuContent.style.paddingTop = Glob.GetInstance().UIContainerOuterPadding;
            searchMenuContent.style.paddingBottom = Glob.GetInstance().UIContainerOuterPadding;

            searchMenuContent.style.height = searchWindowHeight;

            searchMenuContent.verticalScroller.focusable = false;
            searchMenuContent.verticalScroller.pickingMode = PickingMode.Ignore;
            searchMenuContent.verticalScroller.slider.focusable = false;
            searchMenuContent.verticalScroller.slider.pickingMode = PickingMode.Ignore;

            searchMenu.Add(searchMenuContent);
        }
        private void FillMenu(SearchWindowContext context)
        {
            mainGroup = new NodeSearchGroup("Main group", this, null, false);
            var allNodeTypes = Glob.GetInstance().GetAllNodeClasses(false, "", new Vector2(0, 0), "").GetEnumerator();
            while (allNodeTypes.MoveNext())
            {
                //Create a node button for each node type, and add them to the correct group.
                NodeSearchEntry entry = CreateSearchMenuEntry(allNodeTypes.Current, context);

                if (entry != null)
                {
                    //Get the layers of groups this node button is a part of
                    GetSearchMenuGroup(entry.GetGroup(), mainGroup).AddElement(entry);
                }
            }
        }

        public void Open(SearchWindowContext context, Graph window)
        {
            if (searchMenu == null)
            {
                searchMenu = Init(context, window);
            }

            ShowNodeSearchGroup(mainGroup);
            updatePosition(context);

            searchMenu.Focus();
        }
        public void Close()
        {
            if (searchMenu != null)
            {
                searchMenu.visible = false;
                searchMenuContent.Clear();
            }
        }

        private void handleLostFocus(BlurEvent evt)
        {
            //If the focus changed to a different object than the search menu, or the search bar, close the search menu.
            if (evt.relatedTarget != searchMenu)
            {
                var searchBarChildren = searchBar.Children().GetEnumerator();
                while (searchBarChildren.MoveNext())
                {
                    if (evt.relatedTarget == searchBarChildren.Current)
                    {
                        return;
                    }
                }
                Close();
            }
        }

        public TextField GetSearchBar()
        {
            return searchBar;
        }
        private void handleSearch(ChangeEvent<string> evt)
        {
            handleSearch(evt.newValue);
        }
        private void handleSearch(string search)
        {
            //Show the main group if the search field is empty
            if (string.IsNullOrEmpty(search))
            {
                ShowNodeSearchGroup(mainGroup);
                return;
            }

            //Make the search case insensitive.
            search = search.ToLower();
            List<VisualElement> allElements = getAllElements();

            //If this search contains any search filters
            if (search.Split(new char[1] { ':' }).Length > 1)
            {
                //Get all the search filters
                string[] searchSplit = search.Split(new char[1]{':'}, System.StringSplitOptions.RemoveEmptyEntries);
                //For every search filter
                for (int i = 0; i < searchSplit.Length; i+=2)
                {
                    //If there were any parameters passed along with the filter
                    if (i+1 < searchSplit.Length)
                    {
                        //Get all the parameters
                        string[] searchEntries = searchSplit[i + 1].Split(new char[2] { ',', ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

                        //For every parameter
                        for (int j = 0; j < searchEntries.Length; j++)
                        {
                            //NOTE: Ugly way to allow users to write float instead of single
                            if (searchEntries[j] == "float")
                            {
                                searchEntries[j] = "single";
                            }

                            //If there is an 'input' filter applied to the search
                            if (searchSplit[i] == "input" || searchSplit[i] == "in")
                            {
                                //Throw out any element that does not adhere to the filter
                                allElements = allElements.Where(x => x.GetType() == typeof(NodeSearchEntry) && ((NodeSearchEntry)x).GetInputTooltip().ToLower().Contains(searchEntries[j])).ToList();
                            }
                            //If there is an 'output' filter applied to the search
                            else if (searchSplit[i] == "output" || searchSplit[i] == "out")
                            {
                                //Throw out any element that does not adhere to the filter
                                allElements = allElements.Where(x => x.GetType() == typeof(NodeSearchEntry) && ((NodeSearchEntry)x).GetOutputTooltip().ToLower().Contains(searchEntries[j])).ToList();
                            }
                            //If there is a 'description' filter applied to the search
                            else if (searchSplit[i] == "description" || searchSplit[i] == "des")
                            {
                                //Throw out any element that does not adhere to the filter
                                allElements = allElements.Where(x => x.GetType() == typeof(NodeSearchEntry) && ((NodeSearchEntry)x).GetDescriptionTooltip().ToLower().Contains(searchEntries[j])).ToList();
                            }
                            //If there is a 'name' filter applied to the search
                            else if (searchSplit[i] == "name" || searchSplit[i] == "title")
                            {
                                //Throw out any element that does not adhere to the filter
                                allElements = allElements.Where(x => x.name.ToLower().Contains(searchEntries[j])).ToList();
                            }
                        }
                    }
                }

                //Show only the elements that adhere to all the search filters
                ShowElements(allElements);
            }
            else
            {
                //If there are no search filters, simply show all elements whose name or tooltip contain the search string.
                List<VisualElement> returnElements = allElements.Where(x => x.name.ToLower().Contains(search) || x.tooltip.ToLower().Contains(search)).ToList();
                ShowElements(returnElements);
            }
        }
        private List<VisualElement> getAllElements()
        {
            return mainGroup.GetElements(true);
        }

        //Iteratively creates and gets SearchMenuGroups
        private NodeSearchGroup GetSearchMenuGroup(string[] group, NodeSearchGroup currentGroup = null)
        {
            //If this entry is not part of a group, return the searchMenu directly
            if (group == null || group.Length <= 0)
            {
                return mainGroup;
            }

            //Get the first group name
            string name = group[0];

            //Get an array that contains the groups up to the depth of the current group.
            string[] currentGroupArray = new string[0];
            List<string> currentGroupList = group.ToList();
            //If there is a current group
            if (currentGroup != null)
            {
                //Go over the entire group array
                for (int i = 0; i < currentGroupList.Count; i++)
                {
                    //Find the index of the current group within the group array
                    if (currentGroupList[i] == currentGroup.name)
                    {
                        //If there is a sub-group of the current group
                        if (currentGroupList.Count > i + 1)
                        {
                            //Get the name of the sub-group
                            name = currentGroupList[i + 1];

                            //Remove any excess group references from the group array, since the sub-group should only know the group array up to the current group.
                            currentGroupList.RemoveRange(i + 1, currentGroupList.Count - (i + 1));
                            currentGroupArray = currentGroupList.ToArray();
                        }
                        else
                        {
                            //There are no more groups to be made, so return the current group.
                            return currentGroup;
                        }

                        break;
                    }
                }
            }
            else
            {
                //If there is no current group, start at the beginning of the array.
                name = group[0];
                currentGroupArray = new string[0];

                //Set the current group to the main group
                currentGroup = mainGroup;
            }

            //Get an empty reference to store the return value
            NodeSearchGroup subGroup;

            //If the current group already contains a sub-group with the name
            if (currentGroup.GetElements().Any(x => x.name == name && x.GetType() == typeof(NodeSearchGroup)))
            {
                //Fetch the existing group
                subGroup = (NodeSearchGroup)currentGroup.GetElements().First(x => x.name == name && x.GetType() == typeof(NodeSearchGroup));
            }
            else
            {
                //If there is no group with this name, create a new group, pass in the truncated group array we created at the start.
                subGroup = new NodeSearchGroup(name, this, currentGroupArray);

                //Add the sub-group to the current group
                currentGroup.AddElement(subGroup);
            }

            //Get the next sub-group down
            return GetSearchMenuGroup(group, subGroup);
        }
        //Fetches the NodeSearchEntry from a node, and tells the SearchWindow to close whenever the entry is clicked.
        private NodeSearchEntry CreateSearchMenuEntry(Node node, SearchWindowContext context)
        {
            NodeSearchEntry entry = node.GetSearchMenuEntry(context);
            if (entry != null)
            {
                entry.clicked += Close;
            }

            return entry;
        }

        public void ShowNodeSearchGroup(string[] group)
        {
            ShowNodeSearchGroup(GetSearchMenuGroup(group));
        }
        public void ShowNodeSearchGroup(NodeSearchGroup group)
        {
            ShowElements(group.GetElements());
        }
        private void ShowElements(List<VisualElement> elements)
        {
            searchMenuContent.Clear();

            foreach (VisualElement element in elements)
            {
                searchMenuContent.Add(element);
            }

            searchMenu.visible = true;
        }

        private void updatePosition(SearchWindowContext context)
        {
            var viewOffsetPosition = context.screenMousePosition + (Vector2)Graph.Instance.GetGraphView().viewTransform.position - Graph.Instance.position.position;
            var worldMousePosition = Graph.Instance.rootVisualElement.ChangeCoordinatesTo(Graph.Instance.rootVisualElement.parent, viewOffsetPosition);
            var localMousePosition = Graph.Instance.GetGraphView().contentViewContainer.WorldToLocal(worldMousePosition);
            //NOTE: Without this offset the menu is placed 40 pixels below where the mouse button was pressed.
            var menuOffset = new Vector2(0, -40);

            searchMenu.transform.position = (localMousePosition * Graph.Instance.GetGraphView().scale) + menuOffset;

            var entries = getAllElements().Where(x => x.GetType() == typeof(NodeSearchEntry)).GetEnumerator();
            while (entries.MoveNext())
            {
                ((NodeSearchEntry)entries.Current).SetContext(context);
            }           
        }
    }
}
#endif