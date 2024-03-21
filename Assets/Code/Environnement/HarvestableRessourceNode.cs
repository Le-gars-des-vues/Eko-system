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
    public float timer;
    [SerializeField] GameObject ressourceToSpawn;
    [SerializeField] GameObject consummableToSpawn;
    public float ressourceAmount;
    [SerializeField] float spawnForce;

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
        if (timer >= player.timeToHarvest)
        {
            int i = 0;
            while (i < ressourceAmount)
            {
                i++;
                var ressource = Instantiate(ressourceToSpawn, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.rotation);
                Vector2 direction = new Vector2((float)Random.Range(-4, 4), (float)Random.Range(-4, 4));
                ressource.GetComponent<Rigidbody2D>().AddForce(direction * spawnForce, ForceMode2D.Impulse);
            }
            if (gameObject.tag == "Plant" && !GetComponent<PlantConsummable>().hasPickedUpConsummable)
                Instantiate(consummableToSpawn, transform.position, transform.rotation);

            gameObject.SetActive(false);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundRaycastLength, LayerMask.GetMask("Ground", "Planters"));
        if (hit.collider == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y - 0.1f), 0.1f);
        }
    }

    private void OnMouseEnter()
    {
        if (Vector2.Distance(player.gameObject.transform.position, transform.position) < player.minDistanceToHarvest && player.isUsingMultiTool)
        {
            sprite_empty.material = outlineMaterial;
            sprite_full.material = outlineMaterial;
        }
    }

    private void OnMouseExit()
    {
        sprite_empty.material = ogMaterial1;
        sprite_full.material = ogMaterial2;
        timer = 0f;
        sprite_empty.color = Color.white;
        sprite_full.color = Color.white;
    }

    private void OnMouseOver()
    {
        if (player.isUsingMultiTool)
        {
            if (Vector2.Distance(player.gameObject.transform.position, transform.position) < player.minDistanceToHarvest)
            {
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
                timer += Time.deltaTime;
                sprite_empty.color = Color.Lerp(sprite_empty.color, Color.red, timer * Time.deltaTime);
                sprite_full.color = Color.Lerp(sprite_full.color, Color.red, timer * Time.deltaTime);
            }

            if (Input.GetMouseButtonUp(0))
            {
                timer = 0f;
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
