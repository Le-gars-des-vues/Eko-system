using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubblePlant : MonoBehaviour
{
    bool isInside;
    CameraController cam;
    [SerializeField] SpriteRenderer sprite;
    [SerializeField] GameObject maskWater;
    [SerializeField] GameObject maskIsolate;

    private float desiredAlpha = 0;
    private float currentAlpha = 0;
    [SerializeField] float fadeSpeed;

    private void Start()
    {
        cam = Camera.main.GetComponent<CameraController>();
    }

    private void Update()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, fadeSpeed * Time.deltaTime);
        sprite.color = sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, currentAlpha);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PlayerPermanent>().isInAirPocket)
            {
                collision.gameObject.GetComponent<PlayerPermanent>().isInAirPocket = true;
                AudioManager.instance.PlaySound(AudioManager.instance.bubblePlant, gameObject);
            }
            if (!cam.isIsoldated)
                cam.IsolateCameraView(true);
            desiredAlpha = 1;
            maskWater.SetActive(true);
            maskIsolate.SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isInside = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.GetComponent<PlayerPermanent>().isInAirPocket)
            {
                collision.gameObject.GetComponent<PlayerPermanent>().isInAirPocket = false;
                AudioManager.instance.PlaySound(AudioManager.instance.bubblePlantStop, gameObject);
            }
            isInside = false;
            if (cam.isIsoldated)
                cam.IsolateCameraView(false);
            desiredAlpha = 0;
            maskWater.SetActive(false);
            maskIsolate.SetActive(false);
        }
    }
}
