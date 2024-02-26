using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickableObject : MonoBehaviour
{
    private Material ogMaterial;
    [SerializeField] private Material flashMaterial;
    private Color ogColor;
    private SpriteRenderer sprite;

    private GameObject rightHand;
    //private GameObject leftHand;
    private PlayerPermanent player;

    [HideInInspector] public InventoryItem item;
    public ItemGrid inventory, playerInventory;
    public List<GameObject> hotbar = new List<GameObject>();
    public GameObject itemInInventory;
    [SerializeField] private GameObject itemPrefab;

    public bool hasFlashed; //Pour les consommable
    public bool isFlashing; //Pour les ressources
    public bool isSelected;
    public bool isPickedUp = false;
    [SerializeField] private float rotateSpeed;

    // Start is called before the first frame update
    void OnEnable()
    {
        sprite = GetComponent<SpriteRenderer>();
        ogColor = sprite.color;
        ogMaterial = sprite.material;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();

        rightHand = GameObject.FindGameObjectWithTag("Player").transform.Find("player_model").transform.Find("bone_1").Find("bone_2").Find("bone_4").Find("bone_5").gameObject;
        //leftHand = GameObject.FindGameObjectWithTag("Player").transform.Find("player_model").transform.Find("bone_1").Find("bone_2").Find("bone_6").Find("bone_7").gameObject;

        playerInventory = GameObject.Find("GridInventaire").GetComponent<ItemGrid>();
        item = GetComponent<InventoryItem>();

        foreach (GameObject hb in GameObject.FindGameObjectsWithTag("Hotbar"))
        {
            if (hb.GetComponent<ItemGrid>() != null)
            {
                hotbar.Add(hb);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (gameObject.tag != "Ressource")
        {
            //Debug.Log(Mathf.Abs(Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position)));
            if (CanBePickedUp() && !hasFlashed)
            {
                hasFlashed = true;
                StartCoroutine(FlashWhite(sprite, 0.05f, 5));
            }
            else if (!CanBePickedUp() && hasFlashed)
                hasFlashed = false;
        }
        else
        {
            if (isSelected && !isFlashing)
            {
                isFlashing = true;
                sprite.material = flashMaterial;
            }
            else if (!isSelected && isFlashing)
            {
                isFlashing = false;
                sprite.material = flashMaterial;
            }

            if (Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) <= 3f)
            {
                if (!GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().ressourcesNear.Contains(gameObject))
                    GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().ressourcesNear.Add(gameObject);
            }
            else
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().ressourcesNear.Remove(gameObject);
                sprite.material = ogMaterial;
                isSelected = false;
                isFlashing = false;
            }
        }

        //Pick up
        if (Input.GetKeyDown(KeyCode.E) && (hasFlashed || isSelected))
        {
            if (!isPickedUp)
            {
                PickUp(false, false);
            }
        }

        if (isPickedUp)
        {
            item.onGridPositionX = itemInInventory.GetComponent<InventoryItem>().onGridPositionX;
            item.onGridPositionY = itemInInventory.GetComponent<InventoryItem>().onGridPositionY;

            transform.position = rightHand.transform.Find("RightArmEffector").transform.position;

            if (gameObject.tag == "Spear")
            {   
                /*
                Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
                if ((difference.x < 0f && isFacingRight) || (difference.x > 0f && !isFacingRight))
                {
                    Turn();
                }
                */

                Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
                //transform.rotation = Quaternion.Euler(0, 0, angle);
            }
        }
    }

    public void PickUp(bool isAlreadyInInventory, bool bypassObjectInHand)
    {
        if (!isAlreadyInInventory)
        {
            bool hasBeenPlaced = false;
            InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
            ItemData itemData = Instantiate(item.itemData);
            for (int i = hotbar.Count - 1; i > 0; i--)
            {
                if (hotbar[i].GetComponent<ItemGrid>().GetItem(0, 0) == null)
                {
                    inventory = hotbar[i].GetComponent<ItemGrid>();
                    itemData.width = itemData.hotbarWidth;
                    itemData.height = itemData.hotbarHeight;

                    inventoryItem.Set(itemData, inventory);
                    inventoryItem.stackAmount = item.stackAmount;

                    inventory.PlaceItem(inventoryItem, 0, 0);
                    hasBeenPlaced = true;
                    break;
                }
            }
            if (inventory == null)
                inventory = playerInventory;

            inventoryItem.Set(itemData, inventory);
            inventoryItem.stackAmount = item.stackAmount;

            if (!hasBeenPlaced)
                InsertItem(inventoryItem);

            itemInInventory = inventoryItem.gameObject;
        }

        if (gameObject.tag != "Ressource")
        {
            if (player.objectInRightHand == null || (bypassObjectInHand && player.objectInRightHand != null))
            {
                Physics2D.IgnoreCollision(GetComponent<CapsuleCollider2D>(), player.gameObject.GetComponent<Collider2D>(), true);
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().gravityScale = 0;

                float facingAngle = player.isFacingRight ? 0 : 180;
                transform.eulerAngles = new Vector3(0, 0, facingAngle);

                transform.position = rightHand.transform.Find("RightArmEffector").transform.position;
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - rightHand.transform.rotation.z);

                //gameObject.transform.SetParent(rightHand.transform);
                sprite.sortingOrder = 7;
                player.EquipObject(gameObject);
                isPickedUp = true;
            }
            else
            {
                Destroy(gameObject);
            }
            /* Utilisation de la main gauche
            else if (player.objectInLeftHand == null)
            {
                isPickedUp = true;
                GetComponent<BoxCollider2D>().enabled = false;
                GetComponent<Rigidbody2D>().simulated = false;
                transform.position = leftHand.transform.Find("LeftArmEffector").transform.position;
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - leftHand.transform.rotation.z);
                gameObject.transform.SetParent(leftHand.transform);
                GetComponent<SpriteRenderer>().sortingOrder = 2;
                player.EquipObject(gameObject, false);
            }
            */
        }
        else
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().ressourcesNear.Clear();
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().nearestRessourceDistance = 10;

            Destroy(gameObject);
        }
    }

    //Flash en blanc en changeant le materiel du joueur
    private IEnumerator FlashWhite(SpriteRenderer sprite, float duration, int flashCount)
    {
        for (int i = 0; i < flashCount; i++)
        {
            yield return new WaitForSecondsRealtime(duration);
            //ogMaterial = sprite.material;
            sprite.color = new Color(255, 255, 255, 255);
            //sprite.material = flashMaterial;
            yield return new WaitForSecondsRealtime(duration);
            //sprite.material = ogMaterial;
            sprite.color = ogColor;
            if (!CanBePickedUp())
            {
                sprite.color = ogColor;
                yield break;
            }
            if (isPickedUp)
            {
                sprite.color = ogColor;
                yield break;
            }
        }
    }

    bool CanBePickedUp()
    {
        if (gameObject.tag == "Spear")
        {
            return Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.Find("LeftHandPos").transform.position) <= 2f;
        }
        else
            return Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) <= 2f;
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = inventory.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) 
        {
            return; 
        }

        inventory.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }
}
