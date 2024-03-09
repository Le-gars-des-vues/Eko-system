using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCrafters : MonoBehaviour
{
    [SerializeField] private RectTransform inventaireADupliquer;

    /*private int tempHeight;
    private int tempWidth;
    ItemGrid theItemGrid;
    InventoryItem anItem;*/

    public ItemGrid duplicatingSlot;
    public ItemGrid[] craftingSlots;
    public List<ItemGrid> grids;

    public InventoryItem itemToDuplicateSave;
    public InventoryItem[] fabricatedItems;

    private void Start()
    {
        grids = new List<ItemGrid>();
        for (int i = 0; i < craftingSlots.Length; i++)
        {
            grids.Add(craftingSlots[i].GetComponent<ItemGrid>());
        }
        
    }

    public void SetItemToDuplicate()
    {
        itemToDuplicateSave = duplicatingSlot.GetItem(1, 1);

        duplicatingSlot.GetItem(1, 1).Delete();

        for(int i =0; i <= craftingSlots.Length-1; i++)
        {
            fabricatedItems[i]=craftingSlots[i].GetItem(1, 1);
            craftingSlots[i].GetItem(1, 1).Delete();
        }

    }

    public void SetItemToShowInSlot()
    {
        duplicatingSlot.PlaceItem(Instantiate(itemToDuplicateSave), 1, 1);

        for (int i = 0; i <= craftingSlots.Length - 1; i++)
        {
             craftingSlots[i].PlaceItem(Instantiate(fabricatedItems[i]),1, 1);
        }
    }

    public void SpawnExtraItem()
    {
        if (itemToDuplicateSave!=null)
        {
            for (int i = 0;i <= craftingSlots.Length - 1; i++)
            {
                if (fabricatedItems[i]==null)
                {
                    fabricatedItems[i] = itemToDuplicateSave;
                    i = craftingSlots.Length;
                }

            }
        }

        
    }
}
