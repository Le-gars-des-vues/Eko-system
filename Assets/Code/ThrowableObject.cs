using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowableObject : MonoBehaviour
{
    [SerializeField] private float force;
    [SerializeField] private float timeToMaxThrow;
    private float timer;
    private PickableObject item;
    private PlayerPermanent player;

    private void Start()
    {
        item = GetComponent<PickableObject>();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.inventoryOpen)
        {
            timer += Time.deltaTime;
            if (GetComponent<PickableObject>().isPickedUp)
            {
                if (Input.GetMouseButtonDown(0))
                    timer = 0;

                if (Input.GetMouseButton(0))
                {
                    if (player.objectInRightHand != null)
                    {
                        if (player.objectInRightHand.name == gameObject.name && (gameObject.tag == "Throwable" || gameObject.tag == "Javelin"))
                        {
                            force = Mathf.Lerp(10, 100, timer / timeToMaxThrow);
                        }
                    }
                    else if (player.objectInLeftHand != null)
                    {
                        if (player.objectInLeftHand == gameObject && (gameObject.tag == "Throwable" || gameObject.tag == "Javelin"))
                        {
                            force = Mathf.Lerp(10, 100, timer / timeToMaxThrow);
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (player.objectInRightHand != null)
                    {
                        if (player.objectInRightHand.name == gameObject.name && (gameObject.tag == "Throwable" || gameObject.tag == "Javelin"))
                            StartCoroutine(Throw(player.objectInRightHand));
                    }
                    else if (player.objectInLeftHand != null)
                    {
                        if (player.objectInLeftHand == gameObject && (gameObject.tag == "Throwable" || gameObject.tag == "Javelin"))
                            StartCoroutine(Throw(player.objectInLeftHand));
                    }
                }
            }
        }
    }

    IEnumerator Throw(GameObject objectToThrow)
    {
        objectToThrow.transform.parent = null;
        objectToThrow.GetComponent<Rigidbody2D>().simulated = true;

        Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - objectToThrow.transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        objectToThrow.transform.rotation = Quaternion.Euler(0, 0, angle);

        objectToThrow.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
        yield return new WaitForSecondsRealtime(0.1f);
        objectToThrow.GetComponent<BoxCollider2D>().enabled = true;
        objectToThrow.GetComponent<PickableObject>().isPickedUp = false;
        objectToThrow.GetComponent<PickableObject>().hasFlashed = false;

        if (gameObject == player.objectInRightHand)
            player.UnequipObject(true);
        else if (gameObject == player.objectInLeftHand)
            player.UnequipObject(false);

    }
}
