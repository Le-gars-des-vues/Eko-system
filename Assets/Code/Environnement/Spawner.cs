using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    public GameObject objectSpawned;
    public string objectName;
    public bool canSpawn;

    public void Spawn()
    {
        var spawned = Instantiate(objectToSpawn, transform.position, transform.rotation);
        objectSpawned = spawned;
    }
}
