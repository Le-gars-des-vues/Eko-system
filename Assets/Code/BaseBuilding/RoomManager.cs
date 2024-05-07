using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.SceneManagement;

public class RoomManager : MonoBehaviour
{
    public static RoomManager instance;

    [SerializeField] ItemGrid storageInventory;
    [SerializeField] ItemGrid playerInventory;
    PlayerPermanent player;
    [SerializeField] GameObject itemPrefab;
    public GameObject currentRoom;

    //public List<ItemGrid> duplicatingSlots = new List<ItemGrid>();
    //public List<ItemGrid> craftingSlots = new List<ItemGrid>();

    public List<GameObject> rooms = new List<GameObject>();
    public TextMeshProUGUI roomName;
    public TextMeshProUGUI roomDesc;

    private void Awake()
    {
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;

        SceneLoader.allScenesLoaded += StartScript;
    }

    // Start is called before the first frame update
    void StartScript()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    public void RefundRoom()
    {
        RoomInfo roomInfo = currentRoom.GetComponent<RoomInfo>();
        roomInfo.isRefunded = true;
        if (roomInfo.firstMat != null)
        {
            for (int i = 0; i <roomInfo.firstMatQuantity; i++)
            {
                InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
                inventoryItem.Set(roomInfo.firstMat, playerInventory);
                bool placeAvailable = playerInventory.InsertItem(inventoryItem);
                if (!placeAvailable)
                {
                    if (player.CanOpenStorage())
                    {
                        bool storageAvailable = storageInventory.InsertItem(inventoryItem);
                        if (!storageAvailable)
                        {
                            inventoryItem.DropItem();
                        }
                    }
                    else
                        inventoryItem.DropItem();
                }
            }
        }
        if (roomInfo.secondMat != null)
        {
            for (int i = 0; i < roomInfo.secondMatQuantity; i++)
            {
                InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
                inventoryItem.Set(roomInfo.secondMat, playerInventory);
                bool placeAvailable = playerInventory.InsertItem(inventoryItem);
                if (!placeAvailable)
                {
                    if (player.CanOpenStorage())
                    {
                        bool storageAvailable = storageInventory.InsertItem(inventoryItem);
                        if (!storageAvailable)
                        {
                            inventoryItem.DropItem();
                        }
                    }
                    else
                        inventoryItem.DropItem();
                }
            }
        }
        if (roomInfo.thirdMat != null)
        {
            for (int i = 0; i < roomInfo.thirdMatQuantity; i++)
            {
                InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
                inventoryItem.Set(roomInfo.thirdMat, playerInventory);
                bool placeAvailable = playerInventory.InsertItem(inventoryItem);
                if (!placeAvailable)
                {
                    if (player.CanOpenStorage())
                    {
                        bool storageAvailable = storageInventory.InsertItem(inventoryItem);
                        if (!storageAvailable)
                        {
                            inventoryItem.DropItem();
                        }
                    }
                    else
                        inventoryItem.DropItem();
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
            Destroy(transform.Find("BuildButtonSide").gameObject);
            Destroy(room.GetComponent<RoomInfo>().sideWall);
        }

        if (room.GetComponent<RoomInfo>().roomUnder != null)
        {
            Destroy(transform.Find("BuildButtonDown").gameObject);
            Destroy(room.GetComponent<RoomInfo>().elevatorFloor);
        }

        room.gameObject.transform.SetParent(GameObject.Find("Base").transform.Find("Interior").transform);
        room.gameObject.transform.SetAsLastSibling();

        rooms.Remove(currentRoom);
        Destroy(currentRoom);
    }

    public bool CheckForRoom(string roomType)
    {
        foreach(GameObject room in rooms)
        {
            if (room.GetComponent<RoomInfo>().roomType == roomType)
                return true;
        }
        return false;
    }
}
