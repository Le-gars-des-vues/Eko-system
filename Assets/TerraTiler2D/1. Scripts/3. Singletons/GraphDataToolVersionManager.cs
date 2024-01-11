using System.Collections.Generic;
using System.Linq;
using UnityEngine;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

namespace TerraTiler2D
{
    ///<summary>Automatically updates GraphData objects to the currently installed version of TerraTiler2D.</summary>

    public class GraphDataToolVersionManager : Singleton<GraphDataToolVersionManager>
    {
        private readonly List<float> versions = new List<float> { 1.00001f, 1.10001f };

        //Updates GraphData objects to newer versions whenever a new TerraTiler2D update gets released.
        public bool UpdateGraphData(GraphData data)
        {
            //NOTE: Required for GraphData objects updating from version 1.0.0.1f to 1.1.0.1f. Can be removed in the next update.
            if (data.ToolVersion <= 1)
            {
                data.ToolVersion = 1.00001f;
            }
            //If this is a brand new GraphData object.
            if (data.GetGraphDataVersion() <= 1)
            {
                data.ToolVersion = Glob.GetInstance().ToolVersion;
            }

            //If the GraphData has been initialized
            if (data.GetGUID() != null)
            {
                //If the GraphData is not the same as the currently installed version of TerraTiler2D
                if (data.ToolVersion != Glob.GetInstance().ToolVersion)
                {
                    //If the GraphData has an older version than the currently installed version of TerraTiler2D
                    if (data.ToolVersion < Glob.GetInstance().ToolVersion)
                    {
#if (UNITY_EDITOR)
                        //Show a dialog box that asks if the user wants to update the GraphData to the new version.
                        bool dialogResponse = UnityEditor.EditorUtility.DisplayDialog("Old GraphData version", "GraphData object '" + data.GetFileName() + "' is at version '" + Glob.GetInstance().GetToolVersionAsString(data.ToolVersion) + "', but version '" + Glob.GetInstance().GetToolVersionAsString(Glob.GetInstance().ToolVersion) + "' of TerraTiler2D is currently installed. \n\nDo you want to update the GraphData to version '" + Glob.GetInstance().GetToolVersionAsString(Glob.GetInstance().ToolVersion) + "'?", "Yes, update this GraphData", "No, let me make a back-up first");
                        if (!dialogResponse)
                        {
                            //If they select 'No', abort the operation
                            return false;
                        }
                        else
                        {
#endif
                            //For every version of the tool that exists
                            for (int i = 0; i < versions.Count - 1; i++)
                            {
                                //If the GraphData is at versions[i]
                                if (data.ToolVersion == versions[i])
                                {
                                    //Try to update the GraphData to the next version
                                    if (updateToVersion(data, versions[i+1]))
                                    {
                                        //If succesfully updated, update the ToolVersion variable to the new version
                                        data.ToolVersion = versions[i+1];
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }

#if (UNITY_EDITOR)
                            //Write the changes to disk so they persist
                            AssetDatabase.Refresh();
                            EditorUtility.SetDirty(data);
                            AssetDatabase.SaveAssets();
#endif
                            //If the GraphData object is at the same version as the currently installed version of TerraTiler2D
                            if (data.ToolVersion == Glob.GetInstance().ToolVersion)
                            {
                                Glob.GetInstance().DebugString("GraphData '" + data.GetFileName() + "' has been updated and is now compatible with the installed version of TerraTiler2D, version " + Glob.GetInstance().GetToolVersionAsString(Glob.GetInstance().ToolVersion) + ".", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Default);
                                return true;
                            }
                            else
                            {
                                Glob.GetInstance().DebugString("GraphData '" + data.GetFileName() + "' failed to update to the required version of TerraTiler2D.\n\n", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                                return false;
                            }
#if (UNITY_EDITOR)
                        }
#endif
                    }
                    else
                    {
                        Glob.GetInstance().DebugString("GraphData '" + data.GetFileName() + "' has a newer version than the currently installed version of TerraTiler2D. \n" +
                                                        "TerraTiler2D currently does not support backwards compatibility.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                        return false;
                    }
                }
            }
            
            //The GraphData object is at the same version as the currently installed version of TerraTiler2D
            return true;
        }

        private bool updateToVersion(GraphData data, float version)
        {
            Glob.GetInstance().DebugString("===== Updating GraphData '" + data.GetFileName() + "' to ToolVersion " + Glob.GetInstance().GetToolVersionAsString(version) + " =====\n", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);

            if (version == versions[1])
            {
                return updateToVersion1_1_0(data);
            }

            return false;
        }

        private bool updateToVersion1_1_0(GraphData data)
        {
            Glob.GetInstance().DebugString("Automatically updating GraphData from version " + Glob.GetInstance().GetToolVersionAsString(data.ToolVersion) + " or older to version 1.1.0 is not supported. \n" +
                                            "Please install a compatible version of TerraTiler2D, create a new GraphData object, or try to manually update this GraphData object (see below)." +
                                            "\n\n" +
                                            "Apologies for not offering an automatic GraphData update to TerraTiler2D version 1.1.0. \n" +
                                            "In the original release I paid no attention to how future updates would be rolled out, which is biting me in the ass right now. \n" +
                                            "Version 1.1.0 has restructured a lot of code to allow future updates to go a lot more smoothly, so this will hopefully be the last time that GraphData is not automatically updated to a new version." +
                                            "\n\n" +
                                            "To manually update this GraphData to version 1.1.0, follow these steps:" +
                                            "\n\n" +
                                            "Step 1. Move any Tile asset or Texture2D asset referenced by a port or property to '.../Resources/TerraTiler2D/Graph Resources'." +
                                            "\n\n" +
                                            "Step 2. Open this GraphData object in the Inspector, and change 'Terra Tiler 2D Version' to '1.10001'. This should allow you to open this GraphData object, but all of your connections, resource references, and possibly more, will be lost. \n" +
                                            "At this point it is advised to stop following these steps, and simply reconnect everything. If reconnecting everything is not an option for you, move on to the next step. The next steps will be extremely tedious, so be warned." +
                                            "\n\n" +
                                            "Step 3. Open this GraphData object in the Inspector, and update the following 'File Name' fields in your GraphData object manually, by changing 'Tiles/' to 'TerraTiler2D/Graph Resources/':\n" +
                                            "\t- 'Port Tile Data/[GUID]/Value/File Name'\n" +
                                            "\t- 'Port Texture2D Data/[GUID]/Value/File Name'\n" +
                                            "\t- 'Blackboard Property_Tile/[GUID]/Property Value/File Name'\n" +
                                            "\t- 'Blackboard Property_Texture2D/[GUID]/Property Value/File Name'" +
                                            "\n\n" +
                                            "Step 4. This is an extemely tedious process, so be warned. The format of generating GUIDs for every port has changed, meaning you will have to manually change all the port GUID references in the inspector to the new format.\n" +
                                            "The old format was 'NodeGUID.Append(1)' for every port, starting in the input container from top to bottom, and following with the output container from top to bottom.\n" +
                                            "\t(Example) The 'AddInt' node has 2 input ports and 1 output port. The first input port would have GUID '[NodeGUID]1', the second input port '[NodeGUID]11', and the output port '[NodeGUID]111'\n" +
                                            "The new format is no longer tied to the order of the ports on the node, but instead relies on the 'uniqueGuidExtension' argument in the 'GeneratePort<T>()' and 'GeneratePortWithField<T>()' methods.\n" +
                                            "All port GUIDs are now '[NodeGUID][uniqueGuidExtension]'. In general, ports still use the order on the node as their uniqueGuidExtension, but this is not always true. \n" +
                                            "Input ports and output ports now count their order separately, with the output ports counting into the negatives.\n" +
                                            "\t(Example) The 'AddInt' node has 2 input ports and 1 output port. The first input port would have GUID '[NodeGUID]1', the second input port '[NodeGUID]2', and the output port '[NodeGUID]-1'\n" +
                                            "However, this format does not hold true for every node. To check the exact uniqueGuidExtension for every port, open up the script of the node you are trying to repair, and look at what value is passed into the 'uniqueGuidExtension' argument of the 'GeneratePort<T>()' and 'GeneratePortWithField<T>()' methods (don't forget to look at inherited ports)." +
                                            "\n\n" +
                                            "So now you know how to convert the port GUIDs to the new format, but you still need to apply the new GUID's. To do this you once again open this GraphData object in the Inspector.\n" +
                                            "For every entry in any of the Port lists, you have to check which type of node the port belongs to. You can do this by comparing the 'Node GUID' value to the entries in the 'Node Data' list, and looking at the 'Node Type' field in the matching Node GUID.\n" +
                                            "Once you know the Node Type and the original PortGUID, you can convert the PortGUID using the process explained above.\n" +
                                            "You have to do the same for the 'Node Links' list, and convert all the 'Base Port Guid' and 'Target Port Guid' fields." +
                                            "\n\n" +
                                            "Hopefully you didn't have to put yourself through this hell, but if you did I truly hope it worked. \n" +
                                            "If after following these steps it still doesn't work, please contact me for help.\n"
                                        , Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
            return false;
        }
    }
}