#if (UNITY_EDITOR)
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class Graph : EditorWindow
    {
        public static Graph Instance { get; private set; }
        public static bool IsOpen
        {
            get { return Instance != null; }
        }

        //Add an option to the toolbar at the top to open this window.
        [MenuItem("Tools/TerraTiler2D/Open Graph window", priority = 0)]
        public static void OpenGraphWindow()
        {
            InitializeWindow();
        }

        private static bool InitializeWindow(string assetPath = "")
        {
            GraphData graphData = null;
            //If no assetPath was passed into this method
            if (string.IsNullOrEmpty(assetPath))
            {
                //Get the temp graph data.
                graphData = GraphSaveManager.GetInstance().GetTempGraphData();
            }
            else
            {
                //Get the graph data at assetPath.
                graphData = AssetDatabase.LoadAssetAtPath<GraphData>(assetPath);

                //If we were able to load graphData
                if (graphData != null)
                {
                    //Apply any changes to the file name.
                    //TODO: When the file name changes while the GraphData is loaded in this window, the name does not get updated until the graph is reloaded.
                    graphData.SetFileName(graphData.name);
                }
            }

            return InitializeWindow(graphData);
        }
        private static bool InitializeWindow(GraphData graphData)
        {
            //Get or create a Graph window, and dock it.
            var window = GetWindow<Graph>(GetDockWindow());

            if (Instance == null)
            {
                Instance = window;
            }

            //Set the title of the window.
            window.titleContent = new GUIContent(Glob.GetInstance().TerraTiler2DWindowName);

            //If we were able to load graphData
            if (graphData != null)
            {
                //Make sure the GraphData has the correct ToolVersion, matching the installed version of TerraTiler2D
                if (!GraphDataToolVersionManager.GetInstance().UpdateGraphData(graphData))
                {
                    Glob.GetInstance().DebugString("Could not load GraphData '" + graphData.GetFileName() + "'. \n" +
                                                    "GraphData is at version " + Glob.GetInstance().GetToolVersionAsString(graphData.ToolVersion) + ", while version " + Glob.GetInstance().GetToolVersionAsString(Glob.GetInstance().ToolVersion) + " of TerraTiler2D is installed.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    return false;
                }

                //Set the file name
                window.SetFileName(graphData.GetFileName());

                //Once the window is ready, we load the content of the graphData.
                window.RequestDataOperation(Graph.SaveOperation.Load, graphData);

                return true;
            }

            return false;
        }

        private static System.Type GetDockWindow()
        {
            var result = new System.Collections.Generic.List<System.Type>();
            System.Reflection.Assembly[] AS = System.AppDomain.CurrentDomain.GetAssemblies();
            System.Type editorWindow = typeof(EditorWindow);
            foreach (var A in AS)
            {
                System.Type[] types = A.GetTypes();
                foreach (var T in types)
                {
                    if (T.IsSubclassOf(editorWindow))
                        result.Add(T);
                }
            }

            System.Type targetDockWindow = typeof(SceneView);

            foreach (System.Type win in result)
            {
                if (win.Name == "InspectorWindow")
                {
                    targetDockWindow = win;
                    break;
                }
            }

            return targetDockWindow;
        }

        [MenuItem("Tools/TerraTiler2D/Save Graph %&s", priority = 1)]
        static void SaveGraphCommand()
        {
            if (IsOpen)
            {
                Graph window = GetWindow<Graph>();
                window.RequestDataOperation(Graph.SaveOperation.Save);
            }
            else
            {
                Glob.GetInstance().DebugString("The TerraTiler2D window is not open, so there is no graph to be saved.", Glob.DebugCategories.Misc, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
            }
        }

        private GraphView _graphView;
        private Toolbar _toolbar;
        private Blackboard _blackboard;
        private Minimap _minimap;

        private string _fileName;

        //Gets called upon creating the TerraTiler2D window, and upon recompiling.

        private void OnGUI()
        {
            if (Instance == null)
            {
                Instance = this;

                //Initialize an empty TerraTiler2D window, with an empty graph.
                Instance.InitializeGraph();

                //If the tempGraphData is not dirty before loading it, make sure to mark it as such after loading it (calling LoadTempGraphData will make it dirty).
                bool setClean = !EditorUtility.IsDirty(GraphSaveManager.GetInstance().GetTempGraphData());

                //Check if there is any temp graph data that we can load
                GraphSaveManager.GetInstance().LoadTempGraphData(Instance);

                if (setClean)
                {
                    AssetDatabase.SaveAssets();
                }
            }
        }

        private void OnEnable()
        {
            //WARNING: Avoid using OnEnable if possible. Some methods will cause Unity to crash if called while loading an editor layout.
            //OnGUI can be used instead by using a toggle, although I am not 100% sure this is bug free.
        }

        //Gets called when the TerraTiler2D window gets closed.
        private void OnDisable()
        {
            if (_graphView != null)
            {
                rootVisualElement.Remove(_graphView);
            }
        }

        //Initializes an empty TerraTiler2D window, with an empty graph.
        private void InitializeGraph()
        {
            Glob.GetInstance().DebugString("Initializing an empty TerraTiler2D window.", Glob.DebugCategories.Misc, Glob.DebugLevel.Low);

            //If the file does not have a name yet, give it the default name.
            if (string.IsNullOrEmpty(GetFileName()))
            {
                SetFileName(Glob.GetInstance().DefaultGraphName);
            }

            ClearGraph();
            //Create the container for all the nodes and connections.
            ConstructGraphView();
            //Create a toolbar with options.
            GenerateToolbar();
            //Create a minimap of all the nodes.
            GenerateMiniMap();
            //Create an overlay containing properties.
            GenerateBlackBoard();

            GraphRunner.GetInstance().HasGraphBeenStopped = false;
        }

        private void ClearGraph()
        {
            if (_toolbar != null)
            {
                _toolbar.parent.Remove(_toolbar);
            }

            if (_blackboard != null)
            {
                _blackboard.parent.Remove(_blackboard);
            }

            if (_minimap != null)
            {
                _minimap.parent.Remove(_minimap);
            }

            if (_graphView != null)
            {
                _graphView.parent.Remove(_graphView);
            }

            rootVisualElement.Clear();
        }

        //Create the container for all the nodes and connections.
        private void ConstructGraphView()
        {
            //Create a new graph for in this window.
            _graphView = new GraphView(this)
            {
                name = "TerraTiler2D GraphView"
            };
        }

        //Create a toolbar with options.
        private void GenerateToolbar()
        {
            //Create a new minimap for in this window.
            _toolbar = new Toolbar(this, GetFileName())
            {
                name = "TerraTiler2D Toolbar"
            };
        }

        //Create a minimap of all the nodes.
        private void GenerateMiniMap()
        {
            //Create a new minimap for in this window.
            _minimap = new Minimap(this, Glob.GetInstance().MiniMapSize, new Vector2(10, 30))
            {
                name = "TerraTiler2D Minimap"
            };
        }

        //Create an overlay containing properties.
        private void GenerateBlackBoard()
        {
            //Create a new minimap for in this window.
            _blackboard = new Blackboard(_graphView, Glob.GetInstance().BlackboardSize, new Vector2(10, 30))
            {
                name = "TerraTiler2D Blackboard"
            };
        }

        public GraphView GetGraphView()
        {
            return _graphView;
        }
        public void AddToGraphView(VisualElement child)
        {
            _graphView.Add(child);
        }

        public void SetFileName(string fileName)
        {
            _fileName = fileName;

#if (UNITY_EDITOR)
            if (_toolbar != null)
            {
                _toolbar.SetFileName(fileName);
            }
#endif
        }
        public string GetFileName()
        {
            return _fileName;
        }

        public enum SaveOperation
        {
            Save,
            Load,
            Refresh
        }

        //Handles save and load operations.
        public void RequestDataOperation(SaveOperation operation, GraphData graphData = null)
        {
            //Do the correct operation.
            if (operation == SaveOperation.Save)
            {
                GraphSaveManager.GetInstance().SaveGraph();
            }
            else if (operation == SaveOperation.Load)
            {
                //Check if the GraphData file is valid.
                if (graphData == null)
                {
                    Glob.GetInstance().DebugString("The TerraTiler2D Graph you attempted to load returned null. Aborting load operation.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    return;
                }

                //Clear whatever is currently on the graph, and initialize a new blank graph.
                InitializeGraph();

                GraphSaveManager.GetInstance().LoadGraph(graphData, this.GetGraphView());

                //Set the tempGraphData to the loaded graph data.
                GraphSaveManager.GetInstance().SaveTempGraphData();
                //Clear the undo stack
                Undo.ClearAll();
            }
            else if (operation == SaveOperation.Refresh)
            {
                //Check if the GraphData file is valid.
                if (graphData == null)
                {
                    Glob.GetInstance().DebugString("The TerraTiler2D Graph you attempted to load returned null. Aborting load operation.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    return;
                }

                //Clear whatever is currently on the graph, and initialize a new blank graph.
                InitializeGraph();

                GraphSaveManager.GetInstance().LoadGraph(graphData, this.GetGraphView());
            }
        }

        //Save the current output of the graph
        public void SaveResult()
        {
            GraphRunner.GetInstance().RunGraph(this.GetGraphView(), handleSaveResult, true);
        }

        private void handleSaveResult(GraphFinishedEvent evt)
        {
            //TODO: Does this work as intended?
            World generatedWorld = evt.generatedOutput.GetProperty<World>("New World");

            //TODO: Save the world as a better file type than .csv
            generatedWorld.CreateCSVFile(GetFileName());
        }

        //Runs the graph from start to finish, and updates all previews.
        public void RunGraph()
        {
            //Run the graph
            GraphRunner.GetInstance().RunGraph(this.GetGraphView());
        }

        public void StopGraph()
        {
            Glob.GetInstance().DebugString("Stopping graph execution.", Glob.DebugCategories.Graph, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
            GraphRunner.GetInstance().StopGraph(this.GetGraphView());
        }

        public void ToggleLivePreview(bool toggle)
        {
            Glob.GetInstance().DebugString("Toggling the live preview has not been implemented yet.", Glob.DebugCategories.Error, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
        }

        //Opens the graph window and loads a graph when its asset is double clicked in the project window.
        /* OnOpenAssetAttribute has an option to provide an order index in the callback, starting at 0. 
        * This is useful if you have more than one OnOpenAssetAttribute callback, 
        * and you would like them to be called in a certain order. Callbacks are called in order, starting at zero.
        *
        * Must return true if you handled the opening of the asset or false if an external tool should open it.
        * The method with this attribute must use at least these two parameters :
        */
        [OnOpenAsset]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            //Use the instanceID of the GraphData to get the asset path.
            string assetPath = AssetDatabase.GetAssetPath(instanceID);

            //If there is a GraphData object that can be loaded
            if (AssetDatabase.GetMainAssetTypeAtPath(assetPath) == typeof(GraphData))
            {
                //If the TerraTiler2D window is currently open
                if (Graph.Instance != null)
                {
                    //If there are unsaved changes
                    if (EditorUtility.IsDirty(GraphSaveManager.GetInstance().GetTempGraphData()))
                    {
                        int dialogResponse = UnityEditor.EditorUtility.DisplayDialogComplex("Unsaved changes", "The currently opened graph has unsaved changes which will be discarded. \nAre you sure you want to close the graph?", "Close without saving", "Don't close", "Save and close");
                        //Show a dialog box that warns about unsaved changes.
                        if (dialogResponse == 1)
                        {
                            //If they select 'No', abort the operation
                            return false;
                        }
                        else if (dialogResponse == 2)
                        {
                            Graph.Instance.RequestDataOperation(SaveOperation.Save);
                        }
                    }
                }

                //Load the GraphData into the window
                bool returnValue = InitializeWindow(assetPath);

                return returnValue;
            }

            return false;
        }
    }
}
#endif
