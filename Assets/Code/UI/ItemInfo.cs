using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfo : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemValue;
    public TextMeshProUGUI itemDurability;
    public TextMeshProUGUI itemDesc;
    public Image itemImage;
    public InventoryItem referencedItem;
    [SerializeField] Color lowDurability;
    [SerializeField] Color broken;

    public void Set(ItemData itemData)
    {
        itemName.text = itemData.itemName;
        itemValue.text = "VALUE: " + itemData.value.ToString() + "$";
        itemDesc.text = itemData.description;
        itemImage.sprite = itemData.itemIcon;
        if (itemData.useDurability)
        {
            itemDurability.text = "DURABILITY: " + referencedItem.currentDurability.ToString() + " / " + referencedItem.maxDurability.ToString();
            if (referencedItem.broken)
                itemDurability.color = broken;
            else if (referencedItem.lowDurability)
                itemDurability.color = lowDurability;
        }
        else
        {
            itemDurability.text = "";
        }
    }
}
