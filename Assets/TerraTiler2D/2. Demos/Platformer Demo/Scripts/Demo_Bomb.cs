using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TerraTiler2D
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class Demo_Bomb : MonoBehaviour
    {
        public float blastRadius = 0.5f;

        private void Start()
        {
            
        }

        private void Update()
        {
            
        }

        private void Explode()
        {
            GetComponent<CircleCollider2D>().isTrigger = true;
            GetComponent<CircleCollider2D>().radius = blastRadius;

            List<Collider2D> allDestructibleTiles = new List<Collider2D>();

            ContactFilter2D colliderFilter = new ContactFilter2D();
            colliderFilter.layerMask = 1 << 8;

            GetComponent<CircleCollider2D>().OverlapCollider(colliderFilter, allDestructibleTiles);

            for (int i = 0; i < allDestructibleTiles.Count; i++)
            {
                if (allDestructibleTiles[i].GetComponent<Destructible_Tile>() != null)
                {
                    allDestructibleTiles[i].GetComponent<Destructible_Tile>().DamageTile();
                }
            }

            Destroy(gameObject);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.GetComponent<Demo_CharacterController>() == null)
            {
                Explode();
            }
        }
    }
}
