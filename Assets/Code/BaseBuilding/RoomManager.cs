using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    [SerializeField] ItemGrid storageInventory;
    [SerializeField] ItemGrid playerInventory;
    PlayerPermanent player;
    [SerializeField] GameObject itemPrefab;
    public GameObject currentRoom;

    public ItemGrid duplicatingSlot;
    public List<ItemGrid> craftingSlots = new List<ItemGrid>();

    public List<GameObject> rooms = new List<GameObject>();

    // Start is called before the first frame update
    void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    public void RefundRoom()
    {
        RoomInfo roomInfo = currentRoom.GetComponent<RoomInfo>();
        if (roomInfo.firstMat != null)
        {
            for (int i = 0; i <roomInfo.firstMatQuantity; i++)
            {
                InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
                inventoryItem.Set(roomInfo.firstMat, playerInventory);
                bool placeAvailable = playerInventory.InsertItemAtRandom(inventoryItem);
                if (!placeAvailable)
                {
                    if (player.CanOpenStorage())
                    {
                        bool storageAvailable = storageInventory.InsertItemAtRandom(inventoryItem);
                        if (!storageAvailable)
                        {
                            Debug.Log("Not enough room in inventory");
                        }
                    }
                }
            }
        }
        if (roomInfo.secondMat != null)
        {
            for (int i = 0; i < roomInfo.secondMatQuantity; i++)
            {
                InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
                inventoryItem.Set(roomInfo.secondMat, playerInventory);
                bool placeAvailable = playerInventory.InsertItemAtRandom(inventoryItem);
                if (!placeAvailable)
                {
                    if (player.CanOpenStorage())
                    {
                        bool storageAvailable = storageInventory.InsertItemAtRandom(inventoryItem);
                        if (!storageAvailable)
                        {
                            Debug.Log("Not enough room in inventory");
                        }
                    }
                }
            }
        }
        if (roomInfo.thirdMat != null)
        {
            for (int i = 0; i < roomInfo.thirdMatQuantity; i++)
            {
                InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
                inventoryItem.Set(roomInfo.thirdMat, playerInventory);
                bool placeAvailable = playerInventory.InsertItemAtRandom(inventoryItem);
                if (!placeAvailable)
                {
                    if (player.CanOpenStorage())
                    {
                        bool storageAvailable = storageInventory.InsertItemAtRandom(inventoryItem);
                        if (!storageAvailable)
                        {
                            Debug.Log("Not enough room in inventory");
                        }
                    }
                }
            }
        }
        ReplaceRoom(currentRoom);
    }

    void ReplaceRoom(GameObject roomToDestroy)
    {
        int index = roomToDestroy.GetComponent<RoomInfo>().sideIndex > 0 ? 1 : 0;

        var room = Instantiate(Camera.main.GetComponent<InventoryController>().emptyRooms[index], currentRoom.transform.position, currentRoom.transform.rotation);
        room.GetComponent<RoomInfo>().Set(currentRoom.GetComponent<RoomInfo>());

        if (room.GetComponent<RoomInfo>().roomToTheLeft != null)
        {
            Destroy(room.GetComponent<RoomInfo>().sideWall);
        }

        if (room.GetComponent<RoomInfo>().roomUnder != null)
        {
            Destroy(room.GetComponent<RoomInfo>().elevatorFloor);
        }

        room.gameObject.transform.SetParent(GameObject.Find("Base").transform.Find("Interior").transform);
        room.gameObject.transform.SetAsLastSibling();

        Destroy(currentRoom);
    }
}
