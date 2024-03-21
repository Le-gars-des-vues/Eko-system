using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowFarming : MonoBehaviour
{
    bool isActive = false;
    int index = 0;
    InventoryItem item;
    RoomManager room;

    private void OnEnable()
    {
        item = GetComponent<InventoryItem>();
        room = GameObject.Find("RoomMenu").GetComponent<RoomManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetComponent<InventoryItem>().itemGrid.name == "WhatToCraft1")
            index = 1;
        else if (GetComponent<InventoryItem>().itemGrid.name == "WhatToCraft2")
            index = 2;

        if (item.isPlaced && !isActive)
        {
            isActive = true;
            //ActivateVisual(true);
        }
        else if (!item.isPlaced && isActive)
        {
            isActive = false;
            //ActivateVisual(false);
        }
    }
    /*
    void ActivateVisual(bool activated)
    {
        switch (item.itemData.itemName)
        {
            case "Infpisum Pine":
                if (activated)
                {
                    room.currentRoom.GetComponent<Planters>().AddOrRemovePlant(true, index, "Infpisum Pine");
                    break;
                }
                else
                {
                    room.currentRoom.GetComponent<Planters>().AddOrRemovePlant(false, index, "Infpisum Pine");
                    break;
                }
            case "Macrebosia Nut":
                if (activated)
                {
                    room.currentRoom.GetComponent<Planters>().AddOrRemovePlant(true, index, "Macrebosia Nut");
                    break;
                }
                else
                {
                    room.currentRoom.GetComponent<Planters>().AddOrRemovePlant(false, index, "Macrebosia Nut");
                    break;
                }
            case "Caeruletam Leaf":
                if (activated)
                {
                    room.currentRoom.GetComponent<Planters>().AddOrRemovePlant(true, index, "Caeruletam Leaf");
                    break;
                }
                else
                {
                    room.currentRoom.GetComponent<Planters>().AddOrRemovePlant(false, index, "Caeruletam Leaf");
                    break;
                }
        }
    }
    */
}
