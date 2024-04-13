using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantConsummable : MonoBehaviour
{
    [SerializeField] private GameObject consummable;
    [SerializeField] private GameObject ConsummableGFX;
    bool canPickUpConsummable;
    public bool hasPickedUpConsummable;
    public string foodName;

    private Material ogMaterial;
    [SerializeField] private Material flashMaterial;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canPickUpConsummable = true;
            if (!hasPickedUpConsummable)
                ArrowManager.instance.PlaceArrow(transform.position, "PICK UP", new Vector2(0, 0), gameObject, 1);
            StartCoroutine(FlashWhite(ConsummableGFX.GetComponent<SpriteRenderer>(), 0.05f, 5));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKey(KeyCode.E)) && canPickUpConsummable && ArrowManager.instance.targetObject == gameObject)
            {
                if (!hasPickedUpConsummable)
                {
                    hasPickedUpConsummable = true;
                    canPickUpConsummable = false;
                    var cons = Instantiate(consummable, transform.position, transform.rotation);
                    cons.GetComponent<PickableObject>().PickUp(false, false);
                    ConsummableGFX.SetActive(false);
                    ArrowManager.instance.RemoveArrow();
                }
            }
        }
    }

    private void OnDisable()
    {
        if (ArrowManager.instance.targetObject == gameObject)
            ArrowManager.instance.RemoveArrow();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canPickUpConsummable = false;
            if (ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();
        }
    }

    private IEnumerator FlashWhite(SpriteRenderer sprite, float duration, int flashCount)
    {
        for (int i = 0; i < flashCount; i++)
        {
            yield return new WaitForSecondsRealtime(duration);
            ogMaterial = sprite.material;
            //sprite.color = new Color(255, 255, 255, 255);
            sprite.material = flashMaterial;
            yield return new WaitForSecondsRealtime(duration);
            sprite.material = ogMaterial;
            //sprite.color = ogColor;
            if (!canPickUpConsummable)
            {
                //sprite.color = ogColor;
                sprite.material = ogMaterial;
                yield break;
            }
            if (hasPickedUpConsummable)
            {
                //sprite.color = ogColor;
                sprite.material = ogMaterial;
                yield break;
            }
        }
    }
}
