using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;
    public int stackAmount;
    public Sprite[] sprites;

    public bool markedForDestroy;

    public int HEIGHT
    {
        get { 
            if (rotated == false)
            {
                return itemData.height;
            }
            return itemData.width;
        } 
    }

    public int WIDTH
    {
        get
        {
            if (rotated == false)
            {
                return itemData.width;
            }
            return itemData.height;
        }
    }


    public int onGridPositionX;
    public int onGridPositionY;

    public bool rotated = false;

    internal void Rotate()
    {
        rotated = !rotated;

        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.rotation = Quaternion.Euler(0,0,rotated == true ? 90f : 0f);
    }

    private void OnEnable()
    {
        markedForDestroy = false;
    }

    internal void Set(ItemData itemData, ItemGrid itemGrid)
    {
        this.itemData = itemData;

        GetComponent<Image>().sprite = itemData.itemIcon;

        //CHANGEMENT ICI ---------------------------
        Vector2 size = new Vector2();
        size.x = WIDTH * itemGrid.tileSizeWidth;
        size.y = HEIGHT * itemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
