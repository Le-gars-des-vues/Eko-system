using UnityEngine;
using UnityEngine.UIElements;

namespace TerraTiler2D
{
    public delegate Texture2D GetPreviewTextureMethod();

    public class NodePreview
    {
        private Node parent;

        private Toggle previewToggle = null;
        private bool shouldPreview = false;

        public NodePreview(Node parent, GetPreviewTextureMethod getPreviewMethod)
        {
            this.parent = parent;

            SetPreviewTextureMethod(getPreviewMethod);

#if (UNITY_EDITOR)
            //Only generate previews if this is in the Unity Editor
            shouldPreview = true;

            //Add a toggle to automatically show a preview when the target node is called
            previewToggle = new Toggle();
            previewToggle.text = "Auto";
            previewToggle.tooltip = "Should this node automatically show a preview when called in the Editor. NOTE: Generating previews may affect the performance of the graph.";
            previewToggle.SetValueWithoutNotify(shouldPreview);
            previewToggle.RegisterValueChangedCallback(SetShouldPreview);
            parent.titleContainer.Add(previewToggle);

            //Add a button that previews what this node will do
            Button button = new Button(() => { parent.GetGraphView().ResetNodeVariables(); parent.GetGraphView().ResetBlackboardProperties(); ShowTexture(); });
            button.text = "Preview";
            button.tooltip = "Show a preview of what this node will output.";
            parent.titleContainer.Add(button);

            parent.extensionContainer.style.backgroundColor = new Color(0, 0, 0, 1);
            parent.extensionContainer.style.alignItems = Align.Center;

            //Show an empty preview
            ShowTexture(null, true);

            EventManager.GetInstance().AddListener<GraphStartedEvent>(handleGraphStarted);
#endif
        }

        private void SetShouldPreview(ChangeEvent<bool> evt)
        {
            shouldPreview = evt.newValue;

#if (UNITY_EDITOR)
            //This is a very ugly way to prevent the NodePreview data from being saved when it is being loaded. The position of every node will be Vector2.zero when they are being loaded.
            if (parent.GetNodePosition().min != Vector2.zero)
            {
                EventManager.GetInstance().RaiseEvent(new NodeChangedEvent().Init(parent.GetNodeData()));
            }
#endif
        }
        public void SetShouldPreview(bool toggle)
        {
            if (previewToggle != null)
            {
                previewToggle.value = toggle;
            }
        }
        public bool ShouldPreview()
        {
            return shouldPreview;
        }

        public void SetPreviewTextureMethod(GetPreviewTextureMethod newMethod)
        {
            GetPreviewTexture = newMethod;
        }

        private GetPreviewTextureMethod GetPreviewTexture;

        public void ShowTexture(Texture2D preview = null, bool emptyPreview = false)
        {
            if (!shouldPreview && preview != null)
            {
                return;
            }
#if (UNITY_EDITOR)
            //If there is no texture to preview
            if (preview == null)
            {
                //If an actual preview was requested
                if (!emptyPreview)
                {
                    //If the delegate has not been defined
                    if (GetPreviewTexture == null)
                    {
                        //Warn the user, and abort
                        Glob.GetInstance().DebugString("NodePreview of node '" + parent + "' has not defined the GetPreviewTexture delegate. Use NodePreview.SetPreviewTextureMethod() to define the method.", Glob.DebugCategories.Misc, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                        return;
                    }
                    //If the delegate has been defined, get the preview from the delegate
                    preview = GetPreviewTexture();
                }
                //If an empty preview was requested
                else
                {
                    //Create an empty texture2D of the default preview size.
                    preview = new Texture2D((int)Glob.GetInstance().DefaultPreviewSize.x, (int)Glob.GetInstance().DefaultPreviewSize.y);
                }
            }

            parent.extensionContainer.Clear();

            if (preview != null)
            {
                if (preview.width != (int)Glob.GetInstance().DefaultPreviewSize.x || preview.height != (int)Glob.GetInstance().DefaultPreviewSize.y)
                {
                    //Resize the texture to the default preview size.
                    preview = Glob.GetInstance().ScaleToFitTexture2D(preview, (int)Glob.GetInstance().DefaultPreviewSize.x, (int)Glob.GetInstance().DefaultPreviewSize.y);
                }

                //Create an empty Image object.
                Image texturePreview = new Image();
                //Load the texture into the image.
                texturePreview.image = preview;
                //Name the image.
                texturePreview.name = "Preview";

                texturePreview.style.width = (int)Glob.GetInstance().DefaultPreviewSize.x;
                texturePreview.style.height = (int)Glob.GetInstance().DefaultPreviewSize.x;

                //Add the Image to this node.
                parent.extensionContainer.Add(texturePreview);
            }

            //Refresh the node, and apply all changes.
            parent.RefreshNode();
#endif
        }

        private void handleGraphStarted(GraphStartedEvent evt)
        {
            ShowTexture(null, true);
        }
    }
}
