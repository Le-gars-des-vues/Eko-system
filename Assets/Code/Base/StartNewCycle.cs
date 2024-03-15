using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartNewCycle : MonoBehaviour
{
    bool isInRange;

    // Update is called once per frame
    void Update()
    {
        if (isInRange)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartCoroutine(GameObject.Find("GameManager").GetComponent<GameManager>().NewCycle());
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            isInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            isInRange = false;
    }
}
