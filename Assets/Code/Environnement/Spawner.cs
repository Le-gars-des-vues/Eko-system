using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private GameObject gameManager;
    public GameObject objectSpawned;
    public string objectName;
    public bool canSpawn;
    public int index;

    private void Start()
    {
        gameManager = GameObject.Find("GameManager");
    }

    public void Spawn(List<GameObject> species = null)
    {
        var spawned = Instantiate(objectToSpawn, transform.position, transform.rotation);
        spawned.name = objectName + " " + index;
        spawned.transform.SetParent(gameObject.transform);
        objectSpawned = spawned;
        GameObject creature = spawned.transform.GetChild(0).gameObject;
        if (species != null)
        {
            foreach (GameObject specimen in species)
            {
                foreach (Collider2D collider in specimen.GetComponent<CreatureDeath>().creatureColliders)
                {
                    foreach (Collider2D col in creature.GetComponent<CreatureDeath>().creatureColliders)
                    {
                        Physics2D.IgnoreCollision(col, collider);
                    }
                }
            }

            species.Add(creature);
            creature.GetComponent<CreatureDeath>().species = species;

        }
    }
}
