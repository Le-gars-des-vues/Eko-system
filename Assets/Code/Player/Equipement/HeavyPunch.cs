using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class HeavyPunch : MonoBehaviour
{
    [SerializeField] PlayerPermanent player;
    [SerializeField] Tilemap tilemap;
    [SerializeField] GridLayout gridLayout;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Breakable")
        {
            ContactPoint2D[] contactPoints = collision.contacts;
            if (tilemap == null)
                tilemap = collision.gameObject.GetComponent<Tilemap>();
            if (gridLayout == null)
                gridLayout = collision.gameObject.GetComponent<GridLayout>();

            Punch(contactPoints[0]);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Breakable")
        {
            ContactPoint2D[] contactPoints = collision.contacts;
            if (tilemap == null)
                tilemap = collision.gameObject.GetComponent<Tilemap>();
            if (gridLayout == null)
                gridLayout = collision.gameObject.GetComponent<GridLayout>();

            Punch(contactPoints[0]);
        }
    }

    void Punch(ContactPoint2D contactPoint)
    {
        Vector3Int tilePos = gridLayout.WorldToCell(contactPoint.point);
        tilemap.SetTile(tilePos, null);
    }
}
