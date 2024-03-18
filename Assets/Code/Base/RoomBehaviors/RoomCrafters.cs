using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCrafters : MonoBehaviour
{
    public ItemGrid duplicatingSlot;
    public List<ItemGrid> craftingSlots = new List<ItemGrid>();

    public InventoryItem itemToDuplicateSave;
    public InventoryItem[] fabricatedItems;

    void OnEnable()
    {
        foreach (GameObject grid in GameObject.FindGameObjectsWithTag("RoomCrafting"))
        {
            craftingSlots.Add(grid.GetComponent<ItemGrid>());
        }
    }

    public void SetItemToDuplicate()
    {
        if (duplicatingSlot.GetItem(0, 0) != null)
        {
            itemToDuplicateSave = duplicatingSlot.GetItem(0, 0);

            duplicatingSlot.GetItem(0, 0).Delete();
            for (int i = 0; i <= craftingSlots.Count - 1; i++)
            {
                fabricatedItems[i] = craftingSlots[i].GetItem(0, 0);
                craftingSlots[i].GetItem(0, 0).Delete();
            }
        }
    }

    public void SetItemToShowInSlot()
    {
        if (itemToDuplicateSave != null)
        {
            duplicatingSlot.PlaceItem(Instantiate(itemToDuplicateSave), 0, 0);

            for (int i = 0; i <= craftingSlots.Count - 1; i++)
            {
                if (fabricatedItems[i] != null)
                {
                    craftingSlots[i].PlaceItem(Instantiate(fabricatedItems[i]), 0, 0);
                }
            }
        }
    }

    public void SpawnExtraItem()
    {
        if (itemToDuplicateSave!=null)
        {
            for (int i = 0;i <= craftingSlots.Count - 1; i++)
            {
                if (fabricatedItems[i]==null)
                {
                    fabricatedItems[i] = itemToDuplicateSave;
                    i = craftingSlots.Count;
                }
            }
        }
    }
}
