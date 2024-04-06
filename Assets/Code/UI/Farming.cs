using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Farming : MonoBehaviour
{
    [SerializeField] GameObject planting;
    [SerializeField] GameObject cloning;

    public ItemGrid farmingSlot;
 
    public void Activate(bool isFarming)
    {
        planting.SetActive(isFarming);
        cloning.SetActive(!isFarming);
    }

    public void Plant()
    {
        RoomManager.instance.currentRoom.GetComponent<RoomInfo>().interactiveObject.GetComponent<Planters>().Plant();
    }
}
