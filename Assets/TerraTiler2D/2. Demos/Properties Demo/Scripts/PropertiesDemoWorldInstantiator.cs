using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace TerraTiler2D
{
    public class PropertiesDemoWorldInstantiator : MonoBehaviour
    {
        public GraphData graphData;
        private GraphData graphDataInstance;

        private string graphSaveName = "PropertiesDemo";

        private string worldSizePropertyName = "World Size";
        private string buildingAmountPropertyName = "City Amount";
        private string connectBuildingsPropertyName = "Connect Cities";
        private string outputExamplePropertyName = "Output Example";

        public UnityEvent<float> OnLoadGraphVector2;
        public UnityEvent<float> OnLoadGraphInt;
        public UnityEvent<bool> OnLoadGraphBool;
        public UnityEvent<float> OnLoadGraphFloat;

        void Start()
        {
            graphData.SaveSerializedData();
            Debug.Log("Saved the default graph data.");

            //It is best to create a copy of your GraphData object, otherwise you would be directly editing the ScriptableObject asset, which may cause unintended persistent behaviour.
            graphDataInstance = Instantiate(graphData);

            InstantiateWorld();
        }

        private void InstantiateWorld()
        {
            Vector2 WorldSize = graphDataInstance.GetPropertyValue<Serializable.Vector2>(worldSizePropertyName);
            int BuildingAmount = graphDataInstance.GetPropertyValue<int>(buildingAmountPropertyName);
            bool ConnectBuildings = graphDataInstance.GetPropertyValue<bool>(connectBuildingsPropertyName);
            float OutputExample = graphDataInstance.GetPropertyValue<float>(outputExamplePropertyName);

            Debug.Log("\n===== Running the graph =====" + "\n");

            Debug.Log("--- Input property values ---");
            Debug.Log("\t\t" + worldSizePropertyName + ": " + WorldSize);
            Debug.Log("\t\t" + buildingAmountPropertyName + ": " + BuildingAmount);
            Debug.Log("\t\t" + connectBuildingsPropertyName + ": " + ConnectBuildings);
            Debug.Log("\t\t" + outputExamplePropertyName + ": " + OutputExample + "\n");

            Debug.Log("----- Graph starting -----");
            GraphRunner.GetInstance().RunGraphDuringRuntime(graphDataInstance, HandleGraphFinished);

        }

        private void HandleGraphFinished(GraphFinishedEvent evt)
        {
            Debug.Log("----- Graph finished -----" + "\n");

            Debug.Log("--- Output property values ---");
            Debug.Log("\t\t" + worldSizePropertyName + ": " + evt.generatedOutput.GetProperty<Vector2>(worldSizePropertyName));
            Debug.Log("\t\t" + buildingAmountPropertyName + ": " + evt.generatedOutput.GetProperty<int>(buildingAmountPropertyName));
            Debug.Log("\t\t" + connectBuildingsPropertyName + ": " + evt.generatedOutput.GetProperty<bool>(connectBuildingsPropertyName));
            Debug.Log("\t\t" + outputExamplePropertyName + ": " + evt.generatedOutput.GetProperty<float>(outputExamplePropertyName) + "\n");

            Debug.Log("===== Placing tiles =====" + "\n");
            WorldBuilder.GetInstance().PlaceTiles(evt.generatedOutput.GetProperty<World>("Map"), transform);
        }

        private void SetProperty<T>(string property, T value)
        {
            Debug.Log("Setting property '" + property + "' to " + value);

            graphDataInstance.SetPropertyValue<T>(property, value);
        }

        public void RunGraph()
        {
            InstantiateWorld();
        }

        public void SetWorldSize(float newValue)
        {
            SetProperty<Serializable.Vector2>(worldSizePropertyName, new Vector2(newValue, newValue));
        }

        public void SetBuildingAmount(float newValue)
        {
            SetProperty<int>(buildingAmountPropertyName, (int)newValue);
        }

        public void SetConnectBuildings(bool newValue)
        {
            SetProperty<bool>(connectBuildingsPropertyName, newValue);
        }

        public void SetOutputExample(float newValue)
        {
            newValue *= 100;
            newValue = Mathf.Round(newValue);
            newValue /= 100;

            SetProperty<float>(outputExamplePropertyName, newValue);
        }

        public void SetSaveName(string newName)
        {
            if (!string.IsNullOrEmpty(newName))
            {
                graphSaveName = newName;
            }
        }

        public void SaveGraph()
        {
            Debug.Log("\n===== Saving graph data =====");
            graphDataInstance.SaveSerializedData(graphSaveName);
        }

        public void LoadGraph()
        {
            Debug.Log("\n===== Loading saved graph data =====");
            graphDataInstance.LoadSerializedPropertyData(graphSaveName);

            if (graphDataInstance.ToolVersion == Glob.GetInstance().ToolVersion)
            {
                OnLoadGraphVector2.Invoke(graphDataInstance.GetPropertyValue<Serializable.Vector2>(worldSizePropertyName).x);
                OnLoadGraphInt.Invoke(graphDataInstance.GetPropertyValue<int>(buildingAmountPropertyName));
                OnLoadGraphBool.Invoke(graphDataInstance.GetPropertyValue<bool>(connectBuildingsPropertyName));
                OnLoadGraphFloat.Invoke(graphDataInstance.GetPropertyValue<float>(outputExamplePropertyName));
            }
            else
            {
                LoadDefaultGraph();
            }
        }

        public void LoadDefaultGraph()
        {
            Debug.Log("\n===== Loading default data =====");
            graphDataInstance.LoadSerializedPropertyData();

            OnLoadGraphVector2.Invoke(graphDataInstance.GetPropertyValue<Serializable.Vector2>(worldSizePropertyName).x);
            OnLoadGraphInt.Invoke(graphDataInstance.GetPropertyValue<int>(buildingAmountPropertyName));
            OnLoadGraphBool.Invoke(graphDataInstance.GetPropertyValue<bool>(connectBuildingsPropertyName));
            OnLoadGraphFloat.Invoke(graphDataInstance.GetPropertyValue<float>(outputExamplePropertyName));
        }

        public void QuitGame()
        {
            Application.Quit();

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }
    }
}
