using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    private ItemGrid selectedItemGrid;
    private ItemGrid defaultItemGrid;

    public ItemGrid SelectedItemGrid { get => selectedItemGrid;
        set { 
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    
    public InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;


    [SerializeField] List<ItemData> items;
    [SerializeField] List<ItemData> craftables = new List<ItemData>();
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight inventoryHighlight;

    InventoryItem itemToHighlight;

    HotbarManager hotbar;

    Vector2Int oldPosition;
    string gridName;

    [SerializeField] GameObject invalid;


    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
        hotbar = GameObject.Find("Hotbar").GetComponent<HotbarManager>();
        defaultItemGrid = GameObject.Find("GridInventaire").GetComponent<ItemGrid>();
    }

    private void Start()
    {
        invalid = GameObject.Find("X");
        invalid.SetActive(false);
    }

    private void Update()
    {
        ItemIconDrag();

        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (selectedItem == null)
            {
                CreateRandomItem();
            }
        }

        //Temporairement disabled, mais pourrait etre utile pour ajouter des items a l'inventaire quand on les pick up
        /*
        if (Input.GetKeyDown(KeyCode.P))
        {
            InsertRandomItem();
        }
        */

        if (Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }

        if (selectedItemGrid == null) {
            inventoryHighlight.Show(false);
            return;
        }

        if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().inventoryOpen)
            HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }
    }

    private void RotateItem()
    {
        if (selectedItem == null) { return; }

        selectedItem.Rotate();
    }

    private void InsertRandomItem()
    {
        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    public void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) {  return; }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();

        //CHANGEMENT ICI---------
        if (oldPosition == positionOnGrid && gridName == selectedItemGrid.gameObject.name && inventoryHighlight.gameObject.activeSelf) { return;  }
        oldPosition = positionOnGrid;
        gridName = selectedItemGrid.gameObject.name;

        if (selectedItem == null)
        {
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            if (itemToHighlight != null){
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight, selectedItemGrid);
                //inventoryHighlight.SetParent(selectedItemGrid);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else
            {
                inventoryHighlight.Show(false);
            }
            
        }
        else
        {
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(positionOnGrid.x, positionOnGrid.y, selectedItem.WIDTH, selectedItem.HEIGHT));
            inventoryHighlight.SetSize(selectedItem, selectedItemGrid);
            //inventoryHighlight.SetParent(selectedItemGrid);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID], defaultItemGrid);
    }

    public void CreateRecipeItem(int recipeChoice)
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = recipeChoice;
        inventoryItem.Set(craftables[selectedItemID], defaultItemGrid);
    }

    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();
        Debug.Log(tileGridPosition);

        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x += (selectedItem.WIDTH - (1 * selectedItem.WIDTH)) * selectedItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT- (1 * selectedItem.HEIGHT)) * selectedItemGrid.tileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if (complete)
        {
            rectTransform.sizeDelta = new Vector2(selectedItem.WIDTH * selectedItemGrid.tileSizeWidth, selectedItem.HEIGHT * selectedItemGrid.tileSizeHeight);
            if (selectedItemGrid.gameObject.tag == "Hotbar")
            {
                if (selectedItemGrid == hotbar.grids[hotbar.currentlySelected])
                {
                    hotbar.SpawnObject(hotbar.currentlySelected, out GameObject objectSpawned);
                    hotbar.player.EquipObject(objectSpawned);
                }
            }

            selectedItem = null;
            if (overlapItem != null)
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
            if (selectedItemGrid.gameObject.tag == "Hotbar")
            {
                if (selectedItemGrid = hotbar.grids[hotbar.currentlySelected])
                {
                    Destroy(hotbar.player.objectInRightHand);
                    hotbar.player.UnequipObject();
                }
            }
        }
    }

    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();
            if (selectedItem.tag == "Ressource")
                invalid.SetActive(true);
            else
            {
                if (invalid.activeSelf)
                    invalid.SetActive(false);
            }
        }
        else
        {
            if (invalid.activeSelf)
                invalid.SetActive(false);
        }
    }
}
