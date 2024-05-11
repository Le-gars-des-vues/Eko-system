using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateWhenPlayerIsNear : MonoBehaviour
{
    [SerializeField] List<GameObject> objectsToActivate = new List<GameObject>();

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (GameObject obj in objectsToActivate)
            {
                obj.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            foreach (GameObject obj in objectsToActivate)
            {
                obj.SetActive(false);
            }
        }
    }
}
