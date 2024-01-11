using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TerraTiler2D
{
    ///<summary>Instantiates <see cref="World"/> objects into the current scene as a <see cref="Tilemap"/>. Uses a basic tile placing algorithm, which should work for standard 2D worlds generated using TerraTiler2D. If your project requires a more advanced tile placing algorithm, you will have to write it yourself.</summary>
    public class WorldBuilder : Singleton<WorldBuilder>
    {
        private List<GameObject> worlds = new List<GameObject>();

        ///<summary>Places a <see cref="Tilemap"/> in the scene and fills it with <see cref="Tile"/> objects based on the data in a <see cref="World"/> object. Uses a basic tile placing algorithm, which should work for standard 2D worlds generated with TerraTiler2D. If your project requires a more advanced tile placing algorithm, you will have to write it yourself.</summary>
        ///<param name="world">The World object to instantiate.</param>
        ///<param name="parent">The Transform to parent the instantiated <see cref="World"/> to. The instantiated <see cref="World"/> will be centered on the parent Transform.</param>
        ///<param name="destroyExistingWorlds">Should all existing worlds be destroyed before instantiating this new one?</param>
        public GameObject PlaceTiles(World world, Transform parent, bool destroyExistingWorlds = true)
        {
            //Destroy any existing worlds.
            if (destroyExistingWorlds)
            {
                foreach (GameObject worldParent in worlds)
                {
                    GameObject.DestroyImmediate(worldParent);
                }
            }

            //Create an empty GameObject to parent all the Tiles to.
            GameObject newWorld = new GameObject("Instantiated world");

            var tileLayerEnum = world.GetTileLayers().GetEnumerator();

            while (tileLayerEnum.MoveNext())
            {
                newWorld.AddComponent<Grid>().cellSize = tileLayerEnum.Current.Value.Item1.tileSize * 0.01f;
                break;
            }

            //Add the world GameObject to a list, so we can delete it later.
            worlds.Add(newWorld);
            //Position the world gameobject
            newWorld.transform.parent = parent;
            newWorld.transform.localPosition = new Vector3(0, 0, 0);
            newWorld.transform.localRotation = Quaternion.identity;
            newWorld.transform.localScale = new Vector3(1, 1, 1);

            //Create an empty list of 2D Tile arrays. Every 2D Tile array holds the tiles from 1 TileLayer.
            List<TileBase[,]> worldTiles = new List<TileBase[,]>();

            //Get an enumerator for all the TileLayers in the World
            var tileLayerKeyEnumerator = world.GetTileLayers().Keys.GetEnumerator();

            //For every TileLayer
            while (tileLayerKeyEnumerator.MoveNext())
            {
                //Get the current TileLayer
                Tuple<TileLayer, Vector2, int> currentLayer = world.GetTileLayers()[tileLayerKeyEnumerator.Current];

                //Create a new empty gameobject for the current layer
                GameObject newLayer = new GameObject("Tile Layer " + tileLayerKeyEnumerator.Current);
                newLayer.transform.parent = newWorld.transform;
                newLayer.transform.localPosition = currentLayer.Item2 * (currentLayer.Item1.tileSize * 0.01f);
                newLayer.transform.localRotation = Quaternion.identity;
                newLayer.transform.localScale = new Vector3(1, 1, 1);
                //Item3 is the collisionLayer
                newLayer.layer = currentLayer.Item3;

                //Make the gameobject a Tilemap
                newLayer.AddComponent<TilemapRenderer>().sortingOrder = tileLayerKeyEnumerator.Current;
                Tilemap currentTilemap = newLayer.GetComponent<Tilemap>();

                //Set the size of the Tilemap
                currentTilemap.size = new Vector3Int(currentLayer.Item1.generatedTiles.GetLength(0), currentLayer.Item1.generatedTiles.GetLength(1), 0);

                //Create an empty TileBase[,] with the size of the TileLayer.
                TileBase[,] tiles = new TileBase[currentLayer.Item1.generatedTiles.GetLength(0), currentLayer.Item1.generatedTiles.GetLength(1)];

                //For every column of Tiles in the TileLayer
                for (int i = 0; i < currentLayer.Item1.generatedTiles.GetLength(0); i++)
                {
                    //For ever row of Tiles in the TileLayer
                    for (int j = 0; j < currentLayer.Item1.generatedTiles.GetLength(1); j++)
                    {
                        //If there is a Tile at the index
                        if (world.GetTileByIndex(currentLayer.Item1.generatedTiles[i, j]) != null)
                        {
                            //Instantiate a Tile of the type that matches the tileIndex at the current column and row, and parent it to the world gameobject.
                            TileBase newTile = world.GetTileByIndex(currentLayer.Item1.generatedTiles[i, j]);

                            currentTilemap.SetTile(new Vector3Int(i, j, 0), newTile);

                            //Add the tile to the return array.
                            tiles[i, j] = newTile;
                        }
                    }
                }

                newLayer.AddComponent<TilemapCollider2D>();

                //Add the 2D Tile array to the list
                worldTiles.Add(tiles);
            }

            //Return the tile array
            return newWorld;
        }
    }
}
