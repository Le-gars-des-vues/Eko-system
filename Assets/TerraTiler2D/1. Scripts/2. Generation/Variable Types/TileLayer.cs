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
    public class TileLayer : ICloneable
    {
        public int[,] generatedTiles;

        private Vector2 layerSize = new Vector2(100, 100);
        public Vector2 tileSize = new Vector2(16, 16);

        private Dictionary<int, TileBase> tileIndexes = new Dictionary<int, TileBase>();

        //When swapping tile indexes, we need to first change all of them to an index of which we are sure it will not be in use. After that, we can safely assign each tile a normal index without any chance of overwriting another tile's index.
        private int tileIndexUpdateOffset = 9999999;

        public TileLayer(Vector2 layerSize, Vector2 tileSize)
        {
            if (layerSize != Vector2.zero)
            {
                this.layerSize = layerSize;
            }
            if (tileSize != Vector2.zero)
            {
                this.tileSize = tileSize;
            }

            if (this.layerSize.x > Glob.GetInstance().MaxWorldSize || this.layerSize.y > Glob.GetInstance().MaxWorldSize)
            {
                Glob.GetInstance().DebugString("The max size of a TileLayer is currently capped at " + Glob.GetInstance().MaxWorldSize + "x" + Glob.GetInstance().MaxWorldSize + " tiles. This is to prevent the Unity Editor from freezing/crashing due to the high performance requirement of bigger worlds. You can increase this cap in the Glob script if you do not care about potential crashes. I will do my best to improve the performance of TerraTiler2D in future updates, so this cap should gradually increase.", Glob.DebugCategories.Misc, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
            }

            this.layerSize.x = Mathf.Clamp(this.layerSize.x, 0, Glob.GetInstance().MaxWorldSize);
            this.layerSize.y = Mathf.Clamp(this.layerSize.y, 0, Glob.GetInstance().MaxWorldSize);

            generatedTiles = new int[(int)this.layerSize.x, (int)this.layerSize.y];
            for (int x = 0; x < generatedTiles.GetLength(0); x++)
            {
                for (int y = 0; y < generatedTiles.GetLength(1); y++)
                {
                    generatedTiles[x, y] = Glob.GetInstance().DefaultNullTileIndex;
                }
            }

            AddTileIndex(Glob.GetInstance().DefaultNullTileIndex, null);
        }

        public TileLayer(TileLayer other)
        {
            this.layerSize = other.layerSize;
            
            this.tileSize = other.tileSize;

            this.generatedTiles = (int[,])other.generatedTiles.Clone();

            //Copy all the tile indexes from the other World
            var tileIndexEnumerator = other.tileIndexes.GetEnumerator();
            while (tileIndexEnumerator.MoveNext())
            {
                tileIndexes.Add(tileIndexEnumerator.Current.Key, tileIndexEnumerator.Current.Value);
            }
        }

        public int AddTileIndex(int index, TileBase tile)
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
        public int AddTileIndex(TileBase tile)
        {
            int index;

            //Tile is already in the dictionary
            if (tileIndexes.ContainsValue(tile))
            {
                return GetIndexByTile(tile);
            }

            //Get a new unique index
            index = getUniqueTileIndex();
            //Add the tile to the dictionary with the unique index
            tileIndexes.Add(index, tile);
            return index;
        }
        public TileBase GetTileByIndex(int index)
        {
            return tileIndexes[index];
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
        public void UpdateTileIndexes(Dictionary<int, TileBase> newTileIndexes)
        {
            Dictionary<int, int> updatedIndexes = new Dictionary<int, int>();

            var newTileIndexEnumerator = newTileIndexes.GetEnumerator();

            //For every new tile index
            while (newTileIndexEnumerator.MoveNext())
            {
                //If the current dictionary contains the same Tile as the new dictionary
                if (tileIndexes.ContainsValue(newTileIndexEnumerator.Current.Value))
                {
                    //Add the conversion from the old index to the new index to the dictionary
                    updatedIndexes.Add(GetIndexByTile(newTileIndexEnumerator.Current.Value), newTileIndexEnumerator.Current.Key);
                }
            }

            var updatedIndexesEnumerator = updatedIndexes.GetEnumerator();

            //For every tile index update
            while (updatedIndexesEnumerator.MoveNext())
            {
                //Convert all the generated tiles from the old index to the new index
                convertTileIndex(updatedIndexesEnumerator.Current.Key, updatedIndexesEnumerator.Current.Value);
            }

            tileIndexes = newTileIndexes;

            finalizedUpdatedTileIndexes();
        }
        private void convertTileIndex(int from, int to)
        {
            //For every row
            for (int i = 0; i < generatedTiles.GetLength(0); i++)
            {
                //For every column
                for (int j = 0; j < generatedTiles.GetLength(1); j++)
                {
                    //If the tile at index is the tile index to convert
                    if (generatedTiles[i, j] == from)
                    {
                        //Convert the tile to the new index, and add an offset to prevent other index conversions from affecting this one.
                        generatedTiles[i, j] = to + tileIndexUpdateOffset;
                    }
                }
            }
        }
        private void finalizedUpdatedTileIndexes()
        {
            for (int i = 0; i < generatedTiles.GetLength(0); i++)
            {
                for (int j = 0; j < generatedTiles.GetLength(1); j++)
                {
                    if (generatedTiles[i, j] >= tileIndexUpdateOffset)
                    {
                        //Remove the offset from all converted tile indexes after all conversions are finished
                        generatedTiles[i, j] = generatedTiles[i, j] - tileIndexUpdateOffset;
                    }
                }
            }
        }

        //Start at index 1, since 0 is used for NULL tiles.
        private int getUniqueTileIndex(int iter = 1)
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
            string csvTiles = "";
            for (int j = 0; j < generatedTiles.GetLength(1); j++)
            {
                for (int i = 0; i < generatedTiles.GetLength(0); i++)
                {
                    csvTiles += generatedTiles[i, j] + ",";
                }
                csvTiles += ";\n";
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

            System.IO.File.WriteAllText(assetPath, csvTiles);
            Glob.GetInstance().DebugString("World has been saved at: " + assetPath, Glob.DebugCategories.Data, Glob.DebugLevel.User, Glob.DebugTypes.Default);
        }
#endif
        public object Clone()
        {
            return new TileLayer(this);
        }
        public bool Merge(TileLayer other, Vector2 relativePosition)
        {
            if (tileSize != other.tileSize)
            {
                Glob.GetInstance().DebugString("Tried to merge two TileLayers with a different tileSize. Aborting operation.", Glob.DebugCategories.Node, Glob.DebugLevel.User, Glob.DebugTypes.Warning);
                return false;
            }

            var tileIndexEnumerator = other.GetTileIndexDictionary().GetEnumerator();

            //For every Tile in the other TileLayer
            while (tileIndexEnumerator.MoveNext())
            {
                //If this tileIndex dictionary does not contain the Tile yet
                if (!tileIndexes.ContainsValue(tileIndexEnumerator.Current.Value))
                {
                    //Add the Tile to the dictionary with a unique index
                    AddTileIndex(getUniqueTileIndex(), tileIndexEnumerator.Current.Value);
                }
            }

            //Make sure both TileLayers use the same TileIndexes
            other.UpdateTileIndexes(this.tileIndexes);

            Rect myBounds = new Rect(0, 0, layerSize.x, layerSize.y);
            Rect otherBounds = new Rect(relativePosition.x, relativePosition.y, other.layerSize.x, other.layerSize.y);

            //If the relativePosition is negative, shift all the tiles over so they fall between 0 and layerSize
            if (relativePosition.x < 0)
            {
                myBounds.x = relativePosition.x * -1;
                otherBounds.x = 0;
            }
            if (relativePosition.y < 0)
            {
                myBounds.y = relativePosition.y * -1;
                otherBounds.y = 0;
            }

            Rect mergedBounds = new Rect(
                Mathf.Min(myBounds.xMin, otherBounds.xMin), 
                Mathf.Min(myBounds.yMin, otherBounds.yMin), 
                Mathf.Max(myBounds.xMax, otherBounds.xMax), 
                Mathf.Max(myBounds.yMax, otherBounds.yMax)
            );

            //Create a new empty 2D array to hold the tile indexes
            int[,] mergedLayerTiles = new int[(int)mergedBounds.width, (int)mergedBounds.height];
            for (int x = 0; x < mergedLayerTiles.GetLength(0); x++)
            {
                for (int y = 0; y < mergedLayerTiles.GetLength(1); y++)
                {
                    if (x >= myBounds.xMin && x < myBounds.xMax)
                    {
                        if (y >= myBounds.yMin && y < myBounds.yMax)
                        {
                            if (generatedTiles[x - (int)myBounds.xMin, y - (int)myBounds.yMin] != Glob.GetInstance().DefaultNullTileIndex)
                            {
                                mergedLayerTiles[x, y] = generatedTiles[x - (int)myBounds.xMin, y - (int)myBounds.yMin];
                                continue;
                            }
                        }
                    }
                    if (x >= otherBounds.xMin && x < otherBounds.xMax)
                    {
                        if (y >= otherBounds.yMin && y < otherBounds.yMax)
                        {
                            if (other.generatedTiles[x - (int)otherBounds.xMin, y - (int)otherBounds.yMin] != Glob.GetInstance().DefaultNullTileIndex)
                            {
                                mergedLayerTiles[x, y] = other.generatedTiles[x - (int)otherBounds.xMin, y - (int)otherBounds.yMin];
                                continue;
                            }
                        }
                    }

                    mergedLayerTiles[x, y] = Glob.GetInstance().DefaultNullTileIndex;
                }
            }

            generatedTiles = mergedLayerTiles;
            layerSize = new Vector2(generatedTiles.GetLength(0), generatedTiles.GetLength(1));

            return true;
        }

        private Dictionary<int, Color[]> GetTileIndexColors(int sizeX, int sizeY)
        {
            //Create a new dictionary to fetch our colors from, so we don't have to recalculate the colors for every individual tile.
            Dictionary<int, Color[]> colorIndexDictionary = new Dictionary<int, Color[]>();

            //Enumerate over the actual tile index dictionary.
            var tileIndexEnumerator = GetTileIndexDictionary().GetEnumerator();

            //For every type of tile that gets generated in the world, store a block of pixels to represent them in our preview
            while (tileIndexEnumerator.MoveNext())
            {
                //If there is no tile, or if the tile has no renderer
                //if (tileIndexEnumerator.Current.Value == null || tileIndexEnumerator.Current.Value.GetGraphVisual(colorBlockSize2, colorBlockSize2) == null)
                if (tileIndexEnumerator.Current.Value == null || Glob.GetInstance().GetTileGraphPreview(tileIndexEnumerator.Current.Value, sizeX, sizeY) == null)
                {
                    //Put an empty block of pixels in the dictionary.
                    colorIndexDictionary.Add(tileIndexEnumerator.Current.Key, new Color[sizeX * sizeY]);
                    continue;
                }

                //Put the pixels of the tile texture in the dictionary.
                colorIndexDictionary.Add(tileIndexEnumerator.Current.Key, Glob.GetInstance().GetTileGraphPreview(tileIndexEnumerator.Current.Value, sizeX, sizeY));
            }

            return colorIndexDictionary;
        }

        public Texture2D GetTileLayerPreviewTexture()
        {
//#if (UNITY_EDITOR)
            int tileLayerWidth = generatedTiles.GetLength(0);
            int tileLayerHeight = generatedTiles.GetLength(1);

            int textureWidth = tileLayerWidth * (int)tileSize.x;
            int textureHeight = tileLayerHeight * (int)tileSize.y;

            int colorBlockSize = Mathf.FloorToInt(Mathf.Min(tileSize.x, tileSize.y));

            //If the texture is going to be too big, use a smaller block of pixels per tile
            if (textureWidth > Glob.GetInstance().DefaultPreviewSize.x || textureHeight > Glob.GetInstance().DefaultPreviewSize.y)
            {
                //Instead of 'tileSize', use a smaller block of pixels per tile
                colorBlockSize = Mathf.Min((int)Mathf.Max(1, Glob.GetInstance().DefaultPreviewSize.x / tileLayerWidth), (int)Mathf.Max(1, Glob.GetInstance().DefaultPreviewSize.y / tileLayerHeight));

                //Recalculate the size of the texture
                textureWidth = tileLayerWidth * colorBlockSize;
                textureHeight = tileLayerHeight * colorBlockSize;

                Glob.GetInstance().DebugString("The preview for TileLayer " + this + " will be too large, so the amount of pixels per tile has been reduced to " + colorBlockSize + ". These pixels will be taken from the top left of the tile.", Glob.DebugCategories.Node, Glob.DebugLevel.Low, Glob.DebugTypes.Warning);
            }

            //Create an empty texture.
            Texture2D previewTexture = new Texture2D(textureWidth, textureHeight, TextureFormat.RGB24, true);

            //Create a new dictionary to fetch our colors from, so we don't have to recalculate the colors for every individual tile.
            Dictionary<int, Color[]> colorIndexDictionary = GetTileIndexColors(colorBlockSize, colorBlockSize);

            //For every tile in the world
            for (int x = 0; x < tileLayerWidth; x++)
            {
                for (int y = 0; y < tileLayerHeight; y++)
                {
                    //Set a block of tileSize pixels in our texture to the pixels of the associated tile.
                    previewTexture.SetPixels(x * colorBlockSize, y * colorBlockSize, colorBlockSize, colorBlockSize, colorIndexDictionary[generatedTiles[x, y]]);
                }
            }
            //Apply the pixels to the preview texture.
            previewTexture.Apply();

            return previewTexture;
//#else
//            return null;
//#endif
        }
    }
}
