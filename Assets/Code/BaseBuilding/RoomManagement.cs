using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManagement : MonoBehaviour
{
    public GameObject myRoom;
    PlayerPermanent player;

    public RoomManager roomMenu;
    [SerializeField] float distanceThreshold;

    bool isInRange;
 
    private void OnEnable()
    {
        roomMenu = GameObject.Find("RoomMenu").GetComponent<RoomManager>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    private void Update()
    {
        if (Vector2.Distance(GetComponent<SpriteRenderer>().sprite.bounds.center, player.gameObject.transform.position) < distanceThreshold)
        {
            isInRange = true;
            if (!ArrowManager.instance.isActive)
                ArrowManager.instance.PlaceArrow(GetComponent<SpriteRenderer>().sprite.bounds.center, "MANAGE ROOM", new Vector2(0, 1), gameObject, 1);
        }
        else
        {
            isInRange = false;
            if (ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();
        }

        if (!player.roomManageIsOpen)
        {
            if (isInRange)
            {
                if (Input.GetKey(KeyCode.E) && ArrowManager.instance.targetObject == gameObject && ArrowManager.instance.readyToActivate)
                {
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
                if (player.roomManageIsOpen)
                {
                    player.ShowOrHideRoomManagement();
                }
            }
        }
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            /*
            if (myRoom.GetComponent<RoomCrafters>() != null)
            {
                myRoom.GetComponent<RoomCrafters>().duplicatingSlots = roomMenu.GetComponent<RoomManager>().duplicatingSlots;
                myRoom.GetComponent<RoomCrafters>().craftingSlots = roomMenu.GetComponent<RoomManager>().craftingSlots;
                myRoom.GetComponent<RoomCrafters>().SetItemToShowInSlot();
            }
            */
            roomMenu.currentRoom = myRoom;
            roomMenu.roomName.text = myRoom.GetComponent<RoomInfo>().roomType;
            roomMenu.roomDesc.text = myRoom.GetComponent<RoomInfo>().roomDesc;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            roomMenu.currentRoom = null;
            roomMenu.roomName.text = " ";
            roomMenu.roomDesc.text = " ";
            /*
            if (myRoom.GetComponent<RoomCrafters>() != null)
            {
                myRoom.GetComponent<RoomCrafters>().SetItemToDuplicate();
                //myRoom.GetComponent<RoomCrafters>().duplicatingSlot = null;
                //myRoom.GetComponent<RoomCrafters>().craftingSlots.Clear();
            }
            */
        }
    }
}
