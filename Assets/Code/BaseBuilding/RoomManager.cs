using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public GameObject myRoom;
    public GameObject roomAmountDataHolder;

    public GameObject roomMenu;

    public bool readyToWork;
    // Start is called before the first frame update
    void Start()
    {
        readyToWork = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(readyToWork && Input.GetKeyDown(KeyCode.W))
        {
            roomMenu.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            readyToWork = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            readyToWork = false;
        }
    }
}
