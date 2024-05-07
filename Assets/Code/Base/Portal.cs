using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    bool isInRange;
    public bool isBaseTeleporter;

    // Update is called once per frame
    void Update()
    {
        if (!isBaseTeleporter)
        {
            if (Input.GetKey(KeyCode.E))
            {
                if (isInRange && ArrowManager.instance.targetObject == gameObject)
                {
                    foreach (Teleporter teleporter in GameManager.instance.teleporter)
                    {
                        if (teleporter.isPoweredUp)
                        {
                            Base.instance.Teleport(false, true, teleporter.gameObject.transform.position);
                            teleporter.isPoweredUp = false;
                            QuickMenu.instance.CheckForOpenTeleporter();
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
            if (!isBaseTeleporter && ArrowManager.instance.targetObject != gameObject)
                ArrowManager.instance.PlaceArrow(transform.position, "TELEPORT", new Vector2(0, 1), gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (!isBaseTeleporter && ArrowManager.instance.targetObject != gameObject)
                ArrowManager.instance.PlaceArrow(transform.position, "TELEPORT", new Vector2(0, 1), gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            if (!isBaseTeleporter && ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();
        }
    }
}
