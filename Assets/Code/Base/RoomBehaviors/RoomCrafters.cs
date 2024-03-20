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
        fabricatedItems = new InventoryItem[craftingSlots.Count];
    }

    public void SetItemToDuplicate()
    {
        if (duplicatingSlot.GetItem(0, 0) != null)
        {
            itemToDuplicateSave = Instantiate(duplicatingSlot.GetItem(0, 0));

            duplicatingSlot.GetItem(0, 0).Delete();
            for (int i = 0; i <= craftingSlots.Count - 1; i++)
            {
                if (craftingSlots[i].GetItem(0, 0) != null)
                {
                    fabricatedItems[i] = Instantiate(craftingSlots[i].GetItem(0, 0));
                    craftingSlots[i].GetItem(0, 0).Delete();
                }
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
                    craftingSlots[i].gameObject.transform.Find("GreyedOut").gameObject.SetActive(false);
                }
                else
                {
                    craftingSlots[i].gameObject.transform.Find("GreyedOut").gameObject.SetActive(true);
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
                if (fabricatedItems[i] == null)
                {
                    fabricatedItems[i] = itemToDuplicateSave;
                    i = craftingSlots.Count;
                }
            }
        }
    }
}
