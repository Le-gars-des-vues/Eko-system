using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapRoom : MonoBehaviour
{
    [SerializeField] float mapUpgradeValue = 10;

    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().hasBuiltMap = true;
        MapManager.instance.maxZoom += mapUpgradeValue;
        MapManager.instance.maxDistanceFromOrigin += mapUpgradeValue;
    }

    private void Update()
    {
        if (GetComponent<RoomInfo>().isRefunded)
        {
            if (GameObject.Find("RoomMenu").GetComponent<RoomManager>().CheckForRoom(GetComponent<RoomInfo>().roomType))
                return;
            else
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().hasBuiltMap = false;

            MapManager.instance.maxZoom -= mapUpgradeValue;
            MapManager.instance.maxDistanceFromOrigin -= mapUpgradeValue;
        }
    }
}
