using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemInfo : MonoBehaviour
{
    public TextMeshProUGUI itemName;
    public TextMeshProUGUI itemValue;
    public TextMeshProUGUI itemDesc;
    public Image itemImage;
    public InventoryItem referenceditem;

    public void Set(ItemData itemData)
    {
        itemName.text = itemData.itemName;
        itemValue.text = "VALUE: " + itemData.value.ToString() + "$";
        itemDesc.text = itemData.description;

        itemImage.sprite = itemData.itemIcon;
    }
}
