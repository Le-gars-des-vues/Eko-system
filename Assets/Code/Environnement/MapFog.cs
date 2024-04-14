using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapFog : MonoBehaviour
{
    [SerializeField] Tilemap tilemap;
    [SerializeField] GridLayout grid;

    // Start is called before the first frame update
    void Start()
    {
        tilemap = GameObject.Find("MapFog").GetComponent<Tilemap>();
        grid = GameObject.Find("MapFog").GetComponent<GridLayout>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Isolated")
        {
            ContactPoint2D[] contactPoints = collision.contacts;
            DestroyTile(contactPoints[0]);
        }
    }

    void DestroyTile(ContactPoint2D contactPoint)
    {
        Vector3Int tilePos = grid.WorldToCell(contactPoint.point);
        tilemap.SetTile(tilePos, null);
    }
}
