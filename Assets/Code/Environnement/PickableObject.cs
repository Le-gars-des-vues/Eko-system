using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public bool canFlash;
    public bool isSelected;
    public bool isPickedUp = false;
    [SerializeField] private float rotateSpeed;
    bool isFacingRight = true;

    public bool isSceneLoaded = false;

    private void Start()
    {
        if (isSceneLoaded)
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
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        if (!isSceneLoaded)
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!isPickedUp)
        {
            if (gameObject.tag != "Ressource" && canFlash)
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
            if (isSelected && !isFlashing)
            {
                isFlashing = true;
                //sprite.material = flashMaterial;
                if (ArrowManager.instance.targetObject != gameObject)
                    ArrowManager.instance.PlaceArrow(sprite.bounds.center, "PICK UP", new Vector2(0, -0.5f), gameObject);
            }
            else if (!isSelected && isFlashing)
            {
                isFlashing = false;
                //sprite.material = flashMaterial;
                if (ArrowManager.instance.targetObject == gameObject)
                    ArrowManager.instance.RemoveArrow();
            }

            //if (isFlashing)
                //arrow.transform.localRotation = Quaternion.Inverse(transform.rotation);

            if (player == null)
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
            if (Vector2.Distance(player.transform.position, transform.position) <= 3f)
            {
                if (!player.objectsNear.Contains(gameObject))
                {
                    player.objectsNear.Add(gameObject);
                }
            }
            else
            {
                if (player.nearestObject == gameObject)
                {
                    player.nearestObject = null;
                    player.nearestObjectDistance = 10f;
                }
                player.objectsNear.Remove(gameObject);
                //sprite.material = ogMaterial;
                isSelected = false;
            }

            //Pick up
            if (Input.GetKeyDown(KeyCode.E) && (hasFlashed || isSelected))
            {
                if (!isPickedUp && player.CanMove())
                {
                    PickUp(false, false);
                }
            }
        }
        else
        {
            item.onGridPositionX = itemInInventory.GetComponent<InventoryItem>().onGridPositionX;
            item.onGridPositionY = itemInInventory.GetComponent<InventoryItem>().onGridPositionY;

            transform.position = rightHand.transform.Find("RightArmEffector").transform.position;

            if ((gameObject.tag == "Spear" || gameObject.tag == "TwoHandedWeapon") && player.CanMove())
            {   
                Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);

                Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.gameObject.transform.position;
                if ((difference.x < 0f && isFacingRight) || (difference.x > 0f && !isFacingRight))
                {
                    Turn();
                }
                //transform.rotation = Quaternion.Euler(0, 0, angle);
            }
            else if (gameObject.tag == "OneHandedWeapon" && player.CanMove())
            {
                Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * 2 * Time.deltaTime);

                Vector2 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - player.gameObject.transform.position;
                if ((difference.x < 0f && isFacingRight) || (difference.x > 0f && !isFacingRight))
                {
                    Turn();
                }
            }
        }
    }

    void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.y *= -1;
        transform.localScale = scale;

        /*
        scale = arrow.transform.localScale;
        scale.y *= -1;
        arrow.transform.localScale = scale;
        */

        isFacingRight = !isFacingRight;
    }

    public void PickUp(bool isAlreadyInInventory, bool bypassObjectInHand)
    {
        //Si l'objet n'est pas deja dans l'inventaire
        if (!isAlreadyInInventory)
        {
            //On clear les objets proches pour recommencer la recherche
            player.objectsNear.Clear();
            player.nearestObjectDistance = 10;

            //Desactive le fleche
            ArrowManager.instance.RemoveArrow();

            //On cree l'item dans l'invenaire
            bool hasBeenPlaced = false;
            InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
            ItemData itemData = Instantiate(item.itemData);

            //Si c'est n'est pas une ressource ou du gear
            if (gameObject.tag != "Ressource" && gameObject.tag != "Gear")
            {
                for (int i = hotbar.Count - 1; i >= 0; i--)
                {
                    //Si une des cases de hotbar est libre
                    if (hotbar[i].GetComponent<ItemGrid>().GetItem(0, 0) == null)
                    {
                        //On change l'item dans l'inventaire
                        inventory = hotbar[i].GetComponent<ItemGrid>();
                        itemData.width = itemData.hotbarWidth;
                        itemData.height = itemData.hotbarHeight;

                        //On ajuste l'image et le stack
                        inventoryItem.stackAmount = item.stackAmount;
                        inventoryItem.Set(itemData, inventory);
                        inventoryItem.sprites = item.sprites;
                        if (inventoryItem.sprites.Length > 0)
                            inventoryItem.GetComponent<Image>().sprite = item.sprites[(item.sprites.Length) - item.stackAmount];

                        //On place l'item
                        inventory.PlaceItem(inventoryItem, 0, 0);
                        hasBeenPlaced = true;
                        break;
                    }
                }
            }
            
            //Si l'item n'a pas ete placer dans la hotbar, on regarde l'inventaire du joueur
            if (inventory == null)
                inventory = playerInventory;

            //Si l'item n'a pas ete placer, on le place dans l'inventaire
            if (!hasBeenPlaced)
            {
                inventoryItem.stackAmount = item.stackAmount;
                inventoryItem.Set(itemData, inventory);
                inventoryItem.sprites = item.sprites;
                if (inventoryItem.sprites.Length > 0)
                {
                    inventoryItem.GetComponent<Image>().sprite = item.sprites[(item.sprites.Length) - item.stackAmount];
                }
                InsertItem(inventoryItem);
            }

            //On ajuste le tag de l'item
            itemInInventory = inventoryItem.gameObject;
            if (gameObject.tag == "Ressource")
                itemInInventory.tag = "Ressource";
            if (gameObject.tag == "Gear")
                itemInInventory.tag = "Gear";
        }

        //Si l'objet n'est pas une ressource ou du gear
        if (gameObject.tag != "Ressource" && gameObject.tag != "Gear")
        {
            //Si le joueur n'a rien dans la main ou que c'est un item qui bypass l'objet en main
            if (player.objectInRightHand == null || (bypassObjectInHand && player.objectInRightHand != null))
            {
                //On ignore les collisions avec le joueur et on desactive le mouvement sur l'objet
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), player.gameObject.GetComponent<Collider2D>(), true);
                GetComponent<Rigidbody2D>().simulated = true;
                GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                GetComponent<Rigidbody2D>().angularVelocity = 0;
                GetComponent<Rigidbody2D>().gravityScale = 0;

                //On tourne l'objet dans la bonne direction
                float facingAngle = player.isFacingRight ? 0 : 180;
                transform.eulerAngles = new Vector3(0, 0, facingAngle);

                //On angle l'objet avec la main
                transform.position = rightHand.transform.Find("RightArmEffector").transform.position;
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - rightHand.transform.rotation.z);

                //On reset le parent de l'objet
                gameObject.transform.parent = null;

                //On ajuste le sorting layer
                sprite.sortingOrder = 8;

                //On equipe l'objet
                player.EquipObject(gameObject);
                isPickedUp = true;
            }
            //Sinon, on le delete
            else
            {
                Destroy(gameObject);
            }
        }
        else
            Destroy(gameObject);
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
            return Vector2.Distance(player.transform.position, transform.Find("LeftHandPos").transform.position) <= 2f;
        }
        else
            return Vector2.Distance(player.transform.position, transform.position) <= 2f;
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = inventory.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) 
        {
            itemToInsert.DropItem(); 
        }
        else
            inventory.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Ground") && !GetComponent<PickableObject>().isPickedUp)
            hasFlashed = false;
    }
}
