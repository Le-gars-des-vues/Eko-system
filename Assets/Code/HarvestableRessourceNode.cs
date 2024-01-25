using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestableRessourceNode : MonoBehaviour
{
    [Header("Materials")]
    Material ogMaterial;
    public Material outlineMaterial;

    [Header("Harvest Variables")]
    private float timeToHarvest;
    public GameObject ressourceToSpawn;
    public float ressourceAmount;

    // Start is called before the first frame update
    void Start()
    {
        ogMaterial = GetComponent<SpriteRenderer>().material;
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
        GetComponent<SpriteRenderer>().material = outlineMaterial;
    }

    private void OnMouseExit()
    {
        GetComponent<SpriteRenderer>().material = ogMaterial;
        timeToHarvest = 0f;
        GetComponent<SpriteRenderer>().color = Color.white;
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButton(0))
        {
            timeToHarvest += Time.deltaTime;
            GetComponent<SpriteRenderer>().color = Color.Lerp(GetComponent<SpriteRenderer>().color, Color.red, timeToHarvest * Time.deltaTime);
        }
            
        if (Input.GetMouseButtonUp(0))
        {
            timeToHarvest = 0f;
            GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
