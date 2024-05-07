using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageRoom : MonoBehaviour
{
    private void OnEnable()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().hasBuiltStorage = true;
        StorageManager.instance.storageCount++;
        if (StorageManager.instance.storageCount >= 2)
        {
            StorageManager.instance.buttonLeft.SetActive(true);
            StorageManager.instance.buttonRight.SetActive(true);
        }
    }

    private void Update()
    {
        if (GetComponent<RoomInfo>().isRefunded)
        {
            if (StorageManager.instance.storageIndex == StorageManager.instance.storageCount)
                StorageManager.instance.storageIndex--;
            StorageManager.instance.storageCount--;
            if (StorageManager.instance.storageCount < 2)
            {
                StorageManager.instance.buttonLeft.SetActive(false);
                StorageManager.instance.buttonRight.SetActive(false);
            }
            if (GameObject.Find("RoomMenu").GetComponent<RoomManager>().CheckForRoom(GetComponent<RoomInfo>().roomType))
                return;
            else
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>().hasBuiltStorage = false;
        }
    }
}
