using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TerraTiler2D
{
    public class Demo_LevelManager : MonoBehaviour
    {
        public GraphData graphData;

        public Demo_CharacterController playerPrefab;
        private Demo_CharacterController player;

        private Transform worldParent;
        private Vector2 worldSize;
        private Vector2 tileSize;

        private Vector3 spawnPoint;

        private bool isGeneratingWorld = false;

        // Start is called before the first frame update
        void Start()
        {
            //Instantiate the player prefab
            player = Instantiate(playerPrefab, transform);
            //Raise an event telling listeners that the player has been spawned
            EventManager.GetInstance().RaiseEvent(new Demo_OnPlayerInstantiated().Init(player));

            GenerateWorld();
        }

        private void GenerateWorld()
        {
            GraphRunner.GetInstance().RunGraphDuringRuntime(graphData, InstantiateWorld);
            isGeneratingWorld = true;
        }

        private void InstantiateWorld(GraphFinishedEvent evt = null)
        {
            //The 'GraphData' ScriptableObject holds all the data about the nodes that were placed in the graph in the TerraTiler2D editor window.
            //By passing GraphData into the GraphRunner singleton, we can execute the graph just like when pressing the 'Run Graph' button in the editor window.
            //The GraphRunner will return a 'GraphOutput' variable containing all of the processed properties of the graph.
            GraphOutput output = evt.generatedOutput;
            isGeneratingWorld = false;

            //This GraphOutput contains a property of the type 'World' with the name "New World", which we fetch using the 'GetProperty' method.
            //The World object contains TileLayers, which in turn each contain a 2D array of Tiles. 
            //By passing the World object into the WorldBuilder instance, all of the Tiles in each of the TileLayers will be instantiated into the scene.
            GameObject tiles = WorldBuilder.GetInstance().PlaceTiles(output.GetProperty<World>("New World"), transform);

            //The WorldBuilder singleton will not parent the individual tiles directly to this transform, but rather to an empty game object, which in turn gets parented to this transform.
            //To find the empty game object containing all the tiles, these nested for loops will iterate over all of the tiles until a valid one is found, and the parent object is stored.
            worldParent = tiles.transform;

            worldSize = output.GetProperty<Vector2>("World Size");
            tileSize = output.GetProperty<Vector2>("Tile Size") * 0.01f;
            spawnPoint = output.GetProperty<Vector2>("Spawn Point");

            Vector3 worldOffset = player.transform.position - spawnPoint;

            //Place the world below the player, so that it seems like they fall down upon it.
            worldParent.position = worldParent.position + worldOffset;
        }

        // Update is called once per frame
        void Update()
        {
            if (!isGeneratingWorld)
            {
                if (player != null && worldParent != null && worldSize != null && tileSize != null)
                {
                    //If the player has fallen far below the currently instantiated world.
                    if (player.transform.position.y <= worldParent.position.y - (worldSize.y * tileSize.y))
                    {
                        //Instantiate a new world
                        GenerateWorld();
                    }
                }
            }
        }
    }
}