using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButton : MonoBehaviour
{
    public GameObject roomAmountDataHolder;
    [SerializeField] GameObject theBase;
    [SerializeField] GameObject elevatorFloor;
    [SerializeField] GameObject sideWall;

    [SerializeField] float upOffset = 7.6f;
    [SerializeField] float sideOffset = 3.2f;

    GameObject children;
    PlayerPermanent player;
    [SerializeField] GameObject cycle;

    bool isActive;
    public bool newFloor;
    public int floorIndex = 0;

    [SerializeField] List<GameObject> availableRooms = new List<GameObject>();

    private void OnEnable()
    {
        cycle = GameObject.Find("Cycle");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        roomAmountDataHolder = GameObject.Find("RoomCrafting").transform.Find("DropdownListOfCrafts").gameObject;
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
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 raycastPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                RaycastHit2D hit = Physics2D.Raycast(raycastPos, Vector2.zero);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        var index = roomAmountDataHolder.GetComponent<RoomCrafting>().RoomCraft();
                        if (index >= 0)
                        {
                            var room = Instantiate(availableRooms[index], transform.position, transform.rotation);
                            if (newFloor)
                            {
                                if (floorIndex > 0)
                                    Destroy(elevatorFloor);
                                room.transform.Find("BuildButtonDown").GetComponent<BuildButton>().floorIndex = floorIndex + 1;
                                room.transform.position = new Vector2(transform.position.x, transform.position.y + upOffset);
                            }
                            else
                            {
                                Destroy(sideWall);
                                room.transform.Find("BuildButtonSide").GetComponent<BuildButton>().floorIndex = floorIndex;
                                room.transform.position = new Vector2(transform.position.x + sideOffset, transform.position.y + 6.8f);
                            }
                            if (index == 0 || index == 1)
                                cycle.GetComponent<Cycle>().theRooms.Add(room);
                            room.gameObject.transform.SetParent(theBase.transform);
                            room.gameObject.transform.SetAsLastSibling();
                        }
                        Destroy(gameObject);
                    }
                }
            }
        }
    }
}
