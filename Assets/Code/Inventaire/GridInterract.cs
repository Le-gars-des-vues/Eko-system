using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


[RequireComponent(typeof(ItemGrid))]
public class GridInterract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    ItemGrid itemGrid;
    [SerializeField] Transform canvasTransform;
    RectTransform rectTransform;

    public void OnPointerEnter(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid = itemGrid;
        if(inventoryController.selectedItem != null && inventoryController.SelectedItemGrid.gameObject.tag == "Hotbar")
        {
            inventoryController.selectedItem.itemData.height = inventoryController.selectedItem.itemData.hotbarHeight;
            inventoryController.selectedItem.itemData.width = inventoryController.selectedItem.itemData.hotbarWidth;
        }
        if (inventoryController.selectedItem != null)
        {
            rectTransform = inventoryController.selectedItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            rectTransform.SetAsLastSibling();
        }
        
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        inventoryController.SelectedItemGrid=null;
        if (inventoryController.selectedItem != null)
        {
            inventoryController.selectedItem.itemData.height = inventoryController.selectedItem.itemData.initialHeight;
            inventoryController.selectedItem.itemData.width = inventoryController.selectedItem.itemData.initialWidth;
        }
    }

    private void Awake()
    {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        itemGrid = GetComponent<ItemGrid>();
    }
}
