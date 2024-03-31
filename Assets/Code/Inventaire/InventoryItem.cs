using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public ItemData itemData;
    public ItemGrid itemGrid;
    public int stackAmount;
    public Sprite[] sprites;

    public bool markedForDestroy;

    public bool isPlaced = false;

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
        if (itemData.hasScript)
        {
            this.gameObject.AddComponent(System.Type.GetType(itemData.scriptToAttach));
        }
        if (sprites.Length <= 0)
            GetComponent<Image>().sprite = itemData.itemIcon;
        else
            GetComponent<Image>().sprite = sprites[stackAmount - 1];

        Vector2 size = new Vector2();
        size.x = WIDTH * itemGrid.tileSizeWidth;
        size.y = HEIGHT * itemGrid.tileSizeHeight;
        GetComponent<RectTransform>().sizeDelta = size;
    }

    public void DropItem()
    {
        if (gameObject.tag != "Gear")
        {
            PlayerPermanent player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
            float facingDirection = player.isFacingRight ? 1 : -1;
            var itemToDrop = Instantiate(this.itemData.objectToSpawn, (Vector2)player.gameObject.transform.position + (Vector2.right * facingDirection), Quaternion.identity);
            itemToDrop.GetComponent<InventoryItem>().itemData = this.itemData;
            itemToDrop.GetComponent<InventoryItem>().stackAmount = stackAmount;
            Destroy(gameObject);
        }
        else
            Debug.Log("Cannot drop this object!");
    }

    public void Delete()
    {
        Destroy(this.gameObject);
    }
}
