using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileDestroyer : MonoBehaviour
{
    Tilemap tilemap;
    Vector3 mousPos;
    Vector3Int tilePos;
    [SerializeField] GridLayout grid;

    Vector3Int nTilePos;
    Vector3Int sTilePos;
    Vector3Int wTilePos;
    Vector3Int eTilePos;
    Tile nTile;
    Tile sTile;
    Tile wTile;
    Tile eTile;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        mousPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        tilePos = grid.WorldToCell(mousPos);
        if (Input.GetMouseButtonUp(0))
        {
            //GetLocalTiles(tilePos);
            tilemap.SetTile(tilePos, null);
        }
    }

    void GetLocalTiles(Vector3Int tielPos)
    {
        nTilePos = new Vector3Int(tilePos.x, tilePos.y + 1, tilePos.z);
        sTilePos = new Vector3Int(tilePos.x, tilePos.y - 1, tilePos.z);
        wTilePos = new Vector3Int(tilePos.x - 1, tilePos.y, tilePos.z);
        eTilePos = new Vector3Int(tilePos.x + 1, tilePos.y, tilePos.z);

        nTile = tilemap.GetTile<Tile>(nTilePos);
        sTile = tilemap.GetTile<Tile>(sTilePos);
        wTile = tilemap.GetTile<Tile>(wTilePos);
        eTile = tilemap.GetTile<Tile>(eTilePos);
    }
}
