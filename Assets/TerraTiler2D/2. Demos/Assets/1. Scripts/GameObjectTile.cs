using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
#if (UNITY_EDITOR)
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Tilemaps;

namespace TerraTiler2D
{
    /// <summary>
    /// Spawns a GameObject, and then disables this tile.
    /// This script is officially part of the https://github.com/Unity-Technologies/2d-extras GitHub repository from Unity, but asset store tools may not contain Git dependencies.
    /// </summary>
    public class GameObjectTile : UnityEngine.Tilemaps.Tile
    {
#if (UNITY_EDITOR)
        [MenuItem("Assets/Create/2D/Tiles/GameObject Tile", priority = 30)]
        static void CreateGameObjectTile()
        {
            GameObjectTile newTile = ScriptableObject.CreateInstance<GameObjectTile>();
            newTile.colliderType = ColliderType.None;
            newTile.flags = TileFlags.InstantiateGameObjectRuntimeOnly;

            ProjectWindowUtil.CreateAsset(newTile, "New GameObject Tile.asset");
        }
#endif

        public override bool StartUp(Vector3Int position, ITilemap tilemap, GameObject go)
        {
            base.StartUp(position, tilemap, go);

            //If this tile has spawned their gameobject
            if (go != null)
            {
                //Store the start color
                Color startColor = this.color;

                //Make this tile invisible
                this.color = new Color(0, 0, 0, 0);

                //Apply the color change to hide the tile, so that only the gameobject remains
                this.RefreshTile(position, tilemap);

                //Revert the color of the tile, so that the ScriptableObject doesn't get changed
                this.color = startColor;
            }

            return true;
        }
    }
}
