using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    [SerializeField] Vector2 parallaxEffectMultiplier;
    [SerializeField] Transform cameraTransform;
    private Vector3 lastCameraPosition;
    public float textureUnitSizeX;
    public float textureUnitSizeY;

    [SerializeField] private bool infinityHorizontal;
    [SerializeField] private bool infinityVertical;

    private void Start()
    {
        cameraTransform = Camera.main.transform;
        lastCameraPosition = cameraTransform.position;
        Sprite sprite = GetComponent<SpriteRenderer>().sprite;
        Texture2D texture = sprite.texture;
        textureUnitSizeX = texture.width / sprite.pixelsPerUnit;
        textureUnitSizeY = texture.height / sprite.pixelsPerUnit;
    }

    private void LateUpdate()
    {
        Vector3 deltaMovement = cameraTransform.position - lastCameraPosition;
        transform.position += new Vector3(deltaMovement.x * parallaxEffectMultiplier.x, deltaMovement.y * parallaxEffectMultiplier.y);
        lastCameraPosition = cameraTransform.position;

        //Debug.Log(cameraTransform.position.x - transform.position.x);
        if (infinityHorizontal)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionX = (cameraTransform.position.x - transform.position.x) % textureUnitSizeX;
                transform.position = new Vector3(cameraTransform.position.x + offsetPositionX, transform.position.y);
            }
        }
        if (infinityVertical)
        {
            if (Mathf.Abs(cameraTransform.position.x - transform.position.x) >= textureUnitSizeX)
            {
                float offsetPositionY = (cameraTransform.position.y - transform.position.y) % textureUnitSizeY;
                transform.position = new Vector3(cameraTransform.position.x, transform.position.y + offsetPositionY);
            }
        }
    }
}
