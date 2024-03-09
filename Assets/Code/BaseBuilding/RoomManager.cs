using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    public GameObject myRoom;
    public GameObject roomAmountDataHolder;

    public GameObject roomMenu;

    public Button boutonNeutral;
    public Button boutonEnclos;
    public Button boutonFarm;

    public void ButtonPressNeutral()
    {
        if(myRoom.GetComponent<RoomInfo>().roomType == 1)
        {
            roomAmountDataHolder.GetComponent<RoomCrafting>().SetFarm(roomAmountDataHolder.GetComponent<RoomCrafting>().GetFarms()+1);
            myRoom.GetComponent<RoomInfo>().ChangementSalle(0);
        }
        else if (myRoom.GetComponent<RoomInfo>().roomType == 2)
        {
            roomAmountDataHolder.GetComponent<RoomCrafting>().SetEnclos(roomAmountDataHolder.GetComponent<RoomCrafting>().GetEnclos() + 1);
            myRoom.GetComponent<RoomInfo>().ChangementSalle(0);
        }
        

        boutonNeutral.interactable = false;
        boutonEnclos.interactable = true;
        boutonFarm.interactable = true;
    }

    public void ButtonPressFarm()
    {
        if(roomAmountDataHolder.GetComponent<RoomCrafting>().GetFarms() >= 1)
        {
            roomAmountDataHolder.GetComponent<RoomCrafting>().SetFarm(roomAmountDataHolder.GetComponent<RoomCrafting>().GetFarms() - 1);
            myRoom.GetComponent<RoomInfo>().ChangementSalle(1);
        }
        if (myRoom.GetComponent<RoomInfo>().roomType == 2)
        {
            roomAmountDataHolder.GetComponent<RoomCrafting>().SetEnclos(roomAmountDataHolder.GetComponent<RoomCrafting>().GetEnclos() + 1);
            myRoom.GetComponent<RoomInfo>().ChangementSalle(1);
        }

        

        boutonNeutral.interactable = true;
        boutonEnclos.interactable = true;
        boutonFarm.interactable = false;
    }

    public void ButtonPressEnclos()
    {
        if (roomAmountDataHolder.GetComponent<RoomCrafting>().GetEnclos() >= 1)
        {
            roomAmountDataHolder.GetComponent<RoomCrafting>().SetEnclos(roomAmountDataHolder.GetComponent<RoomCrafting>().GetEnclos() - 1);
            myRoom.GetComponent<RoomInfo>().ChangementSalle(2);
        }
        if (myRoom.GetComponent<RoomInfo>().roomType == 1)
        {
            roomAmountDataHolder.GetComponent<RoomCrafting>().SetFarm(roomAmountDataHolder.GetComponent<RoomCrafting>().GetFarms() + 1);
            myRoom.GetComponent<RoomInfo>().ChangementSalle(2);
        }

        

        boutonNeutral.interactable = true;
        boutonEnclos.interactable = false;
        boutonFarm.interactable = true;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        myRoom.GetComponent<RoomCrafters>().SetItemToShowInSlot();
        if (collision.gameObject.tag == "Player")
        {
            roomMenu.SetActive(true);
            if (myRoom.GetComponent<RoomInfo>().roomType == 0)
            {
                boutonNeutral.interactable = false;
                boutonEnclos.interactable = true;
                boutonFarm.interactable = true;

            }else if(myRoom.GetComponent<RoomInfo>().roomType == 1)
            {
                boutonNeutral.interactable = true;
                boutonEnclos.interactable = true;
                boutonFarm.interactable = false;

            }
            else if(myRoom.GetComponent<RoomInfo>().roomType == 2)
            {
                boutonNeutral.interactable = true;
                boutonEnclos.interactable = false;
                boutonFarm.interactable = true;

            }
        }
        boutonEnclos.onClick.AddListener(this.gameObject.GetComponent<RoomManager>().ButtonPressEnclos);
        boutonNeutral.onClick.AddListener(this.gameObject.GetComponent<RoomManager>().ButtonPressNeutral);
        boutonFarm.onClick.AddListener(this.gameObject.GetComponent<RoomManager>().ButtonPressFarm);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            roomMenu.SetActive(false);
        }
        boutonNeutral.onClick.RemoveAllListeners();
        boutonEnclos.onClick.RemoveAllListeners();
        boutonFarm.onClick.RemoveAllListeners();

        myRoom.GetComponent<RoomCrafters>().SetItemToDuplicate();
    }
}
