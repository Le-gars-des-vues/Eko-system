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
    [HideInInspector] public ItemGrid inventory;
    private GameObject itemInInventory;
    [SerializeField] private GameObject itemPrefab;

    public bool hasFlashed; //Pour les consommable
    public bool isFlashing; //Pour les ressources
    public bool isSelected;
    public bool isPickedUp = false;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float pickUpPos;
    private bool isFacingRight;

    // Start is called before the first frame update
    void OnEnable()
    {
        sprite = GetComponent<SpriteRenderer>();
        ogColor = sprite.color;
        ogMaterial = sprite.material;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();

        rightHand = GameObject.FindGameObjectWithTag("Player").transform.Find("player_model").transform.Find("bone_1").Find("bone_2").Find("bone_4").Find("bone_5").gameObject;
        //leftHand = GameObject.FindGameObjectWithTag("Player").transform.Find("player_model").transform.Find("bone_1").Find("bone_2").Find("bone_6").Find("bone_7").gameObject;

        inventory = GameObject.Find("GridInventaire").GetComponent<ItemGrid>();
        item = GetComponent<InventoryItem>();
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
                PickUp();
            }
        }

        if (isPickedUp)
        {
            item.onGridPositionX = itemInInventory.GetComponent<InventoryItem>().onGridPositionX;
            item.onGridPositionY = itemInInventory.GetComponent<InventoryItem>().onGridPositionY;

            if (gameObject.tag == "Spear")
            {
                transform.position = new Vector2(rightHand.transform.Find("RightArmEffector").transform.position.x + (transform.right.x * pickUpPos), rightHand.transform.Find("RightArmEffector").transform.position.y);
                
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

    public void PickUp()
    {
        if (gameObject.tag != "Ressource")
        {
            if (player.objectInRightHand == null)
            {
                isFacingRight = player.isFacingRight;

                GetComponent<CapsuleCollider2D>().enabled = false;
                GetComponent<Rigidbody2D>().simulated = false;

                float facingAngle = player.isFacingRight ? 0 : 180;
                transform.eulerAngles = new Vector3(0, 0, facingAngle);

                transform.position = new Vector2(rightHand.transform.Find("RightArmEffector").transform.position.x + (transform.right.x * pickUpPos), rightHand.transform.Find("RightArmEffector").transform.position.y);
                transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - rightHand.transform.rotation.z);

                //gameObject.transform.SetParent(rightHand.transform);
                sprite.sortingOrder = 7;
                player.EquipObject(gameObject, true);
                isPickedUp = true;
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
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        inventoryItem.Set(item.itemData);
        InsertItem(inventoryItem);
        itemInInventory = inventoryItem.gameObject;
    }

    private void Turn()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
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
        return Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position, transform.position) <= 2f;
    }

    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = inventory.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) { return; }

        inventory.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }
}
