using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    PlayerPermanent player;
    bool isInRange;
    public bool isBaseTeleporter;
    [SerializeField] GameObject arrow;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isBaseTeleporter)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (isInRange && arrow.GetComponent<Arrow>().readyToActivate)
                {
                    foreach (Teleporter teleporter in GameManager.instance.teleporter)
                    {
                        if (teleporter.isPoweredUp)
                        {
                            Base.instance.Teleport(false, true, teleporter.gameObject.transform.position);
                            teleporter.isPoweredUp = false;
                            CycleInfo.instance.CheckForOpenTeleporter();
                            Destroy(gameObject);
                            return;
                        }
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
            if (!isBaseTeleporter)
                arrow.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            if (!isBaseTeleporter)
                arrow.SetActive(false);
        }
    }
}
