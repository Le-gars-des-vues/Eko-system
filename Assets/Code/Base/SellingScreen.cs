using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SellingScreen : MonoBehaviour
{
    PlayerPermanent player;
    bool isInRange;
    [SerializeField] GameObject arrow;
    [SerializeField] GameObject tvText;
    [SerializeField] GameObject tvLight;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        tvText.SetActive(false);
        tvLight.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.marketIsOpen)
        {
            if (Input.GetKey(KeyCode.E) && arrow.GetComponent<Arrow>().readyToActivate)
            {
                if (isInRange)
                {
                    if (!player.inventoryOpen)
                    {
                        player.ShowOrHideInventoryNoButtons();
                    }
                    if (!player.marketIsOpen)
                    {
                        player.ShowOrHideMarket();
                    }
                    if (!tvText.activeSelf)
                        tvText.SetActive(true);
                    if (!tvLight.activeSelf)
                        tvLight.SetActive(true);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (isInRange)
                {
                    if (player.inventoryOpen && player.marketIsOpen)
                    {
                        player.ShowOrHideInventoryNoButtons();
                        player.ShowOrHideMarket();
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            arrow.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            arrow.SetActive(false);
            if (player.marketIsOpen)
            {
                player.ShowOrHideMarket();
            }
        }
    }
}
