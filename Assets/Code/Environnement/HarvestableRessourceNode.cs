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
    bool isInRange;
    [SerializeField] GameObject ressourceToSpawn;
    [SerializeField] GameObject consummableToSpawn;
    public float ressourceAmount;
    [SerializeField] float spawnForce;
    public bool isHarvested = false;

    public bool isPointing;
    public bool isOutlined;
    PlayerPermanent player;

    public Vector2 direction = new Vector2(0, -1);
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
        if (timer >= player.timeToHarvest && !isHarvested)
        {
            EmptyRessources();
            player.Harvest(false);
        }

        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, groundRaycastLength, LayerMask.GetMask("Ground", "Planters"));
        if (hit.collider == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + direction.x / 10, transform.position.y + direction.y / 10), 0.1f);
        }

        if (Vector2.Distance(player.gameObject.transform.position, transform.position) < player.minDistanceToHarvest)
        {
            if (!Tutorial.instance.hasSeenFirstRessource)
            {
                Tutorial.instance.RobotTextMessage(Tutorial.instance.tutorialTexts[1].text);
                Tutorial.instance.hasSeenFirstRessource = true;
            }
            isInRange = true;
        }
        else
            isInRange = false;
    }

    void EmptyRessources()
    {
        isHarvested = true;
        GetComponent<Collider2D>().enabled = false;
        int i = 0;
        while (i < ressourceAmount)
        {
            i++;
            var ressource = Instantiate(ressourceToSpawn, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.rotation);
            Vector2 direction = new Vector2((float)Random.Range(-4, 4), (float)Random.Range(-4, 4));
            ressource.GetComponent<Rigidbody2D>().AddForce(direction * spawnForce, ForceMode2D.Impulse);
        }
        if (gameObject.tag == "Plant")
        {
            sprite_full.enabled = false;
            sprite_empty.enabled = false;
            GetComponent<PlantConsummable>().enabled = false;

            if (!GetComponent<PlantConsummable>().hasPickedUpConsummable)
                Instantiate(consummableToSpawn, transform.position, transform.rotation);
        }
        else if (gameObject.tag == "Mineral")
            sprite_full.enabled = false;
    }

    private void OnMouseEnter()
    {
        if (isInRange && player.isUsingMultiTool)
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
        if (player.isHarvesting)
            player.Harvest(false);
    }

    private void OnMouseOver()
    {
        if (player.isUsingMultiTool)
        {
            if (isInRange)
            {
                sprite_empty.material = outlineMaterial;
                sprite_full.material = outlineMaterial;
                Tutorial.instance.ListenForInputs("hasHovered");
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
                if (!player.isHarvesting)
                {
                    player.Harvest(true);
                    Tutorial.instance.ListenForInputs("hasHarvested");
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                timer = 0f;
                sprite_empty.color = Color.white;
                sprite_full.color = Color.white;
                if (player.isHarvesting)
                    player.Harvest(false);
            }
        }
        else
        {
            sprite_empty.material = ogMaterial1;
            sprite_full.material = ogMaterial2;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, direction * groundRaycastLength);
    }
}
