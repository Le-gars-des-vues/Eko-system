#if (UNITY_EDITOR)
using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public class Toolbar : UnityEditor.UIElements.Toolbar
    {
        private Label fileNameField;

        public Toolbar(Graph graph, string fileName) : base()
        {
            fileNameField = new Label("File Name: " + fileName);
            Add(fileNameField);

            //Add a white space
            Add(new Label("\t"));

            //Add save and load buttons to the toolbar.
            Add(new Button(() => graph.RequestDataOperation(Graph.SaveOperation.Save)) { 
                text = "Save graph",
                tooltip = "Save any changes made to the currently loaded GraphData object."
            });

            //Add a generate world button to run the graph.
            Add(new Button(() => graph.SaveResult())
            {
                text = "Save result",
                tooltip = "Save the previously generated NewWorld property of the current graph as a .csv file."
            });

            //Add a white space
            Add(new Label("\t"));

            //Add a generate world button to run the graph.
            Add(new Button(() => graph.RunGraph()) { 
                text = "Run graph",
                tooltip = "Execute the graph."
            });

            //Add a generate world button to run the graph.
            Add(new Button(() => graph.StopGraph()) { 
                text = "Stop graph",
                tooltip = "Stop graph execution, and reset all the nodes and properties to the state they were at before graph execution."
            });

            //Add a white space
            //Add(new Label(" "));

            //Add a toggle to switch between live preview and manual preview.
            //var livePreviewToggle = new Toggle() { text = "Live preview" };
            //livePreviewToggle.RegisterValueChangedCallback(evt => graph.ToggleLivePreview(evt.newValue));
            //Add(livePreviewToggle);

            Button reportBugButton = new Button(() => Application.OpenURL("https://terratiler2d.tiemenkamp.com/Contact.html"))
            {
                text = "Report bug",
                tooltip = "If you encounter a bug or error, I would greatly appreciate it if you could let me know about it so that I can fix it. This button opens the contact page of the TerraTiler2D documentation website."
            };

            //Align the button to the right of the Toolbar
            reportBugButton.style.position = Position.Absolute;
            reportBugButton.style.right = 0;

            //Add the button to the Toolbar
            Add(reportBugButton);



            //bla.style

            //Add the toolbar to the window.
            graph.rootVisualElement.Add(this);
        }

        public void SetFileName(string name)
        {
            fileNameField.text = "File Name: " + name;
        }
    }
}
#endif
