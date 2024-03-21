using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCrafters : MonoBehaviour
{
    const int DUPLICATING_SLOTS = 2;

    public List<ItemGrid> duplicatingSlots = new List<ItemGrid>(DUPLICATING_SLOTS);
    public List<ItemGrid> craftingSlots = new List<ItemGrid>();

    public List<InventoryItem> itemsToDuplicateSave = new List<InventoryItem>(DUPLICATING_SLOTS);
    public InventoryItem[] fabricatedItems;
    int slotsCount;

    [SerializeField] int growthTime = 1;
    List<int> cycleIndexes = new List<int>(DUPLICATING_SLOTS);

    /*
    void OnEnable()
    {
        foreach (GameObject grid in GameObject.FindGameObjectsWithTag("RoomCrafting"))
        {
            craftingSlots.Add(grid.GetComponent<ItemGrid>());
        }
        fabricatedItems = new InventoryItem[craftingSlots.Count];
        slotsCount = GameObject.Find("RoomMenu").GetComponent<RoomManager>().duplicatingSlots.Count;
        for (int i = 0; i < DUPLICATING_SLOTS; i++)
        {
            cycleIndexes[i] = growthTime;
        }
    }

    public void SetItemToDuplicate()
    {
        for (int i = 0; i < slotsCount; i++)
        {
            if (duplicatingSlots[i].GetItem(0, 0) != null)
            {
                itemsToDuplicateSave[i] = Instantiate(duplicatingSlots[i].GetItem(0, 0));

                duplicatingSlots[i].GetItem(0, 0).Delete();
                for (int k = 0; k <= craftingSlots.Count - 1; k++)
                {
                    if (craftingSlots[k].GetItem(0, 0) != null)
                    {
                        fabricatedItems[k] = Instantiate(craftingSlots[k].GetItem(0, 0));
                        craftingSlots[k].GetItem(0, 0).Delete();
                    }
                }
            }
        }
    }

    public void SetItemToShowInSlot()
    {
        for (int i = 0; i < slotsCount; i++)
        {
            if (itemsToDuplicateSave[i] != null)
            {
                duplicatingSlots[i].PlaceItem(Instantiate(itemsToDuplicateSave[i]), 0, 0);

                for (int k = 0; k <= craftingSlots.Count - 1; k++)
                {
                    if (fabricatedItems[k] != null)
                    {
                        craftingSlots[k].PlaceItem(Instantiate(fabricatedItems[k]), 0, 0);
                        craftingSlots[k].gameObject.transform.Find("GreyedOut").gameObject.SetActive(false);
                    }
                    else
                    {
                        craftingSlots[k].gameObject.transform.Find("GreyedOut").gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    public void SpawnExtraItem()
    {
        for (int i = 0; i < slotsCount; i++)
        {
            if (itemsToDuplicateSave[i] != null)
            {
                for (int k = 0; k <= craftingSlots.Count - 1; k++)
                {
                    if (fabricatedItems[k] == null)
                    {
                        fabricatedItems[k] = itemsToDuplicateSave[i];
                        k = craftingSlots.Count;
                    }
                }
            }
        }
    }
    */
}
