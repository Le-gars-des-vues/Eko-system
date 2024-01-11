using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Tilemaps;
#if (UNITY_EDITOR)
using UnityEditor;
#endif

namespace TerraTiler2D
{
    public class World : ICloneable
    {
        private Dictionary<int, Tuple<TileLayer, Vector2, int>> layers = new Dictionary<int, Tuple<TileLayer, Vector2, int>>();

        private Dictionary<int, TileBase> tileIndexes = new Dictionary<int, TileBase>();

        private Vector2 worldSize = new Vector2(0, 0);
        private Vector2 tileSize = new Vector2(Glob.GetInstance().InvalidTileIndex, Glob.GetInstance().InvalidTileIndex);
        private Rect worldBounds = new Rect(0, 0, 0, 0);

        public World()
        {
            addTileIndex(Glob.GetInstance().DefaultNullTileIndex, null);
        }
        public World(World copyFrom)
        {
            //Copy all the TileLayers from the other World
            var tileLayerEnumerator = copyFrom.layers.GetEnumerator();
            while (tileLayerEnumerator.MoveNext())
            {
                layers.Add(tileLayerEnumerator.Current.Key, tileLayerEnumerator.Current.Value);
            }

            //Copy all the tile indexes from the other World
            var tileIndexEnumerator = copyFrom.tileIndexes.GetEnumerator();
            while (tileIndexEnumerator.MoveNext())
            {
                tileIndexes.Add(tileIndexEnumerator.Current.Key, tileIndexEnumerator.Current.Value);
            }

            //Copy the world size from the other World
            worldSize = copyFrom.worldSize;
            worldBounds = copyFrom.worldBounds;
        }

        public Dictionary<int, Tuple<TileLayer, Vector2, int>> GetTileLayers()
        {
            updateTileLayerTileIndexes();

            return layers;
        }

        public Dictionary<int, TileBase> GetTileIndexes()
        {
            return tileIndexes;
        }

        public TileLayer AddTileLayer(TileLayer tileLayer, Vector2 position, int zIndex, int collisionLayer = 0)
        {
            if (layers.ContainsKey(zIndex))
            {
                Glob.GetInstance().DebugString("World already contains a TileLayer with Z index " + zIndex + ". The new TileLayer will not be added.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);

                return tileLayer;
            }

            //If the added layer does not have the same tileSize as the world
            if (tileLayer.tileSize.x != tileSize.x || tileLayer.tileSize.y != tileSize.y)
            {
                //If the world does not have a defined tileSize yet.
                if (tileSize.x == Glob.GetInstance().InvalidTileIndex || tileSize.y == Glob.GetInstance().InvalidTileIndex)
                {
                    tileSize = tileLayer.tileSize;
                }
                //If the world does have a defined tileSize
                else
                {
                    //Warn the user, and don't add the tileLayer
                    Glob.GetInstance().DebugString("Tried to add a TileLayer with TileSize '" + tileLayer.tileSize + "' to World, but the World uses TileSize '" + tileSize + "'. The TileLayer has not been added.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                    return tileLayer;
                }
            }

            layers.Add(zIndex, new Tuple<TileLayer, Vector2, int>(tileLayer, position, collisionLayer));

            addTileIndexes(tileLayer);

            //Recalculate the bounds of the World
            if (position.x < worldBounds.xMin)
            {
                worldBounds.xMin = position.x;
            }
            if (position.x + tileLayer.generatedTiles.GetLength(0) > worldBounds.xMax)
            {
                worldBounds.xMax = position.x + tileLayer.generatedTiles.GetLength(0);
            }
            if (position.y < worldBounds.yMin)
            {
                worldBounds.yMin = position.y;
            }
            if (position.y + tileLayer.generatedTiles.GetLength(1) > worldBounds.yMax)
            {
                worldBounds.yMax = position.y + tileLayer.generatedTiles.GetLength(1);
            }

            worldSize = worldBounds.size;

            return tileLayer;
        }
        private void addTileIndexes(TileLayer tileLayer)
        {
            var tileIndexEnumerator = tileLayer.GetTileIndexDictionary().GetEnumerator();

            //For every Tile in the TileLayer
            while (tileIndexEnumerator.MoveNext())
            {
                //If the World tileIndex dictionary does not contain the Tile yet
                if (!tileIndexes.ContainsValue(tileIndexEnumerator.Current.Value))
                {
                    //Add the Tile to the dictionary with a unique index
                    addTileIndex(getUniqueTileIndex(), tileIndexEnumerator.Current.Value);
                }
            }

            //Update the indexes of all TileLayers to match the indexes of this World
            updateTileLayerTileIndexes();
        }

        private int addTileIndex(int index, TileBase tile)
        {
            if (tileIndexes.ContainsKey(index))
            {
                if (tileIndexes[index] != tile)
                {
                    Glob.GetInstance().DebugString("Tile index " + index + " was already in the dictionary. Removing tile " + tileIndexes[index].name + ", and replacing it with tile " + tile.name + ". If this was not intended, please make sure the tiles have a different Tile Index.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                }
                tileIndexes.Remove(index);
            }

            tileIndexes.Add(index, tile);

            return index;
        }
        public TileBase GetTileByIndex(int index)
        {
            if (tileIndexes.ContainsKey(index))
            {
                return tileIndexes[index];
            }

            return null;
        }
        public int GetIndexByTile(TileBase tile)
        {
            if (tileIndexes.ContainsValue(tile))
            {
                var test = tileIndexes.GetEnumerator();

                while (test.MoveNext())
                {
                    if (test.Current.Value == tile)
                    {
                        return test.Current.Key;
                    }
                }
            }

            Glob.GetInstance().DebugString("Tile " + tile.name + " is not in the dictionary. Returning the default null tile index.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
            return -1;
        }
        public Dictionary<int, TileBase> GetTileIndexDictionary()
        {
            return tileIndexes;
        }
        private void updateTileLayerTileIndexes()
        {
            var tileLayerEnumerator = layers.Values.GetEnumerator();

            while (tileLayerEnumerator.MoveNext())
            {
                tileLayerEnumerator.Current.Item1.UpdateTileIndexes(tileIndexes);
            }
        }
        private int getUniqueTileIndex(int iter = 0)
        {
            if (tileIndexes.ContainsKey(iter))
            {
                iter += 1;
                return getUniqueTileIndex(iter);
            }
            return iter;
        }

#if (UNITY_EDITOR)
        public void CreateCSVFile(string fileName)
        {
            string csvLayers = "";
            var tileLayerKeyEnumerator = layers.Keys.GetEnumerator();

            while (tileLayerKeyEnumerator.MoveNext())
            {
                csvLayers += convertTileLayerToCSV(layers[tileLayerKeyEnumerator.Current]);
            }

            //The default save location is in the resources folder.
            string assetPath = Glob.GetInstance().GetDefaultGraphSavePath() + fileName + ".txt";

            //Get all the assets with the same name and type as this graph.
            string[] assetPaths = AssetDatabase.FindAssets(fileName + " t:GraphData");
            //If we found at least one, save the asset at that location instead of in the resources folder.
            if (assetPaths.Length > 0)
            {
                //Get the path to the TT_GraphData scriptableobject.
                assetPath = AssetDatabase.GUIDToAssetPath(assetPaths[0]);
                //Remove file extension
                assetPath = assetPath.Remove(assetPath.LastIndexOf('.'));
                //Add new file extension
                assetPath += ".txt";

                //Dont overwrite previously saved worlds.
                assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            }
            else
            {
                Glob.GetInstance().DebugString("Something went wrong with finding the file path to the existing GraphData file. To prevent data loss, the graph has been saved at: " + assetPath, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
            }

            System.IO.File.WriteAllText(assetPath, csvLayers);
            Glob.GetInstance().DebugString("World has been saved at: " + assetPath, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Default);
        }
#endif

        private string convertTileLayerToCSV(Tuple<TileLayer, Vector2, int> layer)
        {
            string csvTiles = "";

            csvTiles += "Position: " + layer.Item2 + ";\n";
            csvTiles += "CollisionLayer: " + layer.Item3 + ";\n";

            for (int j = 0; j < layer.Item1.generatedTiles.GetLength(1); j++)
            {
                for (int i = 0; i < layer.Item1.generatedTiles.GetLength(0); i++)
                {
                    csvTiles += layer.Item1.generatedTiles[i, j] + ",";
                }
                csvTiles += ";\n";
            }

            csvTiles += "\n\n";

            return csvTiles;
        }

        private int[,] GetMergedLayerTiles()
        {
            //Create a new empty 2D array to hold the tile indexes
            int[,] mergedLayerTiles = new int[(int)worldSize.x, (int)worldSize.y];
            for (int x = 0; x < mergedLayerTiles.GetLength(0); x++)
            {
                for (int y = 0; y < mergedLayerTiles.GetLength(1); y++)
                {
                    //Set the entire array to empty tiles
                    mergedLayerTiles[x, y] = Glob.GetInstance().DefaultNullTileIndex;
                }
            }

            int layerIndexIter = 0;
            int layerHandledCount = 0;
            Tuple<TileLayer, Vector2, int> currentLayer;

            while (layerHandledCount < layers.Count)
            {
                if (layers.TryGetValue(layerIndexIter, out currentLayer))
                {
                    //Calculate the starting index of this layer's tiles in the merged array, based on the distance from the negative corner of the WorldBounds
                    Vector2 layerStartIndex = new Vector2(currentLayer.Item2.x - (int)worldBounds.xMin, currentLayer.Item2.y - (int)worldBounds.yMin);

                    //For every row of tiles in the tile layer
                    for (int x = 0; x < currentLayer.Item1.generatedTiles.GetLength(0); x++)
                    {
                        //For every column of tiles in the tile layer
                        for (int y = 0; y < currentLayer.Item1.generatedTiles.GetLength(1); y++)
                        {
                            //If there is a tile in the current layer, overwrite the tile in the merged 2D array (Higher Z index = in front)
                            if (currentLayer.Item1.generatedTiles[x, y] != Glob.GetInstance().DefaultNullTileIndex)
                            {
                                //Add the tile index to the merged 2D array
                                mergedLayerTiles[(int)layerStartIndex.x + x, (int)layerStartIndex.y + y] = currentLayer.Item1.generatedTiles[x, y];
                            }
                        }
                    }

                    layerHandledCount++;
                }

                layerIndexIter++;
            }

            return mergedLayerTiles;
        }
        private Dictionary<int, Color[]> GetTileIndexColors(int sizeX, int sizeY)
        {
            //Create a new dictionary to fetch our colors from, so we don't have to recalculate the colors for every individual tile.
            Dictionary<int, Color[]> colorIndexDictionary = new Dictionary<int, Color[]>();

            //Enumerate over the actual tile index dictionary.
            var tileIndexEnumerator = tileIndexes.GetEnumerator();

            //For every type of tile that gets generated in the world, store their texture pixels in our color index dictionary.
            while (tileIndexEnumerator.MoveNext())
            {
                //If there is no tile in the value, or if the tile has no renderer
                if (tileIndexEnumerator.Current.Value == null || Glob.GetInstance().GetTileGraphPreview(tileIndexEnumerator.Current.Value, sizeX, sizeY) == null)
                {
                    //Put an empty block of pixels in the dictionary.
                    colorIndexDictionary.Add(tileIndexEnumerator.Current.Key, new Color[sizeX * sizeY]);
                    continue;
                }

                //Get a block of pixels to represent the tile in previews.
                Color[] tileColors = Glob.GetInstance().GetTileGraphPreview(tileIndexEnumerator.Current.Value, sizeX, sizeY);

                //Put the pixels of the tile texture in the dictionary.
                colorIndexDictionary.Add(tileIndexEnumerator.Current.Key, tileColors);
            }

            return colorIndexDictionary;
        }

        public Texture2D GetWorldPreviewTexture()
        {
            //If this world contains no layers, or the tileSize has not been defined yet
            if (layers.Count <= 0 || tileSize.x == Glob.GetInstance().InvalidTileIndex || tileSize.y == Glob.GetInstance().InvalidTileIndex)
            {
                //Create an empty texture2D of the default preview size.
                return new Texture2D((int)Glob.GetInstance().DefaultPreviewSize.x, (int)Glob.GetInstance().DefaultPreviewSize.y);
            }

            int textureWidth = (int)worldSize.x * (int)tileSize.x;
            int textureHeight = (int)worldSize.y * (int)tileSize.y;

            int colorBlockSize = Mathf.FloorToInt(Mathf.Min(tileSize.x, tileSize.y));

            //If the texture is going to be too big, preview all the tiles as a single pixel
            if (textureWidth > Glob.GetInstance().DefaultPreviewSize.x || textureHeight > Glob.GetInstance().DefaultPreviewSize.y)
            {
                //Instead of 'tileSize', use a smaller block of pixels per tile
                colorBlockSize = Mathf.Min((int)Mathf.Max(1, Glob.GetInstance().DefaultPreviewSize.x / (int)worldSize.x), (int)Mathf.Max(1, Glob.GetInstance().DefaultPreviewSize.y / (int)worldSize.y));

                textureWidth = (int)worldSize.x * colorBlockSize;
                textureHeight = (int)worldSize.y * colorBlockSize;

                Glob.GetInstance().DebugString("The preview for world " + this + " will be too large, so the amount of pixels per tile has been reduced to " + colorBlockSize + ". These pixels will be taken from the top left of the tile.", Glob.DebugCategories.Node, Glob.DebugLevel.Low, Glob.DebugTypes.Warning);
            }

            //Create an empty texture with the size of the generated world, multiplied by the tile size.
            Texture2D previewTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, true);
            
            //Prepare a 2D array to merge the layers into
            int[,] mergedLayerTiles = GetMergedLayerTiles();

            //Create a new dictionary to fetch our colors from, so we don't have to recalculate the colors for every individual tile.
            Dictionary<int, Color[]> colorIndexDictionary = GetTileIndexColors(colorBlockSize, colorBlockSize);

            //For every tile in the world
            for (int x = 0; x < (int)worldSize.x; x++)
            {
                for (int y = 0; y < (int)worldSize.y; y++)
                {
                    //Set a block of tileSize pixels in our texture to the pixels of the associated tile.
                    previewTexture.SetPixels(x * colorBlockSize, y * colorBlockSize, colorBlockSize, colorBlockSize, colorIndexDictionary[mergedLayerTiles[x, y]]);
                }
            }

            //Apply the pixels to the preview texture.
            previewTexture.Apply();

            return previewTexture;
        }

        public object Clone()
        {
            return new World(this);
        }
    }
}
