using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButtonReplace : MonoBehaviour
{
    public GameObject roomCrafting;
    public RoomManager roomManager;
    [SerializeField] GameObject theBase;

    GameObject children;
    PlayerPermanent player;
    [SerializeField] GameObject gameManager;

    bool isActive;

    [SerializeField] List<GameObject> availableRooms;

    private void OnEnable()
    {
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

        //Physics2D.IgnoreCollision(player.playerCollider, gameObject.GetComponent<BoxCollider2D>());
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
                            if (transform.parent.gameObject.GetComponent<RoomInfo>().sideIndex > 0)
                                availableRooms = Camera.main.GetComponent<InventoryController>().buildablesSide;
                            else
                                availableRooms = Camera.main.GetComponent<InventoryController>().buildablesDown;

                            var room = Instantiate(availableRooms[ingredientList.index], transform.parent.gameObject.transform.position, transform.parent.gameObject.transform.rotation);

                            AssignRefundMaterials(room, ingredientList);

                            room.GetComponent<RoomInfo>().Set(transform.parent.gameObject.GetComponent<RoomInfo>());

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

                            room.gameObject.transform.SetParent(theBase.transform.Find("Interior").transform);
                            room.gameObject.transform.SetAsLastSibling();
                            roomManager.rooms.Add(room);
                            Destroy(transform.parent.gameObject);
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
