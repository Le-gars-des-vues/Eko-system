using UnityEngine;

namespace TerraTiler2D
{
    public class BasicWorldInstantiator : MonoBehaviour
    {
        public GraphData graphData;

        // Start is called before the first frame update
        void Start()
        {
            GraphRunner.GetInstance().RunGraphDuringRuntime(graphData, InstantiateWorld);
        }

        private void InstantiateWorld(GraphFinishedEvent evt)
        {
            WorldBuilder.GetInstance().PlaceTiles(evt.generatedOutput.GetProperty<World>("New World"), transform);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
