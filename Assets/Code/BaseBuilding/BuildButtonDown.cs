using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButtonDown : MonoBehaviour
{
    public GameObject roomCrafting;
    public RoomManager roomManager;
    [SerializeField] GameObject theBase;

    [SerializeField] float upOffset = 7.6f;

    GameObject children;
    PlayerPermanent player;
    [SerializeField] GameObject gameManager;

    bool isActive;
    public bool firstFloor = false;

    [SerializeField] List<GameObject> availableRooms;

    private void OnEnable()
    {
        availableRooms = Camera.main.GetComponent<InventoryController>().buildablesDown;
        gameManager = GameObject.Find("GameManager");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        roomCrafting = GameObject.Find("RoomCrafting").transform.Find("DropdownListOfCrafts").gameObject;
        roomManager = GameObject.Find("RoomMenu").GetComponent<RoomManager>();
        theBase = GameObject.FindGameObjectWithTag("Base");
        isActive = false;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        children = gameObject.transform.Find("BuildIcon").gameObject;
        children.GetComponent<SpriteRenderer>().enabled = false;
    }

    private void Update()
    {
        if (player.buildingIsOpen && !isActive)
        {
            isActive = true;
            GetComponent<SpriteRenderer>().enabled = true;
            GetComponent<BoxCollider2D>().enabled = true;
            children.GetComponent<SpriteRenderer>().enabled = true;
        }
        else if (!player.buildingIsOpen && isActive)
        {
            isActive = false;
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<BoxCollider2D>().enabled = false;
            children.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (player.buildingIsOpen)
        {
            Vector2 raycastPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(raycastPos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                gameObject.GetComponent<Animator>().SetBool("isSelected", true);
            }
            else
                gameObject.GetComponent<Animator>().SetBool("isSelected", false);

            if (Input.GetMouseButtonDown(0))
            {
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        Ingredients ingredientList = roomCrafting.GetComponent<RoomCrafting>().RoomCraft();
                        if (ingredientList.index >= 0)
                        {
                            var room = Instantiate(availableRooms[ingredientList.index], transform.position, transform.rotation);

                            AssignRefundMaterials(room, ingredientList);

                            if (transform.parent.gameObject.GetComponent<RoomInfo>().floorIndex > 0)
                                Destroy(transform.parent.gameObject.GetComponent<RoomInfo>().elevatorFloor);

                            room.GetComponent<RoomInfo>().floorIndex = transform.parent.gameObject.GetComponent<RoomInfo>().floorIndex + 1;
                            room.GetComponent<RoomInfo>().sideIndex = 0;
                            room.transform.position = new Vector2(transform.position.x, transform.position.y + upOffset);

                            transform.parent.gameObject.GetComponent<RoomInfo>().roomUnder = room;
                            room.GetComponent<RoomInfo>().roomAbove = transform.parent.gameObject;

                            if (ingredientList.index == 2 || ingredientList.index == 3)
                                gameManager.GetComponent<GameManager>().theRooms.Add(room);

                            room.gameObject.transform.SetParent(theBase.transform.Find("Interior").transform);
                            room.gameObject.transform.SetAsLastSibling();
                            roomManager.rooms.Add(room);
                            Destroy(gameObject);
                        }
                    }
                }
            }
        }
    }

    void AssignRefundMaterials(GameObject room, Ingredients ingredientList)
    {
        RoomInfo roomInfo = room.GetComponent<RoomInfo>();
        if (ingredientList.firstMat != null)
        {
            roomInfo.firstMat = ingredientList.firstMat;
            roomInfo.firstMatQuantity = ingredientList.firstMatQuantity;
        }
        if (ingredientList.secondMat != null)
        {
            roomInfo.secondMat = ingredientList.secondMat;
            roomInfo.secondMatQuantity = ingredientList.secondMatQuantity;
        }
        if (ingredientList.thirdMat != null)
        {
            roomInfo.thirdMat = ingredientList.thirdMat;
            roomInfo.thirdMatQuantity = ingredientList.thirdMatQuantity;
        }
    }
}
