#if (UNITY_EDITOR)
using TerraTiler2D;
using UnityEditor;

[CustomEditor(typeof(TerraTiler2DBuildSettings))]
[CanEditMultipleObjects]
public class TerraTiler2DBuildSettingsEditor : Editor
{
    SerializedProperty debugLevel;
    SerializedProperty pauseBetweenNodes;

    void OnEnable()
    {
        debugLevel = serializedObject.FindProperty("ActiveDebugLevel");
        pauseBetweenNodes = serializedObject.FindProperty("PauseBetweenNodes");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.LabelField("This ScriptableObject is only used to access relevant settings in a build.");
        EditorGUILayout.LabelField("Use the 'Tools/TerraTiler2D/Settings' menu in the toolbar to edit settings.");

        EditorGUILayout.Space(30);

        EditorGUILayout.LabelField("===== Build Settings =====");
        EditorGUILayout.LabelField("Debug Level: " + (Glob.DebugLevel)debugLevel.enumValueIndex);
        EditorGUILayout.LabelField("Pause Between Nodes: " + pauseBetweenNodes.boolValue);
    }
}
#endif