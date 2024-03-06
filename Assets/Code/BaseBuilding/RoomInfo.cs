using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public int roomType; // 0=Empty, 1=Farm, 2=Enclos

    public GameObject itemFarm;
    public GameObject itemEnclos;

    public GameObject rightWall;
    public GameObject nextRoom;

    public void UndoWall()
    {
        if (nextRoom != null)
        {
            nextRoom.GetComponent<RoomInfo>().rightWall.SetActive(false);
        }
    }
    public void ChangementSalle(int laSalle)
    {
        if (laSalle == 0)
        {
            roomType = laSalle;
            
                itemFarm.SetActive(false);
                itemEnclos.SetActive(false);
        }
        else if (laSalle == 1)
        {
            roomType = laSalle;

                itemFarm.SetActive(true);

                itemEnclos.SetActive(false);

            UndoWall();
        }
        else if(laSalle == 2)
        {
            roomType = laSalle;

                itemFarm.SetActive(false);

                itemEnclos.SetActive(true);

            UndoWall();
        }
    }
}
