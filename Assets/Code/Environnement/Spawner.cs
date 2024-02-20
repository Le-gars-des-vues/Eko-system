using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    public bool spawnObject;

    private void Update()
    {
        if (spawnObject)
        {
            spawnObject = false;
            Instantiate(objectToSpawn, transform.position, transform.rotation);
        }
    }
}
