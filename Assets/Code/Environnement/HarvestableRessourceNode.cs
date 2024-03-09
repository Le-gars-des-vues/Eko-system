using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HarvestableRessourceNode : MonoBehaviour
{
    [Header("Materials")]
    Material ogMaterial1;
    Material ogMaterial2;
    public Material outlineMaterial;
    [SerializeField] private SpriteRenderer sprite_empty;
    [SerializeField] private SpriteRenderer sprite_full;

    [Header("Harvest Variables")]
    private float timeToHarvest;
    [SerializeField] GameObject ressourceToSpawn;
    [SerializeField] GameObject consummableToSpawn;
    [SerializeField] float ressourceAmount;
    [SerializeField] float minDistanceToHarvest;

    public bool isPointing;
    public bool isOutlined;
    PlayerPermanent player;

    [SerializeField] float groundRaycastLength;

    // Start is called before the first frame update
    void Start()
    {
        ogMaterial1 = sprite_empty.material;
        ogMaterial2 = sprite_full.material;
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
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
            if (gameObject.tag == "Plant" && !GetComponent<PlantConsummable>().hasPickedUpConsummable)
                Instantiate(consummableToSpawn, transform.position, transform.rotation);

            gameObject.SetActive(false);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastLength, LayerMask.GetMask("Ground"));
        if (hit.collider == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y - 0.1f), 0.1f);
        }
    }

    private void OnMouseEnter()
    {
        if (Vector2.Distance(player.gameObject.transform.position, transform.position) < minDistanceToHarvest && player.isUsingMultiTool)
        {
            sprite_empty.material = outlineMaterial;
            sprite_full.material = outlineMaterial;
        }
    }

    private void OnMouseExit()
    {
        sprite_empty.material = ogMaterial1;
        sprite_full.material = ogMaterial2;
        timeToHarvest = 0f;
        sprite_empty.color = Color.white;
        sprite_full.color = Color.white;
    }

    private void OnMouseOver()
    {
        if (player.isUsingMultiTool)
        {
            if (Vector2.Distance(player.gameObject.transform.position, transform.position) < minDistanceToHarvest)
            {
                Debug.Log("Harvestable");
                sprite_empty.material = outlineMaterial;
                sprite_full.material = outlineMaterial;
            }
            else
            {
                sprite_empty.material = ogMaterial1;
                sprite_full.material = ogMaterial2;
            }

            if (Input.GetMouseButton(0))
            {
                timeToHarvest += Time.deltaTime;
                sprite_empty.color = Color.Lerp(sprite_empty.color, Color.red, timeToHarvest * Time.deltaTime);
                sprite_full.color = Color.Lerp(sprite_full.color, Color.red, timeToHarvest * Time.deltaTime);
            }

            if (Input.GetMouseButtonUp(0))
            {
                timeToHarvest = 0f;
                sprite_empty.color = Color.white;
                sprite_full.color = Color.white;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * groundRaycastLength);
    }
}
