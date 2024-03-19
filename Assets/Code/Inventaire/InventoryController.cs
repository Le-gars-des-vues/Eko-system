using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryController : MonoBehaviour
{
    //[HideInInspector]
    [SerializeField] private ItemGrid selectedItemGrid;
    private ItemGrid defaultItemGrid;
    private ItemGrid upgradeItemGrid;

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
    public List<ItemData> craftables = new List<ItemData>();
    public List<GameObject> buildablesSide = new List<GameObject>();
    public List<GameObject> buildablesDown = new List<GameObject>();
    public List<GameObject> emptyRooms = new List<GameObject>();
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight inventoryHighlight;

    InventoryItem itemToHighlight;

    HotbarManager hotbar;

    Vector2Int oldPosition;
    Vector2Int oldPosition2;
    string gridName;
    string gridName2;

    [SerializeField] GameObject invalid;

    [SerializeField] GameObject itemInfo;
    [SerializeField] GameObject currentInfo;
    bool infoShown = false;


    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
        hotbar = GameObject.Find("Hotbar").GetComponent<HotbarManager>();
        defaultItemGrid = GameObject.Find("GridInventaire").GetComponent<ItemGrid>();
        upgradeItemGrid = GameObject.Find("GridUpgrade").GetComponent<ItemGrid>();
    }

    private void Start()
    {
        invalid = GameObject.Find("X");
        invalid.SetActive(false);
    }

    private void Update()
    {
        ItemIconDrag();

        ShowItemInfo();

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

        if (selectedItemGrid != null) 
        {
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().inventoryOpen || GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().upgradeIsOpen)
                HandleHighlight();
        }
        else
            inventoryHighlight.Show(false);

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }
    }

    void ShowItemInfo()
    {
        if (selectedItemGrid != null && selectedItemGrid.gameObject.tag != "Hotbar" && selectedItem == null)
        {
            Vector2Int tileGridPosition = GetTileGridPosition();
            if (oldPosition2 == tileGridPosition && gridName2 == selectedItemGrid.gameObject.name)
            {
                if (currentInfo != null)
                {
                    rectTransform.position = Input.mousePosition;
                    rectTransform.SetParent(canvasTransform);
                    rectTransform.SetAsLastSibling();
                }
            }
            else
            {
                if (gridName2 == selectedItemGrid.gameObject.name)
                {
                    if (currentInfo != null && currentInfo.GetComponent<ItemInfo>().referenceditem != selectedItemGrid.GetItem(tileGridPosition.x, tileGridPosition.y))
                    {
                        Destroy(currentInfo);
                    }
                    else
                    {
                        if (selectedItemGrid.GetItem(tileGridPosition.x, tileGridPosition.y) != null && currentInfo == null)
                        {
                            Debug.Log("Created info");
                            currentInfo = Instantiate(itemInfo);
                            currentInfo.GetComponent<ItemInfo>().Set(selectedItemGrid.GetItem(tileGridPosition.x, tileGridPosition.y).itemData);
                            currentInfo.GetComponent<ItemInfo>().referenceditem = selectedItemGrid.GetItem(tileGridPosition.x, tileGridPosition.y);
                            rectTransform = currentInfo.GetComponent<RectTransform>();
                        }
                    }
                }
                else
                {
                    Destroy(currentInfo);
                }
            }
            oldPosition2 = tileGridPosition;
            gridName2 = selectedItemGrid.gameObject.name;
        }
        else
        {
            if (currentInfo != null)
            {
                Destroy(currentInfo);
            }
        }
    }

    private void RotateItem()
    {
        if (selectedItem == null) { return; }

        selectedItem.Rotate();
    }

    public void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) 
        {  
            itemToInsert.DropItem(); 
        }
        else
            selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();

        //CHANGEMENT ICI---------
        if (oldPosition == positionOnGrid && gridName == selectedItemGrid.gameObject.name && inventoryHighlight.highlighter.gameObject.activeSelf) 
        {
            Debug.Log("Returned");
            return;  
        }
        oldPosition = positionOnGrid;
        gridName = selectedItemGrid.gameObject.name;

        if (selectedItem == null)
        {
            if (positionOnGrid.x < selectedItemGrid.GetGridSizeWidth() && positionOnGrid.y < selectedItemGrid.GetGridSizeHeight())
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
        inventoryItem.gameObject.tag = "Ressource";
    }

    public void CreateRecipeItem(int recipeChoice, GameObject dropDown)
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        inventoryItem.Set(craftables[recipeChoice], defaultItemGrid);
        if (craftables[recipeChoice].isUpgrade)
        {
            Vector2Int? posOnGrid = upgradeItemGrid.FindSpaceForObject(inventoryItem);
            if (posOnGrid == null)
            {
                selectedItem = inventoryItem;

                rectTransform = inventoryItem.GetComponent<RectTransform>();
                rectTransform.SetParent(canvasTransform);
                rectTransform.SetAsLastSibling();
                return; 
            }
            upgradeItemGrid.PlaceItem(inventoryItem, posOnGrid.Value.x, posOnGrid.Value.y);
            Recipes.listOfRecipes.Remove(recipeChoice);
            if (recipeChoice != Recipes.listOfRecipes.Count)
            {
                Recipes.listOfRecipes.Add(recipeChoice, Recipes.listOfRecipes[recipeChoice + 1]);
                for (int i = recipeChoice + 1; i < Recipes.listOfRecipes.Count; i++)
                {
                    Recipes.listOfRecipes.Remove(i);
                    if (i != Recipes.listOfRecipes.Count)
                        Recipes.listOfRecipes.Add(i, Recipes.listOfRecipes[i + 1]);
                }
            }
            dropDown.GetComponent<TMP_Dropdown>().value--;
            dropDown.GetComponent<TMP_Dropdown>().options.RemoveAt(recipeChoice);
            dropDown.GetComponent<TMP_Dropdown>().RefreshShownValue();
        }
        else
        {
            selectedItem = inventoryItem;

            rectTransform = inventoryItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();
        }

        //int selectedItemID = recipeChoice;
        //inventoryItem.Set(craftables[selectedItemID], defaultItemGrid);
    }

    private void LeftMouseButtonPress()
    {
        if (selectedItemGrid != null)
        {
            Vector2Int tileGridPosition = GetTileGridPosition();
            //Debug.Log(tileGridPosition);
            if (selectedItem == null)
            {
                if (selectedItemGrid.gameObject.tag == "Upgrade")
                {
                    PickUpItem(tileGridPosition);
                    selectedItem.isUpgrading = false;
                }
                else
                    PickUpItem(tileGridPosition);
            }
            else
            {
                if (selectedItemGrid.gameObject.tag == "Upgrade")
                {
                    if (selectedItem.itemData.isUpgrade)
                    {
                        selectedItem.isUpgrading = true;
                        PlaceItem(tileGridPosition);
                    }
                    else
                        Debug.Log("Can only place upgrades in this inventory!");
                }
                else if (selectedItemGrid == upgradeItemGrid)
                {
                    if (selectedItem.itemData.isUpgrade)
                    {
                        selectedItem.isUpgrading = false;
                        PlaceItem(tileGridPosition);
                    }
                    else
                        Debug.Log("Can only place upgrades in this inventory!");
                }
                else
                {
                    PlaceItem(tileGridPosition);
                }
            }
        }
        else
        {
            if (selectedItem != null)
            {
                selectedItem.DropItem();
            }
            else
                return;
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

        //Debug.Log(selectedItemGrid.GetTileGridPosition(position));
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
