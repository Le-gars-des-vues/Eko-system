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

    CraftingManager crafting;

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
    [SerializeField] GameObject notWorking;
    public Sprite multitool;

    [SerializeField] GameObject itemInfo;
    [SerializeField] GameObject currentInfo;

    [SerializeField] bool canSpawnItem = true;


    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
        hotbar = GameObject.Find("Hotbar").GetComponent<HotbarManager>();
        defaultItemGrid = GameObject.Find("GridInventaire").GetComponent<ItemGrid>();
        upgradeItemGrid = GameObject.Find("GridUpgrade").GetComponent<ItemGrid>();
    }

    private void Start()
    {
        invalid = GameObject.Find("Invalid");
        invalid.SetActive(false);
        crafting = GameObject.Find("Crafting").transform.Find("DropdownListOfCrafts").GetComponent<CraftingManager>();
    }

    private void Update()
    {
        ItemIconDrag();

        ShowItemInfo();

        if (Input.GetKeyDown(KeyCode.Q) && canSpawnItem)
        {
            if (selectedItem == null)
            {
                InsertItemAtRandom();
            }
        }

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
            Debug.Log(tileGridPosition);
            if (oldPosition2 == tileGridPosition && gridName2 == selectedItemGrid.gameObject.name)
            {
                if (currentInfo != null)
                {
                    rectTransform.position = Input.mousePosition;
                    rectTransform.SetParent(canvasTransform);
                    rectTransform.SetAsLastSibling();
                    Debug.Log("Returned");
                    return;
                }
            }
            oldPosition2 = tileGridPosition;
            gridName2 = selectedItemGrid.gameObject.name;

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

    public void InsertItemAtRandom()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID], defaultItemGrid);
        if (inventoryItem.itemData.itemType == "Ressource")
            inventoryItem.gameObject.tag = "Ressource";

        Vector2Int? posOnGrid = defaultItemGrid.FindSpaceForObject(inventoryItem);

        if (posOnGrid == null)
        {
            inventoryItem.DropItem();
        }
        else
        {
            defaultItemGrid.PlaceItem(inventoryItem, posOnGrid.Value.x, posOnGrid.Value.y);
            OnItemPlaced();
        }
    }

    public void InsertItem(InventoryItem itemToInsert)
    {

        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) 
        {
            itemToInsert.DropItem(); 
        }
        else
        {

            selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
            OnItemPlaced();
        }
    }

    private void HandleHighlight()
    {
        Vector2Int positionOnGrid = GetTileGridPosition();

        //CHANGEMENT ICI---------
        if (oldPosition == positionOnGrid && gridName == selectedItemGrid.gameObject.name && inventoryHighlight.highlighter.gameObject.activeSelf) 
        {
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
        inventoryItem.stackAmount = craftables[recipeChoice].objectToSpawn.GetComponent<InventoryItem>().stackAmount;
        inventoryItem.Set(craftables[recipeChoice], defaultItemGrid);
        inventoryItem.sprites = craftables[recipeChoice].objectToSpawn.GetComponent<InventoryItem>().sprites;
        inventoryItem.maxDurability = craftables[recipeChoice].objectToSpawn.GetComponent<InventoryItem>().maxDurability;
        inventoryItem.lowDurabilityThreshold = inventoryItem.maxDurability / 3;
        inventoryItem.currentDurability = inventoryItem.maxDurability;
        if (inventoryItem.sprites.Length > 0)
            inventoryItem.GetComponent<Image>().sprite = inventoryItem.sprites[(inventoryItem.sprites.Length) - inventoryItem.stackAmount];
        if (craftables[recipeChoice].itemType == "Upgrade" || craftables[recipeChoice].itemType == "Equipment")
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
            /*
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
            */
            dropDown.GetComponent<CraftingManager>().knownRecipes.RemoveAt(dropDown.GetComponent<TMP_Dropdown>().value);
            //dropDown.GetComponent<TMP_Dropdown>().options.RemoveAt(dropDown.GetComponent<TMP_Dropdown>().value);
            //dropDown.GetComponent<TMP_Dropdown>().RefreshShownValue();
        }
        else if (recipeChoice == 0)
        {
            DialogueManager.conditions["craftedFirstItem"] = true;
            Debug.Log(DialogueManager.conditions["craftedFirstItem"]);
            /*
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
            */
            dropDown.GetComponent<CraftingManager>().knownRecipes.RemoveAt(dropDown.GetComponent<TMP_Dropdown>().value);
            //dropDown.GetComponent<TMP_Dropdown>().options.RemoveAt(dropDown.GetComponent<TMP_Dropdown>().value);
            //dropDown.GetComponent<TMP_Dropdown>().RefreshShownValue();
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().hasMultitool = true;
            notWorking.SetActive(false);
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

    void OnItemPlaced()
    {
        crafting.OnValueChanged();
    }

    private void LeftMouseButtonPress()
    {
        if (selectedItemGrid != null)
        {
            Vector2Int tileGridPosition = GetTileGridPosition();
            //Debug.Log(tileGridPosition);
            if (selectedItem == null)
                PickUpItem(tileGridPosition);
            else
            {
                if (selectedItemGrid.gameObject.tag == "Upgrade")
                {
                    if (selectedItem.itemData.itemType == "Upgrade")
                    {
                        selectedItem.isPlaced = true;
                        PlaceItem(tileGridPosition);
                    }
                    else
                        Debug.Log("Can only place upgrades in this inventory!");
                }
                else if (selectedItemGrid.gameObject.tag == "Equipment")
                {
                    if (selectedItem.itemData.itemType == "Equipment")
                    {
                        selectedItem.isPlaced = true;
                        PlaceItem(tileGridPosition);
                    }
                    else
                        Debug.Log("Can only place equipment in this inventory!");
                }
                else if (selectedItemGrid == upgradeItemGrid)
                {
                    if (selectedItem.itemData.itemType == "Upgrade" || selectedItem.itemData.itemType == "Equipment")
                    {
                        PlaceItem(tileGridPosition);
                    }
                    else
                        Debug.Log("Can only place upgrades in this inventory!");
                }
                /*
                else if (selectedItemGrid.gameObject.tag == "RoomCrafting")
                {
                    RoomInfo room = GameObject.Find("RoomMenu").GetComponent<RoomManager>().currentRoom.GetComponent<RoomInfo>();
                    if (room.roomType == "Farm")
                    {
                        if (selectedItem.itemData.itemType == "Plant")
                        {
                            selectedItem.isPlaced = true;
                            PlaceItem(tileGridPosition);
                        }
                    }
                    else if (room.roomType == "Enclosure")
                    {
                        if (selectedItem.itemData.itemType == "Creature")
                        {
                            selectedItem.isPlaced = true;
                            PlaceItem(tileGridPosition);
                        }
                    }
                }
                */
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

        Debug.Log(selectedItemGrid.GetTileGridPosition(position));
        return selectedItemGrid.GetTileGridPosition(position);
    }

    private void PlaceItem(Vector2Int tileGridPosition)
    {
        bool complete = selectedItemGrid.PlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if (complete)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.inventairePlace, gameObject);
            //selectedItem.itemGrid = selectedItemGrid;
            rectTransform.sizeDelta = new Vector2(selectedItem.WIDTH * selectedItemGrid.tileSizeWidth, selectedItem.HEIGHT * selectedItemGrid.tileSizeHeight);
            if (selectedItemGrid.gameObject.tag == "Hotbar")
            {
                if (selectedItemGrid == hotbar.grids[hotbar.currentlySelected])
                {
                    hotbar.SpawnObject(hotbar.currentlySelected, out GameObject objectSpawned);
                    hotbar.player.EquipObject(objectSpawned);
                }
            }
            OnItemPlaced();

            bool isAmmo = false;
            if (overlapItem != null)
            {
                if (selectedItem.itemData.itemType == "Ammo")
                {
                    if (selectedItem.itemData.itemName == overlapItem.itemData.itemName)
                    {
                        if (selectedItem.stackAmount < selectedItem.maxStack)
                        {
                            isAmmo = true;
                            int diff = selectedItem.maxStack - selectedItem.stackAmount;
                            for (int i = 0; i < diff; i++)
                            {
                                overlapItem.stackAmount--;
                                selectedItem.stackAmount++;
                                selectedItem.gameObject.GetComponent<Image>().sprite = selectedItem.sprites[selectedItem.sprites.Length - selectedItem.stackAmount];
                                if (overlapItem.stackAmount != 0)
                                {
                                    overlapItem.gameObject.GetComponent<Image>().sprite = overlapItem.sprites[overlapItem.sprites.Length - overlapItem.stackAmount];
                                }
                                else
                                {
                                    selectedItem = null;
                                    overlapItem.Delete();
                                    return;
                                }
                            }
                            selectedItem = null;
                            selectedItem = overlapItem;
                            overlapItem = null;
                            rectTransform = selectedItem.GetComponent<RectTransform>();
                            rectTransform.SetAsLastSibling();
                        }
                    }
                }
                
                if (!isAmmo)
                {
                    selectedItem = null;
                    selectedItem = overlapItem;
                    overlapItem = null;
                    rectTransform = selectedItem.GetComponent<RectTransform>();
                    rectTransform.SetAsLastSibling();
                }
            }
            else
                selectedItem = null;
        }
    }

    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            AudioManager.instance.PlaySound(AudioManager.instance.inventairePickUp, gameObject);
            selectedItem.itemGrid = null;
            selectedItem.isPlaced = false;
            rectTransform = selectedItem.GetComponent<RectTransform>();
            if (selectedItemGrid.gameObject.tag == "Hotbar")
            {
                if (selectedItemGrid = hotbar.grids[hotbar.currentlySelected])
                {
                    if (!hotbar.player.isUsingMultiTool)
                    {
                        Destroy(hotbar.player.objectInRightHand);
                        hotbar.player.UnequipObject();
                    }
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
            /*
            if (GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().roomManageIsOpen)
            {
                RoomInfo room = GameObject.Find("RoomMenu").GetComponent<RoomManager>().currentRoom.GetComponent<RoomInfo>();
                if (room.roomType == "Farm")
                {
                    if (selectedItem.itemData.itemType == "Plant")
                        invalid2.SetActive(false);
                    else
                        invalid2.SetActive(true);
                }
                else if (room.roomType == "Enclosure")
                {
                    if (selectedItem.itemData.itemType == "Creature")
                        invalid2.SetActive(false);
                    else
                        invalid2.SetActive(true);
                }
            }
            */
            if (selectedItem.tag == "Ressource" || selectedItem.tag == "Gear")
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
