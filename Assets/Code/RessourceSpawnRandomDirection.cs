using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RessourceSpawnRandomDirection : MonoBehaviour
{
    public float force;

    // Start is called before the first frame update
    void Start()
    {
        Vector2 direction = new Vector2((float)Random.Range(-4, 4), (float)Random.Range(-4, 4));
        GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
    }
}
