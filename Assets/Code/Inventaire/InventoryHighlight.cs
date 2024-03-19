using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHighlight : MonoBehaviour
{
    public RectTransform highlighter;


    public void Show(bool b)
    {

        highlighter.gameObject.SetActive(b);
    }



    public void SetSize(InventoryItem targetItem, ItemGrid itemGrid)
    {
        //CHANGEMENT ICI ------------------------
        Vector2 size = new Vector2();
        size.x = targetItem.WIDTH * itemGrid.tileSizeWidth;
        size.y = targetItem.HEIGHT * itemGrid.tileSizeWidth;
        highlighter.sizeDelta = size;
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        SetParent(targetGrid);

        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, targetItem.onGridPositionX, targetItem.onGridPositionY);


        highlighter.localPosition = pos;
    }

    public void SetParent(ItemGrid targetGrid)
    {
        if(targetGrid == null) { return; }
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }

    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(targetItem, posX, posY);

        highlighter.localPosition = pos;
    }
}
