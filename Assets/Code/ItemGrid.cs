using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    [Header("Tile Format")]
    //Faut somehow mentir au code pour qu'il marche
    const float tileSizeWidth = 16;
    const float tileSizeHeight = 16;

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
        Init(gridSizeWidth, gridSizeHeight);
    }

    private void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size= new Vector2(width*tileSizeWidth*2, height*tileSizeHeight*2);
        rectTransform.sizeDelta = size;
    }
    
    public Vector2Int GetTileGridPosition(Vector2 mousePosition)
    {
        //Bug SOMEWHERE
        positionOnTheGrid.x = mousePosition.x-rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        //Le diviser par 4 est la a cause du 0.25 scale sur la texture live
        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth)/4;
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight)/4;

        return tileGridPosition;
    }
}
