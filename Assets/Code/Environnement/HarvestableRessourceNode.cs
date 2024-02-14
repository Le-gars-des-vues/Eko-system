using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableRessourceNode : MonoBehaviour
{
    [Header("Materials")]
    Material ogMaterial;
    public Material outlineMaterial;
    [SerializeField] private SpriteRenderer sprite;

    [Header("Harvest Variables")]
    private float timeToHarvest;
    [SerializeField] GameObject ressourceToSpawn;
    [SerializeField] float ressourceAmount;
    [SerializeField] float minDistanceToHarvest;

    // Start is called before the first frame update
    void Start()
    {
        ogMaterial = sprite.material;
    }

    private void Update()
    {
        if (timeToHarvest >= 1.5f)
        {
            int i = 0;
            while (i < ressourceAmount)
            {
                i++;
                Instantiate(ressourceToSpawn, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.rotation);
            }
            gameObject.SetActive(false);
        }  
    }

    private void OnMouseEnter()
    {
        if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) < minDistanceToHarvest)
            sprite.material = outlineMaterial;
    }

    private void OnMouseExit()
    {
        sprite.material = ogMaterial;
        timeToHarvest = 0f;
        sprite.color = Color.white;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            timeToHarvest += Time.deltaTime;
            sprite.color = Color.Lerp(GetComponent<SpriteRenderer>().color, Color.red, timeToHarvest * Time.deltaTime);
        }
            
        if (Input.GetMouseButtonUp(0))
        {
            timeToHarvest = 0f;
            sprite.color = Color.white;
        }
    }
}
