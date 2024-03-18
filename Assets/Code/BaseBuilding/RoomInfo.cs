using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    [Header("Room Variables")]
    public int floorIndex;
    public int sideIndex;

    public GameObject roomToTheRight;
    public GameObject roomToTheLeft;
    public GameObject roomAbove;
    public GameObject roomUnder;

    public GameObject elevatorFloor;
    public GameObject sideWall;

    [Header("Materials Variables")]
    public ItemData firstMat;
    public int firstMatQuantity;

    public ItemData secondMat;
    public int? secondMatQuantity;

    public ItemData thirdMat;
    public int? thirdMatQuantity;

    public void Set(RoomInfo roomInfo)
    {
        floorIndex = roomInfo.floorIndex;
        sideIndex = roomInfo.sideIndex;

        roomToTheRight = roomInfo.roomToTheRight;
        roomToTheLeft = roomInfo.roomToTheLeft;
        roomAbove = roomInfo.roomAbove;
        roomUnder = roomInfo.roomUnder;
    }
}
