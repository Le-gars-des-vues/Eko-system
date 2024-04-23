using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SellingScreen : MonoBehaviour
{
    PlayerPermanent player;
    bool isInRange;
    [SerializeField] GameObject tvText;
    [SerializeField] GameObject tvLight;

    [SerializeField] List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    [SerializeField] List<Material> materials = new List<Material>();
    [SerializeField] Transform trainingRoomPos;
    bool isTeleporting;
    Vector2 ogPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        tvText.SetActive(false);
        tvLight.SetActive(false);
        ogPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isInRange)
            {
                if (!player.marketIsOpen)
                {
                    if (ArrowManager.instance.targetObject == gameObject)
                    {
                        if (!player.inventoryOpen)
                            player.ShowOrHideInventory(false);
                        if (!player.marketIsOpen)
                            player.ShowOrHideMarket();
                        if (!tvText.activeSelf)
                            tvText.SetActive(true);
                        if (!tvLight.activeSelf)
                            tvLight.SetActive(true);
                    }
                }
                else
                {
                    if (player.inventoryOpen)
                        player.ShowOrHideInventory(false);
                    if (player.marketIsOpen)
                        player.ShowOrHideMarket();
                }
            }
        }
    }

    public void TrainingRoom(bool isTrue)
    {
        if (isTrue)
        {
            StartCoroutine(Dissolve(2f, true, trainingRoomPos.position, materials[2], materials[2], LayerMask.NameToLayer("Default")));
            foreach (var sprite in sprites)
            {
                sprite.sortingLayerID = SortingLayer.NameToID("Default");
                sprite.sortingOrder = 0;
            }
        }
        else
        {
            StartCoroutine(Dissolve(2f, true, ogPos, materials[2], materials[0], LayerMask.NameToLayer("Pixelate")));
            foreach (var sprite in sprites)
            {
                sprite.sortingLayerID = SortingLayer.NameToID("Pixelate");
                sprite.sortingOrder = 2;
            }
        }
    }

    IEnumerator Dissolve(float dissolveTime, bool teleport, Vector2 target, Material startMaterial, Material endMaterial, LayerMask endLayer, bool isTrue = true)
    {
        isTeleporting = true;
        float elapsedTime = 0;
        if (teleport)
        {
            for (int i = 0; i < sprites.Count; i++)
                sprites[i].material = startMaterial;

            while (elapsedTime < dissolveTime)
            {
                elapsedTime += Time.deltaTime;
                float dissolveAmount = Mathf.Lerp(0.01f, 1f, elapsedTime / dissolveTime);
                foreach (var sprite in sprites)
                    sprite.material.SetFloat("_Transparency", dissolveAmount);

                yield return null;
            }

            transform.position = target;

            foreach (SpriteRenderer sprite in sprites)
                sprite.gameObject.layer = endLayer;

            elapsedTime = 0;
            while (elapsedTime < dissolveTime)
            {
                elapsedTime += Time.deltaTime;
                float dissolveAmount = Mathf.Lerp(1, 0.01f, elapsedTime / dissolveTime);
                foreach (var sprite in sprites)
                    sprite.material.SetFloat("_Transparency", dissolveAmount);

                yield return null;
            }

            for (int i = 0; i < sprites.Count; i++)
                sprites[i].material = endMaterial;
        }
        else
        {
            if (isTrue)
            {
                for (int i = 0; i < sprites.Count; i++)
                    sprites[i].material = startMaterial;

                while (elapsedTime < dissolveTime)
                {
                    elapsedTime += Time.deltaTime;
                    float dissolveAmount = Mathf.Lerp(0.01f, 1f, elapsedTime / dissolveTime);
                    foreach (var sprite in sprites)
                        sprite.material.SetFloat("_Transparency", dissolveAmount);

                    yield return null;
                }
            }
            else
            {
                foreach (SpriteRenderer sprite in sprites)
                    sprite.gameObject.layer = endLayer;

                while (elapsedTime < dissolveTime)
                {
                    elapsedTime += Time.deltaTime;
                    float dissolveAmount = Mathf.Lerp(1f, 0.01f, elapsedTime / dissolveTime);
                    foreach (var sprite in sprites)
                        sprite.material.SetFloat("_Transparency", dissolveAmount);

                    yield return null;
                }

                for (int i = 0; i < sprites.Count; i++)
                    sprites[i].material = endMaterial;
            }
        }
        isTeleporting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (!ArrowManager.instance.isActive && !isTeleporting)
                ArrowManager.instance.PlaceArrow(transform.position, "SELL", new Vector2(0, 1), gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (!ArrowManager.instance.isActive && !isTeleporting)
                ArrowManager.instance.PlaceArrow(transform.position, "SELL", new Vector2(0, 1), gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            if (ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();
            if (player.marketIsOpen)
            {
                player.ShowOrHideMarket();
            }
        }
    }
}
