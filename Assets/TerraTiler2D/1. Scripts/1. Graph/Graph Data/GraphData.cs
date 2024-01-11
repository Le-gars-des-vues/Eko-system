using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TerraTiler2D
{
    /// <summary>
    /// Graph data gets saved in instances of this ScriptableObject.
    /// These instances can be passed into <see cref="GraphRunner"/>.GenerateWorld() to generate worlds based on the graph data.
    /// </summary>
    [Serializable]
    [CreateAssetMenu(fileName = "New TerraTiler2D Graph", menuName = "TerraTiler2D Graph", order = 40)]
    public class GraphData : ScriptableObject
    {
        [SerializeField]
        private SerializableGraphData myData = new SerializableGraphData();

        [Serializable]
        private class SerializableGraphData
        {
            [SerializeField]
            public string GUID;

            [SerializeField]
            public int Version = 1;

            [SerializeField]
            public string FileName;

            //[SerializeField]
            public float TerraTiler2DVersion = 1.0f; //Should forever stay at 1.0f. Keeping it at 1.0f ensures that all old GraphData objects will get properly updated to future versions of TerraTiler2D, and all new GraphData will have the correct version assigned.

            //Connections between nodes
            [SerializeField]
            public List<NodeLinkData> NodeLinks = new List<NodeLinkData>();

            //Properties in the blackboard
            [SerializeField]
            public List<PropertyData<int>> BlackboardProperty_Int = new List<PropertyData<int>>();
            [SerializeField]
            public List<PropertyData<float>> BlackboardProperty_Float = new List<PropertyData<float>>();
            [SerializeField]
            public List<PropertyData<bool>> BlackboardProperty_Bool = new List<PropertyData<bool>>();
            [SerializeField]
            public List<PropertyData<Serializable.Vector2>> BlackboardProperty_Vector2 = new List<PropertyData<Serializable.Vector2>>();
            [SerializeField]
            public List<PropertyData<Serializable.Vector3>> BlackboardProperty_Vector3 = new List<PropertyData<Serializable.Vector3>>();
            [SerializeField]
            public List<PropertyData<Serializable.Vector4>> BlackboardProperty_Vector4 = new List<PropertyData<Serializable.Vector4>>();
            [SerializeField]
            public List<PropertyData_Cloneable<string>> BlackboardProperty_String = new List<PropertyData_Cloneable<string>>();
            [SerializeField]
            public List<PropertyData<Serializable.Color>> BlackboardProperty_Color = new List<PropertyData<Serializable.Color>>();
            [SerializeField]
            public List<PropertyData<Serializable.Gradient>> BlackboardProperty_Gradient = new List<PropertyData<Serializable.Gradient>>();
            [SerializeField]
            public List<PropertyData<Serializable.Texture2D>> BlackboardProperty_Texture2D = new List<PropertyData<Serializable.Texture2D>>();
            [SerializeField]
            public List<PropertyData<Serializable.TileBase>> BlackboardProperty_Tile = new List<PropertyData<Serializable.TileBase>>();
            [SerializeField]
            public List<PropertyData_Cloneable<Serializable.World>> BlackboardProperty_World = new List<PropertyData_Cloneable<Serializable.World>>();

            //Data in the nodes
            [SerializeField]
            public List<NodeData> NodeData = new List<NodeData>();
            //Node data for nodes with a preview
            [SerializeField]
            public List<Preview_NodeData> NodeData_Preview = new List<Preview_NodeData>();
            [SerializeField]
            public List<Array_NodeData> ArrayNodeData = new List<Array_NodeData>();
            [SerializeField]
            public List<NoisemapDimension_NodeData> NoisemapDimensionNodeData = new List<NoisemapDimension_NodeData>();

            [SerializeField]
            public List<PortData> PortData = new List<PortData>();
            [SerializeField]
            public List<PortWithFieldData<int>> PortIntData = new List<PortWithFieldData<int>>();
            [SerializeField]
            public List<PortWithFieldData<float>> PortFloatData = new List<PortWithFieldData<float>>();
            [SerializeField]
            public List<PortWithFieldData<string>> PortStringData = new List<PortWithFieldData<string>>();
            [SerializeField]
            public List<PortWithFieldData<bool>> PortBoolData = new List<PortWithFieldData<bool>>();
            [SerializeField]
            public List<PortWithFieldData<Serializable.Vector2>> PortVector2Data = new List<PortWithFieldData<Serializable.Vector2>>();
            [SerializeField]
            public List<PortWithFieldData<Serializable.Vector3>> PortVector3Data = new List<PortWithFieldData<Serializable.Vector3>>();
            [SerializeField]
            public List<PortWithFieldData<Serializable.Vector4>> PortVector4Data = new List<PortWithFieldData<Serializable.Vector4>>();
            [SerializeField]
            public List<PortWithFieldData<Serializable.Color>> PortColorData = new List<PortWithFieldData<Serializable.Color>>();
            [SerializeField]
            public List<PortWithFieldData<Serializable.Gradient>> PortGradientData = new List<PortWithFieldData<Serializable.Gradient>>();
            [SerializeField]
            public List<PortWithFieldData<Serializable.Texture2D>> PortTexture2DData = new List<PortWithFieldData<Serializable.Texture2D>>();
            [SerializeField]
            public List<PortWithFieldData<Serializable.TileBase>> PortTileData = new List<PortWithFieldData<Serializable.TileBase>>();

            public enum Compatibility
            {
                Compatible = 0,
                OlderVersion = 1,
                NewerVersion = 2,
                NotCompatible = 3,
            }

            public void CopyData(SerializableGraphData copyFrom, bool persistent = false)
            {
                Glob.GetInstance().DebugString("Attempting to copy data from GraphData with name '" + copyFrom.FileName + "' and GUID '" + copyFrom.GUID + "', to GraphData with name '" + FileName + "' and GUID '" + GUID + "'.", Glob.DebugCategories.Data, Glob.DebugLevel.Medium, Glob.DebugTypes.Default);
                
                //Prevent data with a different GUID than this GraphData from overwriting.
                if (IsCompatible(copyFrom) == Compatibility.NotCompatible)
                {
                    //This is prevented because it potentially overwrites an entire GraphData save with a completely different GraphData save, resulting in lost data. If this accidentally happened to you, don't panic! Do NOT press the Save Graph button, as that will permanently overwrite the data. Instead, exit out of Unity, and reopen your project. Your data should now be back when you open the file.
                    //This might prevent you from loading data when you renamed your GraphData object and are trying to load old GraphData with the old file name.
                    Glob.GetInstance().DebugString("The loaded GraphData does not match the current GraphData. The loaded GraphData has filename '" + copyFrom.FileName + "' and GUID '" + copyFrom.GUID + "', and the current GraphData has filename '" + FileName + "' and GUID '" + GUID + "'. Aborting operation to prevent potentially unintentional data overwriting.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    return;
                }

                GUID = copyFrom.GUID;

                Version = copyFrom.Version;

                FileName = copyFrom.FileName;

                TerraTiler2DVersion = copyFrom.TerraTiler2DVersion;

                CopyPropertyData(copyFrom, persistent);

                handleCopyList(copyFrom.NodeLinks, ref NodeLinks);

                handleCopyList(copyFrom.NodeData, ref NodeData);
                handleCopyList(copyFrom.NodeData_Preview, ref NodeData_Preview);

                handleCopyList(copyFrom.ArrayNodeData, ref ArrayNodeData);
                handleCopyList(copyFrom.NoisemapDimensionNodeData, ref NoisemapDimensionNodeData);

                handleCopyList(copyFrom.PortData, ref PortData);
                handleCopyList(copyFrom.PortIntData, ref PortIntData);
                handleCopyList(copyFrom.PortFloatData, ref PortFloatData);
                handleCopyList(copyFrom.PortStringData, ref PortStringData);
                handleCopyList(copyFrom.PortBoolData, ref PortBoolData);
                handleCopyList(copyFrom.PortVector2Data, ref PortVector2Data);
                handleCopyList(copyFrom.PortVector3Data, ref PortVector3Data);
                handleCopyList(copyFrom.PortVector4Data, ref PortVector4Data);
                handleCopyList(copyFrom.PortColorData, ref PortColorData);
                handleCopyList(copyFrom.PortGradientData, ref PortGradientData);
                handleCopyList(copyFrom.PortTexture2DData, ref PortTexture2DData);
                handleCopyList(copyFrom.PortTileData, ref PortTileData);
            }

            public void CopyPropertyData(SerializableGraphData copyFrom, bool persistent = false)
            {
                Glob.GetInstance().DebugString("Attempting to copy property data from GraphData with name '" + copyFrom.FileName + "' and GUID '" + copyFrom.GUID + "', to GraphData with name '" + FileName + "' and GUID '" + GUID + "'.", Glob.DebugCategories.Data, Glob.DebugLevel.Medium, Glob.DebugTypes.Default);
                
                //Prevent incompatible data from overwriting.
                if (IsCompatible(copyFrom) == Compatibility.NotCompatible)
                {
                    //This is prevented because it potentially overwrites an entire GraphData save with a completely different GraphData save, resulting in lost data. If this accidentally happened to you, don't panic! Do NOT press the Save Graph button, as that will permanently overwrite the data. Instead, exit out of Unity, and reopen your project. Your data should now be back when you open the file.
                    //This might prevent you from loading data when you renamed your GraphData object and are trying to load old GraphData with the old file name.
                    if (copyFrom != null)
                    {
                        Glob.GetInstance().DebugString("The loaded GraphData does not match the current GraphData. The loaded GraphData has filename '" + copyFrom.FileName + "' and GUID '" + copyFrom.GUID + "', and the current GraphData has filename '" + FileName + "' and GUID '" + GUID + "'. Aborting operation to prevent potentially unintentional data overwriting.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                    }
                    return;
                }

                TerraTiler2DVersion = copyFrom.TerraTiler2DVersion;

                handleCopyProperties(   BlackboardProperty_Int,        copyFrom.BlackboardProperty_Int,        persistent   );
                handleCopyProperties(   BlackboardProperty_Float,      copyFrom.BlackboardProperty_Float,      persistent   );
                handleCopyProperties(   BlackboardProperty_Bool,       copyFrom.BlackboardProperty_Bool,       persistent   );
                handleCopyProperties(   BlackboardProperty_Vector2,    copyFrom.BlackboardProperty_Vector2,    persistent   );
                handleCopyProperties(   BlackboardProperty_Vector3,    copyFrom.BlackboardProperty_Vector3,    persistent   );
                handleCopyProperties(   BlackboardProperty_Vector4,    copyFrom.BlackboardProperty_Vector4,    persistent   );
                handleCopyProperties(   BlackboardProperty_String,     copyFrom.BlackboardProperty_String,     persistent   );
                handleCopyProperties(   BlackboardProperty_Color,      copyFrom.BlackboardProperty_Color,      persistent   );
                handleCopyProperties(   BlackboardProperty_Gradient,   copyFrom.BlackboardProperty_Gradient,   persistent   );
                handleCopyProperties(   BlackboardProperty_Texture2D,  copyFrom.BlackboardProperty_Texture2D,  persistent   );
                handleCopyProperties(   BlackboardProperty_Tile,       copyFrom.BlackboardProperty_Tile,       persistent   );
                handleCopyProperties(   BlackboardProperty_World,      copyFrom.BlackboardProperty_World,      persistent   );
            }

            private void handleCopyList<T>(List<T> copyFrom, ref List<T> copyTo) where T : ICloneable
            {
                copyTo.Clear();

                foreach (T item in copyFrom)
                {
                    copyTo.Add((T)item.Clone());
                }
            }

            private void handleCopyProperties<T>(List<PropertyData<T>> current, List<PropertyData<T>> copyFrom, bool persistent)
            {
                //For every property that we are copying
                foreach (PropertyData<T> property in copyFrom)
                {
                    //If we currently have a property with the same GUID as the property we are copying
                    if (current.Any(x => x.GUID == property.GUID))
                    {
                        //Update our property's value and name to the new value and name
                        current.First(x => x.GUID == property.GUID).PropertyValue = property.PropertyValue;
                        current.First(x => x.GUID == property.GUID).PropertyName = property.PropertyName;
                    }
                    //If we do not have a property with a matching GUID yet
                    else
                    {
                        //Add the property
                        current.Add((PropertyData<T>)property.Clone());
                    }
                }

                //If copying is persistent, remove any excess properties
                if (persistent)
                {
                    for (int i = current.Count - 1; i >= 0; i--)
                    {
                        //If the save file we are copying from does not have a property with the same GUID
                        if (!copyFrom.Any(x => x.GUID == current[i].GUID))
                        {
                            //Remove the property
                            current.RemoveAt(i);
                        }
                    }
                }
            }
            private void handleCopyProperties<T>(List<PropertyData_Cloneable<T>> current, List<PropertyData_Cloneable<T>> copyFrom, bool persistent) where T : ICloneable
            {
                foreach (PropertyData_Cloneable<T> property in copyFrom)
                {
                    if (current.Any(x => x.GUID == property.GUID))
                    {
                        current.First(x => x.GUID == property.GUID).PropertyValue = (T)property.PropertyValue.Clone();
                        current.First(x => x.GUID == property.GUID).PropertyName = property.PropertyName;
                    }
                    else
                    {
                        current.Add((PropertyData_Cloneable<T>)property.Clone());
                    }
                }

                //If copying is persistent, remove any excess properties
                if (persistent)
                {
                    for (int i = current.Count - 1; i >= 0; i--)
                    {
                        //If the save file we are copying from does not have a property with the same GUID
                        if (!copyFrom.Any(x => x.GUID == current[i].GUID))
                        {
                            //Remove the property
                            current.RemoveAt(i);
                        }
                    }
                }
            }

            public Compatibility IsCompatible(SerializableGraphData other)
            {
                if (other == null)
                {
                    return Compatibility.NotCompatible;
                }

                //Check if the two save files are from the same GraphData object
                if (other.GUID == GUID)
                {
                    //Check if the two save files have the same version
                    if (other.Version != Version)
                    {
                        //If the two save files do not have the same version, warn the user.
                        //When a save file of an older version is loaded into the current GraphData, the GraphData object will completely revert back to that older version, including nodes, connections, and properties. This could produce errors if Node types were changed, like adding or removing ports, or completely deleting a Node type.
                        //When only the properties of an older version save file are loaded into the current GraphData, the GraphData will only revert its properties back to the older version. If this change is persistent, this could produce errors if properties were added that are used by Getter or Setter nodes, since that new property will no longer exist.
                        Glob.GetInstance().DebugString("Loaded GraphData is at version '" + other.Version +"', which is different from current GraphData at version '" + Version +"'.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);

                        if (other.Version < Version)
                        {
                            return Compatibility.OlderVersion;
                        }
                        else
                        {
                            return Compatibility.NewerVersion;
                        }
                    }

                    return Compatibility.Compatible;
                }

                return Compatibility.NotCompatible;
            }
        }

        private void OnEnable()
        {
#if (UNITY_EDITOR)
            if (string.IsNullOrEmpty(GetGUID()))
            {
                SetGUID(UnityEditor.GUID.Generate().ToString());
            }
#endif
        }
#if (UNITY_EDITOR)
        private void Awake()
        {
            //Gets called the first time a GraphData is viewed after opening Unity, or after it was created.
            handleDuplication();
        }

        private void handleDuplication()
        {
            string assetPath = AssetDatabase.GetAssetPath(this);
            if (string.IsNullOrEmpty(assetPath))
            {
                //This is a newly created GraphData object.
                return;
            }

            string[] actualAssetPath = assetPath.Split('/');
            string actualAssetName = actualAssetPath[actualAssetPath.Length-1].Remove(actualAssetPath[actualAssetPath.Length - 1].Length - 6);

            //If the these two values don't match, it means this Object's name was changed during this frame, which means it was duplicated.
            if (actualAssetName != this.name)
            {
                //Change the GUID so it differs from the object it was duplicated from
                this.SetGUID(GUID.Generate().ToString());
                //Set the actual file name
                this.SetFileName(actualAssetName);

                AssetDatabase.Refresh();
                EditorUtility.SetDirty(this);
                AssetDatabase.SaveAssets();
            }

            return;
        }
#endif
        public string GetFileName()
        {
            return myData.FileName;
        }
        public void SetFileName(string fileName)
        {
            myData.FileName = fileName;
        }
        public string GetGUID()
        {
            return myData.GUID;
        }
        public void SetGUID(string newGuid)
        {
            myData.GUID = newGuid;
        }
        //Keeps track of how many times this GraphData has been saved.
        public void HandleGraphDataVersion()
        {
            myData.Version += 1;
        }
        public void SetGraphDataVersion(int newVersion)
        {
            myData.Version = newVersion;
        }
        public int GetGraphDataVersion()
        {
            return myData.Version;
        }

        public float ToolVersion
        {
            get
            {
                return this.myData.TerraTiler2DVersion;
            }
            set
            {
                this.myData.TerraTiler2DVersion = value;
            }
        }

        public void SetData(GraphData copyFrom)
        {
            myData.CopyData(copyFrom.GetData(), true);
        }
        private SerializableGraphData GetData()
        {
            return myData;
        }

        public enum DataOperation
        {
            Add,
            Change,
            Remove
        }

        public void HandleNodeLink(NodeLinkData link, DataOperation operation = DataOperation.Add, bool trackUndoStack = true)
        {
            Glob.GetInstance().DebugString("Handling NodeLink operation '" + operation + "' for NodeLink between output Port with GUID '" + link.BasePortGuid + "' to input Port with GUID '" + link.TargetPortGuid + "'.", Glob.DebugCategories.Data, Glob.DebugLevel.All, Glob.DebugTypes.Default);

#if (UNITY_EDITOR)
            if (trackUndoStack)
            {
                UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Graph change");
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
            if (operation == DataOperation.Add)
            {
                //If this link has not been added yet
                if (!myData.NodeLinks.Any(x => x.BaseNodeGuid == link.BaseNodeGuid && x.BasePortGuid == link.BasePortGuid && x.TargetNodeGuid == link.TargetNodeGuid && x.TargetPortGuid == link.TargetPortGuid))
                {
                    myData.NodeLinks.Add(link);
                }
            }
            else if(operation == DataOperation.Remove)
            {
                if (myData.NodeLinks.Any(x => x.BaseNodeGuid == link.BaseNodeGuid && x.BasePortGuid == link.BasePortGuid && x.TargetNodeGuid == link.TargetNodeGuid && x.TargetPortGuid == link.TargetPortGuid))
                {
                    myData.NodeLinks.Remove(myData.NodeLinks.First(x => x.BaseNodeGuid == link.BaseNodeGuid && x.BasePortGuid == link.BasePortGuid && x.TargetNodeGuid == link.TargetNodeGuid && x.TargetPortGuid == link.TargetPortGuid));
                }
            }
        }

        public void HandleNodeData(NodeData nodeData, DataOperation operation = DataOperation.Add, bool trackUndoStack = true)
        {
            Glob.GetInstance().DebugString("Handling Node operation '" + operation + "' for node with GUID '" + nodeData.GUID + "'.", Glob.DebugCategories.Data, Glob.DebugLevel.All, Glob.DebugTypes.Default);

#if (UNITY_EDITOR)
            if (Application.isPlaying)
            {
                Glob.GetInstance().DebugString("Changes made to a GraphData object during runtime will not persist.", Glob.DebugCategories.Data, Glob.DebugLevel.Medium, Glob.DebugTypes.Warning);
                return;
            }
            if (trackUndoStack)
            {
                UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Graph change");
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif
            if (nodeData.GetType() == typeof(NodeData))
            {
                handleNodeOperation<NodeData>(nodeData, ref myData.NodeData, operation);
                return;
            }
            else if (nodeData.GetType() == typeof(Preview_NodeData))
            {
                handleNodeOperation<Preview_NodeData>(nodeData, ref myData.NodeData_Preview, operation);
                return;
            }

            switch (nodeData.NodeType)
            {
                case Glob.NodeTypes.Array:
                    handleNodeOperation<Array_NodeData>(nodeData, ref myData.ArrayNodeData, operation);
                    break;
                case Glob.NodeTypes.NoisemapDimension:
                    handleNodeOperation<NoisemapDimension_NodeData>(nodeData, ref myData.NoisemapDimensionNodeData, operation);
                    break;

                case Glob.NodeTypes.Default:
                    handleNodeOperation<NodeData>(nodeData, ref myData.NodeData, operation);

                    Glob.GetInstance().DebugString("A default node was added to the save file. This is not supposed to happen. Perhaps you forgot to set the node type in the constructor of a node?", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                    if (!string.IsNullOrEmpty(nodeData.NodeName))
                    {
                        Glob.GetInstance().DebugString("\tDefault node name: " + nodeData.NodeName, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                    }
                    break;

                default:
                    break;
            }
        }
        private void handleNodeOperation<T>(NodeData nodeData, ref List<T> nodeDataList, DataOperation operation) where T : NodeData
        {
            if (nodeData.GUID == null)
            {
                return;
            }

            if (operation == DataOperation.Add)
            {
                if (!nodeDataList.Any(x => x.GUID == nodeData.GUID))
                {
                    nodeDataList.Add(nodeData as T);
                }
            }
            else if (operation == DataOperation.Remove)
            {
                if (nodeDataList.Any(x => x.GUID == nodeData.GUID))
                {
                    nodeDataList.Remove(nodeDataList.Find(x => x.GUID == nodeData.GUID));
                }
            }
            else if (operation == DataOperation.Change)
            {
                //If the node data exists
                if (nodeDataList.Any(x => x.GUID == nodeData.GUID))
                {
                    //Copy all of the changed data to the saved data
                    nodeDataList.First(x => x.GUID == nodeData.GUID).CopyNodeDataFrom(nodeData);
                }
            }
        }

        public void HandlePortData(PortData portData, DataOperation operation = DataOperation.Add, bool trackUndoStack = true)
        {
            Glob.GetInstance().DebugString("Handling Port operation '" + operation + "' for port with GUID '" + portData.PortGUID + "'.", Glob.DebugCategories.Data, Glob.DebugLevel.All, Glob.DebugTypes.Default);
            
            if (operation == DataOperation.Add && GetAllPortData().Exists(x => x.PortGUID == portData.PortGUID))
            {
                Glob.GetInstance().DebugString("Data already exists: " + portData.PortGUID + ", aborting operation.", Glob.DebugCategories.Data, Glob.DebugLevel.High, Glob.DebugTypes.Default);
                return;
            }

#if (UNITY_EDITOR)
            if (Application.isPlaying)
            {
                Glob.GetInstance().DebugString("Changes made to a GraphData object during runtime will not persist.", Glob.DebugCategories.Data, Glob.DebugLevel.Medium, Glob.DebugTypes.Warning);
                return;
            }
            if (trackUndoStack)
            {
                UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Graph change");
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif

            if (portData.GetType() == typeof(PortWithFieldData<int>))
            {
                handlePortOperation<PortWithFieldData<int>>(portData, ref myData.PortIntData, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<float>))
            {
                handlePortOperation<PortWithFieldData<float>>(portData, ref myData.PortFloatData, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<string>))
            {
                handlePortOperation<PortWithFieldData<string>>(portData, ref myData.PortStringData, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<bool>))
            {
                handlePortOperation<PortWithFieldData<bool>>(portData, ref myData.PortBoolData, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<Serializable.Vector2>))
            {
                handlePortOperation<PortWithFieldData<Serializable.Vector2>>(portData, ref myData.PortVector2Data, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<Serializable.Vector3>))
            {
                handlePortOperation<PortWithFieldData<Serializable.Vector3>>(portData, ref myData.PortVector3Data, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<Serializable.Vector4>))
            {
                handlePortOperation<PortWithFieldData<Serializable.Vector4>>(portData, ref myData.PortVector4Data, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<Serializable.Color>))
            {
                handlePortOperation<PortWithFieldData<Serializable.Color>> (portData, ref myData.PortColorData, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<Serializable.Gradient>))
            {
                handlePortOperation<PortWithFieldData<Serializable.Gradient>> (portData, ref myData.PortGradientData, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<Serializable.Texture2D>))
            {
                handlePortOperation<PortWithFieldData<Serializable.Texture2D>> (portData, ref myData.PortTexture2DData, operation);
            }
            else if (portData.GetType() == typeof(PortWithFieldData<Serializable.TileBase>))
            {
                handlePortOperation<PortWithFieldData<Serializable.TileBase>> (portData, ref myData.PortTileData, operation);
            }
            else
            {
                handlePortOperation<PortData>(portData, ref myData.PortData, operation);
            }
        }
        private void handlePortOperation<T>(PortData portData, ref List<T> portDataList, DataOperation operation) where T : PortData
        {
            if (portData.PortGUID == null)
            {
                return;
            }

            if (operation == DataOperation.Add)
            {
                if (!portDataList.Any(x => x.PortGUID == portData.PortGUID))
                {
                    portDataList.Add(portData as T);
                }
            }
            else if (operation == DataOperation.Remove)
            {
                if (portDataList.Any(x => x.PortGUID == portData.PortGUID))
                {
                    portDataList.Remove(portDataList.Find(x => x.PortGUID == portData.PortGUID));
                }
            }
            else if (operation == DataOperation.Change)
            {
                //If the port data exists
                if (portDataList.Any(x => x.PortGUID == portData.PortGUID))
                {
                    //Copy all of the changed data to the saved data
                    portDataList.First(x => x.PortGUID == portData.PortGUID).CopyPortDataFrom(portData);
                }
            }
        }

        public void HandlePropertyData(PropertyData_Abstract propertyData, DataOperation operation = DataOperation.Add, bool trackUndoStack = true)
        {
            Glob.GetInstance().DebugString("Handling Property operation '" + operation + "' for property with GUID '" + propertyData.GUID + "'.", Glob.DebugCategories.Data, Glob.DebugLevel.All, Glob.DebugTypes.Default);

#if (UNITY_EDITOR)
            if (Application.isPlaying)
            {
                Glob.GetInstance().DebugString("Changes made to a GraphData object during runtime will not persist.", Glob.DebugCategories.Data, Glob.DebugLevel.Medium, Glob.DebugTypes.Warning);
                return;
            }

            if (trackUndoStack)
            {
                UnityEditor.Undo.RegisterCompleteObjectUndo(this, "Graph change");
                UnityEditor.EditorUtility.SetDirty(this);
            }
#endif

            if (propertyData.GetType() == typeof(PropertyData<int>))
            {
                handlePropertyOperation<PropertyData<int>>(propertyData, ref myData.BlackboardProperty_Int, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData<float>))
            {
                handlePropertyOperation<PropertyData<float>>(propertyData, ref myData.BlackboardProperty_Float, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData<bool>))
            {
                handlePropertyOperation<PropertyData<bool>>(propertyData, ref myData.BlackboardProperty_Bool, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData<Serializable.Vector2>))
            {
                handlePropertyOperation<PropertyData<Serializable.Vector2>>(propertyData, ref myData.BlackboardProperty_Vector2, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData<Serializable.Vector3>))
            {
                handlePropertyOperation<PropertyData<Serializable.Vector3>>(propertyData, ref myData.BlackboardProperty_Vector3, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData<Serializable.Vector4>))
            {
                handlePropertyOperation<PropertyData<Serializable.Vector4>>(propertyData, ref myData.BlackboardProperty_Vector4, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData_Cloneable<string>))
            {
                handlePropertyOperation<PropertyData_Cloneable<string>>(propertyData, ref myData.BlackboardProperty_String, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData<Serializable.Color>))
            {
                handlePropertyOperation<PropertyData<Serializable.Color>> (propertyData, ref myData.BlackboardProperty_Color, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData<Serializable.Gradient>))
            {
                handlePropertyOperation<PropertyData<Serializable.Gradient>> (propertyData, ref myData.BlackboardProperty_Gradient, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData<Serializable.Texture2D>))
            {
                handlePropertyOperation<PropertyData<Serializable.Texture2D>> (propertyData, ref myData.BlackboardProperty_Texture2D, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData<Serializable.TileBase>))
            {
                handlePropertyOperation<PropertyData<Serializable.TileBase>> (propertyData, ref myData.BlackboardProperty_Tile, operation);
            }
            else if (propertyData.GetType() == typeof(PropertyData_Cloneable<Serializable.World>))
            {
                handlePropertyOperation<PropertyData_Cloneable<Serializable.World>>(propertyData, ref myData.BlackboardProperty_World, operation);
            }
            else
            {
                Glob.GetInstance().DebugString("Blackboard property could not be saved, because the '" + propertyData.GetType().GetGenericTypeDefinition().Name + "' type is not supported.", Glob.DebugCategories.Data, Glob.DebugLevel.Low, Glob.DebugTypes.Warning);
            }
        }
        private void handlePropertyOperation<T>(PropertyData_Abstract propertyData, ref List<T> propertyDataList, DataOperation operation) where T : PropertyData_Abstract
        {
            if (propertyData.GUID == null)
            {
                return;
            }

            if (operation == DataOperation.Add)
            {
                if (!propertyDataList.Any(x => x.GUID == propertyData.GUID))
                {
                    propertyDataList.Add(propertyData as T);
                }
            }
            else if (operation == DataOperation.Remove)
            {
                if (propertyDataList.Any(x => x.GUID == propertyData.GUID))
                {
                    propertyDataList.Remove(propertyDataList.Find(x => x.GUID == propertyData.GUID));
                }
            }
            else if (operation == DataOperation.Change)
            {
                //If the property data exists
                if (propertyDataList.Any(x => x.GUID == propertyData.GUID))
                {
                    //Copy all of the changed data to the saved data
                    propertyDataList.First(x => x.GUID == propertyData.GUID).CopyPropertyDataFrom(propertyData);
                }
            }
        }

        public List<NodeLinkData> GetNodeLinks()
        {
            return myData.NodeLinks;
        }

        public List<PortData> GetPortDataWithGUID(string nodeGUID)
        {
            return GetAllPortData().FindAll(x => x.NodeGUID == nodeGUID);
        }
        public List<PortData> GetAllPortData()
        {
            List<PortData> AllPortData = new List<PortData>();

            AllPortData.AddRange(myData.PortData);
            AllPortData.AddRange(myData.PortIntData);
            AllPortData.AddRange(myData.PortFloatData);
            AllPortData.AddRange(myData.PortStringData);
            AllPortData.AddRange(myData.PortBoolData);
            AllPortData.AddRange(myData.PortVector2Data);
            AllPortData.AddRange(myData.PortVector3Data);
            AllPortData.AddRange(myData.PortVector4Data);
            AllPortData.AddRange(myData.PortColorData);
            AllPortData.AddRange(myData.PortGradientData);
            AllPortData.AddRange(myData.PortTexture2DData);
            AllPortData.AddRange(myData.PortTileData);

            return AllPortData;
        }

        //Get a list of all the NodeData types casted to base class NodeData.
        public List<NodeData> GetAllNodeData()
        {
            List<NodeData> AllNodeData = new List<NodeData>();

            AllNodeData.AddRange(myData.NodeData);
            AllNodeData.AddRange(myData.NodeData_Preview);
            AllNodeData.AddRange(myData.ArrayNodeData);
            AllNodeData.AddRange(myData.NoisemapDimensionNodeData);

            return AllNodeData;
        }
        /// <summary>
        /// Directly access the saved data of all the properties. It is advised to use this method only to access property data, and not to change anything. Use SetPropertyValue() to set the value of properties instead.
        /// </summary>
        public List<PropertyData_Abstract> GetAllPropertyData()
        {
            List<PropertyData_Abstract> AllBlackboardProperties = new List<PropertyData_Abstract>();

            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Int);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Float);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Bool);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Vector2);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Vector3);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Vector4);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_String);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Color);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Gradient);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Texture2D);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_Tile);
            AllBlackboardProperties.AddRange(myData.BlackboardProperty_World);

            //Sort the list based on the SortingIndex.
            AllBlackboardProperties.Sort();

            return AllBlackboardProperties;
        }

        //Get saved property data
        private PropertyData<T> GetBlackboardProperty<T>(string name)
        {
            Glob.GetInstance().DebugString("Attempting to retrieve Blackboard property with name '" + name + "'.", Glob.DebugCategories.Data, Glob.DebugLevel.Medium, Glob.DebugTypes.Default);

            List<PropertyData_Abstract> allProperties = GetAllPropertyData();

            foreach (PropertyData_Abstract property in allProperties)
            {
                if (property.PropertyName == name)
                {
                    if (property.GetType() == typeof(PropertyData<T>))
                    {
                        return property as PropertyData<T>;
                    }
                    else
                    {
                        Glob.GetInstance().DebugString("Incorrect property data type. Data has type " + property.GetType() + ", while the type " + typeof(PropertyData<T>) + " was requested. Attempting to cast to requested type, but this will likely fail.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
                        return property as PropertyData<T>;
                    }
                }
            }

            Glob.GetInstance().DebugString("Property with name '" + name + "' does not exist in GraphData '" + GetFileName() + "'. Check if you spelled the property name correctly (case-sensitive).", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Error);
            return null;
        }
        /// <summary>
        /// Get the value of a saved property with name.
        /// </summary>
        public T GetPropertyValue<T>(string name)
        {
            PropertyData<T> property = GetBlackboardProperty<T>(name);
            if (property != null)
            {
                return property.PropertyValue;
            }

            return default(T);
        }
        /// <summary>
        /// Set the value of a saved property. Use this if you want to change the behaviour of your graph before running it.
        /// <para>This directly changes the saved GraphData, and these changes are therefore persistent. This works similar to manually changing the value of a property in the blackboard.</para>
        /// Example usage: The player unlocked an achievement, as a reward you toggle property 'SpawnSecretRooms' to true. This property changes the flow of the graph, and addition room types will now spawn whenever you run the graph.
        /// </summary>
        public void SetPropertyValue<T>(string name, T value)
        {
            PropertyData<T> property = GetBlackboardProperty<T>(name);

            if (property != null)
            {
                property.PropertyValue = value;
            }
        }

        /// <summary>
        /// Save all the data from this GraphData object into a save file.
        /// </summary>
        /// <param name="saveName">The name of the save file.</param>
        public void SaveSerializedData(string saveName = "")
        {
            if (string.IsNullOrEmpty(saveName))
            {
                saveName = GetFileName();
            }

            string fileName = Application.persistentDataPath + "/" + saveName + ".dat";

            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(fileName);

            bf.Serialize(file, myData);
            file.Close();

            Glob.GetInstance().DebugString("Game data saved at " + fileName + ".", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Default);
        }

        private SerializableGraphData loadSerializableData(string saveName = "")
        {
            if (string.IsNullOrEmpty(saveName))
            {
                saveName = GetFileName();
            }

            string fileName = Application.persistentDataPath + "/" + saveName + ".dat";

            //Does the file exist
            if (File.Exists(fileName))
            {
                //Prepare for loading
                BinaryFormatter bf = new BinaryFormatter();
                //Open the file
                FileStream file = File.Open(fileName, FileMode.Open);

                //Load the data into a new dat object
                SerializableGraphData loadedData = (SerializableGraphData)bf.Deserialize(file);

                file.Close();

                Glob.GetInstance().DebugString("Game data succesfully loaded from " + fileName, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Default);

                return loadedData;
            }
            else
            {
                Glob.GetInstance().DebugString("No save data found at " + fileName, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                return null;
            }
        }

        /// <summary>
        /// Load all the data from a save file into this GraphData object. 
        /// This is useful when you added/removed nodes from your graph, and are loading an old save file where those changes are not applied.
        /// </summary>
        /// <param name="saveName">The name of the save file to load the properties from.</param>
        public GraphData LoadSerializedData(string saveName = "")
        {
            SerializableGraphData loadedData = loadSerializableData(saveName);

            //Copy the loaded data into this GraphData
            myData.CopyData(loadedData);

            if (ToolVersion != Glob.GetInstance().ToolVersion)
            {
                Glob.GetInstance().DebugString("Loaded GraphData was created in TerraTiler2D version '" + Glob.GetInstance().GetToolVersionAsString(ToolVersion) + "', which is different from the currently installed version '" + Glob.GetInstance().GetToolVersionAsString(Glob.GetInstance().ToolVersion) + "'. Attempting to update the GraphData to the required version.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                if (!GraphDataToolVersionManager.GetInstance().UpdateGraphData(this))
                {
                    return null;
                }
            }

            return this;
        }

        /// <summary>
        /// Load all the property information from a save file into this GraphData object.
        /// This is useful when you added/removed nodes from your graph, and are loading an old save file where those changes are not applied.
        /// When loading data (both in the Editor and in builds), the changes will persist until the application is closed. Upon restarting the application, the GraphData will be back to their initial state.
        /// </summary>
        /// <param name="saveName">The name of the save file to load the properties from.</param>
        public GraphData LoadSerializedPropertyData(string saveName = "")
        {
            SerializableGraphData loadedData = loadSerializableData(saveName);

            if (loadedData == null)
            {
                return this;
            }

            //If the loaded data was created in a different version of TerraTiler2D
            if (loadedData.TerraTiler2DVersion != Glob.GetInstance().ToolVersion)
            {
                Glob.GetInstance().DebugString("Loaded GraphData was created in TerraTiler2D version '" + Glob.GetInstance().GetToolVersionAsString(loadedData.TerraTiler2DVersion) + "', which is different from the currently installed version '" + Glob.GetInstance().GetToolVersionAsString(Glob.GetInstance().ToolVersion) + "'. Attempting to update the GraphData to the required version.", Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);

                //Create a copy of the loaded data
                GraphData tempCopy = new GraphData();
                tempCopy.SetGraphDataVersion(loadedData.Version);
                tempCopy.SetGUID(loadedData.GUID);
                tempCopy.GetData().CopyData(loadedData);

                //If the loaded data was succesfully updated to the currently installed version of TerraTiler2D
                if (GraphDataToolVersionManager.GetInstance().UpdateGraphData(tempCopy))
                {
                    //Copy the updated data into this GraphData
                    myData.CopyPropertyData(tempCopy.GetData());
                }
            }
            //If the loaded data was created in the same TerraTiler2D version as currently installed
            else
            {
                //Copy the loaded data into this GraphData
                myData.CopyPropertyData(loadedData);
            }

            return this;
        }
    }
}
