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
    public int maxStack = 5;
    public Sprite[] sprites;

    public bool markedForDestroy;

    public bool isPlaced = false;
    public bool isInInventory = false;

    public int currentDurability;
    public int maxDurability = 20;
    public int lowDurabilityThreshold;
    public bool lowDurability;
    public bool broken;

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

    private void Update()
    {
        if (currentDurability <= 0 && !broken)
            broken = true;
        else if (currentDurability <= lowDurabilityThreshold && !lowDurability)
            lowDurability = true;

        if (isInInventory)
        {
            if (broken)
            {
                transform.GetChild(1).GetComponent<Image>().enabled = true;
                transform.GetChild(0).GetComponent<Image>().enabled = false;
            }
            else if (lowDurability)
                transform.GetChild(0).GetComponent<Image>().enabled = true;
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

        lowDurabilityThreshold = maxDurability / 3;
        currentDurability = maxDurability;
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
        {
            GetComponent<Image>().sprite = sprites[sprites.Length - stackAmount];
        }

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
            itemToDrop.GetComponent<InventoryItem>().currentDurability = currentDurability;
            if (broken)
                itemToDrop.GetComponent<InventoryItem>().broken = true;
            else if (lowDurability)
                itemToDrop.GetComponent<InventoryItem>().lowDurability = true;

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
