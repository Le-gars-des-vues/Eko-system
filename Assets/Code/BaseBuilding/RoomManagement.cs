using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManagement : MonoBehaviour
{
    public GameObject myRoom;
    PlayerPermanent player;

    public GameObject roomMenu;

    bool isInRange;
    [SerializeField] GameObject arrow;
 
    private void OnEnable()
    {
        roomMenu = GameObject.Find("RoomMenu");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    private void Update()
    {
        if (!player.roomManageIsOpen)
        {
            if (isInRange)
            {
                if (Input.GetKey(KeyCode.E) && arrow.GetComponent<ArrowFill>().readyToActivate)
                {
                    if (!player.inventoryOpen)
                    {
                        player.ShowOrHideInventoryNoButtons();
                    }
                    if (!player.roomManageIsOpen)
                    {
                        player.ShowOrHideRoomManagement();
                    }
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (player.inventoryOpen && player.roomManageIsOpen)
                {
                    player.ShowOrHideInventoryNoButtons();
                    player.ShowOrHideRoomManagement();
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (myRoom.GetComponent<RoomCrafters>() != null)
            {
                myRoom.GetComponent<RoomCrafters>().duplicatingSlot = roomMenu.GetComponent<RoomManager>().duplicatingSlot;
                myRoom.GetComponent<RoomCrafters>().craftingSlots = roomMenu.GetComponent<RoomManager>().craftingSlots;
                myRoom.GetComponent<RoomCrafters>().SetItemToShowInSlot();
            }

            isInRange = true;
            roomMenu.GetComponent<RoomManager>().currentRoom = myRoom;
            arrow.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            arrow.SetActive(false);
            roomMenu.GetComponent<RoomManager>().currentRoom = null;

            if (myRoom.GetComponent<RoomCrafters>() != null)
            {
                myRoom.GetComponent<RoomCrafters>().SetItemToDuplicate();
                //myRoom.GetComponent<RoomCrafters>().duplicatingSlot = null;
                //myRoom.GetComponent<RoomCrafters>().craftingSlots.Clear();
            }
        }
    }
}
