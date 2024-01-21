using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    [Header("Tile Format")]

    public const float tileSizeWidth = 32;
    public const float tileSizeHeight = 32;
    private Canvas rootCanvas;

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
        //tileSizeWidth = Screen.width / 60;
        //tileSizeHeight = Screen.height / 33.75f;
        rootCanvas = GetComponentInParent<Canvas>();
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);

        
    }

    //Initialise la grille a la taille desire selon les parametres gridSizeWitdh/Height
    private void Init(int width, int height)
    {
        //Cree autant de case dans le array que de case sur la grille
        inventoryItemSlot = new InventoryItem[width, height];
        //Multiplie le nombre de case par leur taille
        Vector2 size= new Vector2(width*tileSizeWidth, height*tileSizeHeight);
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

    private void CleanGridReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.itemData.width; ix++)
        {
            for (int iy = 0; iy < item.itemData.height; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }

    public bool PlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        if(BoundryCheck(posX,posY,inventoryItem.itemData.width, inventoryItem.itemData.height) == false)
        {
            return false;
        }

        if(OverlapCheck(posX,posY,inventoryItem.itemData.width, inventoryItem.itemData.height, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        if(overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }

        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.itemData.width; x++)
        {
            for (int y = 0; y < inventoryItem.itemData.height; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        

        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.itemData.width / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.itemData.height/ 2);

        rectTransform.localPosition=position;

        return true;
    }

    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++)
        {
            for(int y = 0; y < width; y ++){

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

    bool BoundryCheck(int posX, int posY, int width, int height)
    {
        if(PositionCheck(posX, posY) == false)
        {
            return false;
        }

        posX += width-1;
        posY += height-1;

        if (PositionCheck(posX, posY) == false)
        {
            return false;
        }

        return true;
    }

}
