using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Beacon : MonoBehaviour
{
    public bool isActive = false;
    [SerializeField] SpriteRenderer mapIcon;
    [SerializeField] GameObject beaconToSpawn;

    public void ActiveBeacon()
    {
        //Spawn the beacon
        var beacon = Instantiate(beaconToSpawn, GameManager.instance.player.gameObject.transform.position, Quaternion.identity);

        //Reset beacon
        beacon.GetComponent<PickableObject>().isPickedUp = false;
        beacon.GetComponent<PickableObject>().tooltipActive = false;
        beacon.GetComponent<PickableObject>().isSelected = false;

        //Activate physics on beacon
        beacon.GetComponent<Rigidbody2D>().freezeRotation = true;
        beacon.GetComponent<Rigidbody2D>().gravityScale = 1;

        //Activate map icon
        beacon.GetComponent<Beacon>().mapIcon.enabled = true;

        //Activate and add beacon to list in map manager
        MapManager.instance.beacons.Add(beacon);
        MapManager.instance.activeBeaconsCount++;
        beacon.GetComponent<Beacon>().isActive = true;

        //Prompt to name the beacon
        PromptManager.instance.CreateNewPrompt(new Prompt("Name your beacon!", true, "", "", "Continue", true));
        PromptManager.onButtonClick = ActivateButton;
    }

    void ActivateButton()
    {
        MapManager.instance.buttons[MapManager.instance.activeBeaconsCount].SetActive(true);
        MapManager.instance.buttons[MapManager.instance.activeBeaconsCount].GetComponentInChildren<TextMeshProUGUI>().text = PromptManager.instance.inputFieldText;
    }

    public void DeactivateButton()
    {
        if (MapManager.instance.beacons.Contains(gameObject))
        {
            int index = MapManager.instance.beacons.IndexOf(gameObject);
            MapManager.instance.buttons[index].GetComponentInChildren<TextMeshProUGUI>().text = "";
            MapManager.instance.buttons[index].SetActive(false);
            MapManager.instance.beacons.Remove(gameObject);
        }
    }
}
