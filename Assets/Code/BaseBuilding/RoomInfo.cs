using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public int roomType; // 0=Empty, 1=Farm, 2=Enclos

    public List<GameObject> itemFarm;
    public List<GameObject> itemEnclos;

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
            for (int i = 0; i == itemFarm.Count - 1; i++) {
                itemFarm[i].SetActive(false);
            }
            for (int i = 0; i == itemEnclos.Count - 1; i++)
            {
                itemEnclos[i].SetActive(false);
            }
        }
        else if (laSalle == 1)
        {
            roomType = laSalle;
            for (int i = 0; i == itemFarm.Count - 1; i++)
            {
                itemFarm[i].SetActive(true);
            }
            for (int i = 0; i == itemEnclos.Count - 1; i++)
            {
                itemEnclos[i].SetActive(false);
            }
            UndoWall();
        }
        else if(laSalle == 2)
        {
            roomType = laSalle;
            for (int i = 0; i == itemFarm.Count - 1; i++)
            {
                itemFarm[i].SetActive(false);
            }
            for (int i = 0; i == itemEnclos.Count - 1; i++)
            {
                itemEnclos[i].SetActive(true);
            }
            UndoWall();
        }
    }
}
