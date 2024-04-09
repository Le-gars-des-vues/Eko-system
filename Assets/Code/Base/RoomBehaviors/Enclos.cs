using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enclos : MonoBehaviour
{
    bool isInRange = false;
    bool hasACreature = false;
    [SerializeField] GameObject growLight;
    [SerializeField] Transform spawnPoint;

    PlayerPermanent player;

    [SerializeField] List<GameObject> creatures = new List<GameObject>();
    [SerializeField] GameObject creatureToClone;
    [SerializeField] GameObject theCreature;
    [SerializeField] int timeToGrow = 1;
    public int growthIndex = 0;

    [SerializeField] int maxRessource = 3;

    [SerializeField] GameObject room;
    [SerializeField] Farming farming;


    private void OnEnable()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        farming = GameObject.Find("Farming").GetComponent<Farming>();
        GameManager.instance.enclosure.Add(this);
        room = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            if (isInRange && ArrowManager.instance.readyToActivate)
            {
                if (!player.inventoryOpen)
                {
                    player.ShowOrHideInventoryNoButtons();
                }
                if (!player.farmingIsOpen)
                {
                    player.ShowOrHideFarming(false);
                }
            }
        }

        if (room.GetComponent<RoomInfo>().isRefunded)
        {
            GameManager.instance.enclosure.Remove(this);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        isInRange = true;
        ArrowManager.instance.PlaceArrow(transform.position, "Clone", new Vector2(0, 1), gameObject, 1);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isInRange = false;
        if (ArrowManager.instance.targetObject == gameObject)
            ArrowManager.instance.RemoveArrow();
    }
}
