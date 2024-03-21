using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom : MonoBehaviour
{
    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().hasBuiltMap = true;
    }

    private void Update()
    {
        if (GetComponent<RoomInfo>().isRefunded)
        {
            if (GameObject.Find("RoomMenu").GetComponent<RoomManager>().CheckForRoom(GetComponent<RoomInfo>().roomType))
                return;
            else
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().hasBuiltMap = false;
        }
    }
}
