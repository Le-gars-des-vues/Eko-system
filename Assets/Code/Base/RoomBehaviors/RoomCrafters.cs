using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCrafters : MonoBehaviour
{

    public ItemGrid duplicatingSlot;
    public ItemGrid[] craftingSlots;

    public InventoryItem itemToDuplicateSave;
    public InventoryItem[] fabricatedItems;
    public void SetItemToDuplicate()
    {
        if (duplicatingSlot.GetItem(1, 1) != null)
        {
            itemToDuplicateSave = duplicatingSlot.GetItem(1, 1);

            duplicatingSlot.GetItem(1, 1).Delete();
            for (int i = 0; i <= craftingSlots.Length - 1; i++)
            {
                fabricatedItems[i] = craftingSlots[i].GetItem(1, 1);
                craftingSlots[i].GetItem(1, 1).Delete();
            }
        }
    }

    public void SetItemToShowInSlot()
    {
        if (itemToDuplicateSave != null)
        {
            duplicatingSlot.PlaceItem(Instantiate(itemToDuplicateSave), 1, 1);

            for (int i = 0; i <= craftingSlots.Length - 1; i++)
            {
                if (fabricatedItems[i] != null)
                {
                    craftingSlots[i].PlaceItem(Instantiate(fabricatedItems[i]), 1, 1);
                }
            }
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
