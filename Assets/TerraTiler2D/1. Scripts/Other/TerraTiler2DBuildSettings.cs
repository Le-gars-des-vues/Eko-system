using System;
using UnityEngine;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

namespace TerraTiler2D
{
    [Serializable]
    public class TerraTiler2DBuildSettings : ScriptableObject
    {
        //NOTE: GetInstance() gets called from the Glob.GetInstance(), which means we can not reference the Glob within this GetInstance() method.
#if (UNITY_EDITOR)
        private static string ParentFolder = "Assets/Resources/";
#endif
        private static string SettingsFolderPath = "TerraTiler2D/Editor/Settings";
        private static string SettingsName = "TerraTiler2DBuildSettings";

        private static TerraTiler2DBuildSettings Instance;

        public static TerraTiler2DBuildSettings GetInstance()
        {
            if (Instance == null)
            {
#if (UNITY_EDITOR)
                if (!AssetDatabase.IsValidFolder(ParentFolder + SettingsFolderPath))
                {
                    return null;
                }
#endif

                //Try to load the settings
                Instance = Resources.Load<TerraTiler2DBuildSettings>(SettingsFolderPath + "/" + SettingsName);
#if (UNITY_EDITOR)
                //If no settings object has been created yet
                if (Instance == null)
                {
                    //Create a new settings object.
                    AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<TerraTiler2DBuildSettings>(), ParentFolder + SettingsFolderPath + "/" + SettingsName + ".asset");

                    //Store the reference in Instance
                    Instance = AssetDatabase.LoadAssetAtPath<TerraTiler2DBuildSettings>(ParentFolder + SettingsFolderPath + "/" + SettingsName + ".asset");

                    AssetDatabase.Refresh();
                    EditorUtility.SetDirty(Instance);
                    AssetDatabase.SaveAssets();
                }
#endif
            }

            return Instance;
        }

        [HideInInspector]
        [SerializeField]
        private Glob.DebugLevel ActiveDebugLevel = Glob.DebugLevel.User;
        [HideInInspector]
        [SerializeField]
        private bool PauseBetweenNodes = true;

        public Glob.DebugLevel GetDebugLevel()
        {
            if (!Enum.IsDefined(typeof(Glob.DebugLevel), ActiveDebugLevel))
            {
                ActiveDebugLevel = Glob.GetInstance().ActiveDebugLevel;
            }

            return ActiveDebugLevel;
        }

        public void SetDebugLevel(Glob.DebugLevel level)
        {
            ActiveDebugLevel = level;
            handleSettingChanged();
        }

        public bool GetPauseBetweenNodes()
        {
            if (bool.Equals(PauseBetweenNodes, null))
            {
                PauseBetweenNodes = Glob.GetInstance().PauseBetweenNodes;
            }

            return PauseBetweenNodes;
        }

        public void SetPauseBetweenNodes(bool toggle)
        {
            PauseBetweenNodes = toggle;
            handleSettingChanged();
        }

        private void handleSettingChanged()
        {
#if (UNITY_EDITOR)
            AssetDatabase.Refresh();
            EditorUtility.SetDirty(GetInstance());
            AssetDatabase.SaveAssets();
#endif
        }
    }
}