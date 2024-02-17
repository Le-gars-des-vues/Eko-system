using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearPlant : MonoBehaviour
{
    [SerializeField] private GameObject consummable;
    [SerializeField] private GameObject plantConsummableGFX;
    bool canPickUpConsummable;
    public bool hasPickedUpConsummable;

    private Material ogMaterial;
    [SerializeField] private Material flashMaterial;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canPickUpConsummable = true;
            StartCoroutine(FlashWhite(plantConsummableGFX.GetComponent<SpriteRenderer>(), 0.05f, 5));
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKey(KeyCode.E)) && canPickUpConsummable)
            {
                if (!hasPickedUpConsummable)
                {
                    canPickUpConsummable = false;
                    var spear = Instantiate(consummable, transform.position, transform.rotation);
                    spear.GetComponent<PickableObject>().PickUp();
                    hasPickedUpConsummable = true;
                    plantConsummableGFX.SetActive(false);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            canPickUpConsummable = false;
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
