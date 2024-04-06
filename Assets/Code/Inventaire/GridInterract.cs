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
    PlayerPermanent player;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (player.uiOpened)
        {
            inventoryController.SelectedItemGrid = itemGrid;
            if (inventoryController.selectedItem != null && (inventoryController.SelectedItemGrid.gameObject.tag == "Hotbar" || inventoryController.SelectedItemGrid.gameObject.tag == "Upgrade" || inventoryController.SelectedItemGrid.gameObject.tag == "Farming"))
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
