using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour, IDataPersistance
{
    [Header("Tile Format")]

    //CHANGEMENT ICI------
    //[HideInInspector]
    public float tileSizeWidth;
    //[HideInInspector]
    public float tileSizeHeight;

    [Header("Transform of Grid")]
    RectTransform rectTransform;

    [Header("Mouse and individual tile positions")]
    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();

    InventoryItem[,] inventoryItemSlot;

    [SerializeField] int gridSizeWidth = 20;
    [SerializeField] int gridSizeHeight = 15;




    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        if (gameObject.tag == "Hotbar" || gameObject.tag == "Upgrade" || gameObject.tag == "Equipment" || gameObject.tag == "Farming")
        {
            tileSizeWidth = 96;
            tileSizeHeight = 96;
        }
        else
        {
            tileSizeWidth = 64;
            tileSizeHeight = 64;
        }
        Init(gridSizeWidth, gridSizeHeight);
    }

    //Initialise la grille a la taille desire selon les parametres gridSizeWitdh/Height
    private void Init(int width, int height)
    {
        //Cree autant de case dans le array que de case sur la grille
        inventoryItemSlot = new InventoryItem[width, height];
        //Multiplie le nombre de case par leur taille

        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }
    
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        //Calcule la difference entre la position de la souris et le pivot du RectTransform (la case en haut a droite devrait etre 0,0)
        positionOnTheGrid.x = mousePosition.x-rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        Vector2 scaledGridPosition = new Vector2();
        scaledGridPosition.x = positionOnTheGrid.x / Screen.width * 1920;
        scaledGridPosition.y = positionOnTheGrid.y / Screen.height * 1080;

        //Transform la position en Int pour identifier la bonne case. Le second diviseur est en fonction du Scale du RectTransform, pas besoin de changer le code si on change le scale!
        tileGridPosition.x = (int)(scaledGridPosition.x / tileSizeWidth) / (int)rectTransform.localScale.x;
        tileGridPosition.y = (int)(scaledGridPosition.y / tileSizeHeight) / (int)rectTransform.localScale.y;

        return tileGridPosition;
    }

    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null)
        {
            return null;
        }

        CleanGridReference(toReturn);
        return toReturn;
    }

    public void CleanGridReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.WIDTH; ix++)
        {
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
        {
            return false;
        }

        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        List<string> keys = new List<string>(Recipes.discoveredRessources.Keys);
        foreach (string ressource in keys)
        {
            if (inventoryItem.itemData.itemName == ressource && Recipes.discoveredRessources[ressource] == false)
            {
                Recipes.discoveredRessources[ressource] = true;
                PromptManager.instance.SendNotification("NEW RESSOURCE!", "DISCOVERED: " + inventoryItem.itemData.itemName, inventoryItem.itemData.itemIcon);
                //Debug.Log(ressource + " has been discovered");
            }
        }

        inventoryItem.itemGrid = this;

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;

        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;
    }

    public bool InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = this.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null) { return false; }

        this.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
        return true;
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.HEIGHT / 2);
        return position;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < height; y ++){

                if (inventoryItemSlot[posX+x, posY+y] != null)
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else if(overlapItem != inventoryItemSlot[posX + x, posY + y])
                    {
                        return false;
                    }
                    
                }


            }
        }
        return true;
    }

    private bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                    return false;
            }
        }
        return true;
    }

    public InventoryItem CheckIfItemPresent(int posX, int posY)
    {
        if (inventoryItemSlot[posX, posY] == null)
        {
            return null;
        }
        else
        {
            return inventoryItemSlot[posX, posY];
        }

    }

    public int GetGridSizeWidth()
    {
        return gridSizeWidth;
    }

    public int GetGridSizeHeight() 
    {
        return gridSizeHeight; 
    }

    bool PositionCheck(int posX, int posY)
    {
        if(posX < 0 || posY < 0)
        {
            return false;
        }

        if(posX > gridSizeWidth || posY > gridSizeHeight) 
        {
            return false;
        }

        return true;
    }

    public bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if(PositionCheck(posX, posY) == false)
        {
            return false;
        }
        
        posX += width/*-1*/;
        posY += height;

        if (PositionCheck(posX, posY) == false)
        {
            return false;
        }

        return true;
    }

    internal InventoryItem GetItem(int x, int y)
    {
        Vector2 index = new Vector2(x, y);
        bool isInsideArray = index.x >= 0 && // check left
                               index.y >= 0 && // check bottom
                               index.x < inventoryItemSlot.GetLength(0) && // check right
                               index.y < inventoryItemSlot.GetLength(1);
        if (isInsideArray)
            return inventoryItemSlot[x, y];
        else
        {
            Debug.Log("index Was outside the bound of the array");
            return inventoryItemSlot[0, 0];
        }
    }

    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeHeight-itemToInsert.HEIGHT;
        int width = gridSizeWidth-itemToInsert.WIDTH;
        for (int y = 0; y < height + 1; y++)
        {
            for (int x = 0; x < width + 1; x++)
            {
                if(CheckAvailableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true)
                {
                    return new Vector2Int(x, y);
                }
            }
        }
        return null;
    }

    public void LoadData(GameData data)
    {
        foreach (GameObject item in data.itemsInInventory)
        {
            if (item.GetComponent<InventoryItem>().isInInventory && item.GetComponent<InventoryItem>().itemGrid == this)
            {
                var itemToPlace = Instantiate(item);
                itemToPlace.GetComponent<InventoryItem>().itemGrid.PlaceItem(itemToPlace.GetComponent<InventoryItem>(), itemToPlace.GetComponent<InventoryItem>().onGridPositionX, itemToPlace.GetComponent<InventoryItem>().onGridPositionY);
            }
        }
    }

    public void SaveData(ref GameData data)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            data.itemsInInventory.Add(transform.GetChild(i).gameObject);
        }
    }
}
