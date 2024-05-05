using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingBench : MonoBehaviour
{
    PlayerPermanent player;
    bool isInRange;
    CraftingManager crafting;

    [SerializeField] List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    [SerializeField] List<Material> materials = new List<Material>();
    [SerializeField] Transform trainingRoomPos;
    bool isTeleporting;
    Vector2 ogPos;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        crafting = GameObject.Find("Crafting").transform.Find("DropdownListOfCrafts").GetComponent<CraftingManager>();
        ogPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && !DialogueManager.instance.dialogueRunning)
        {
            if (isInRange)
            {
                if (!player.craftingIsOpen)
                {
                    if (ArrowManager.instance.targetObject == gameObject)
                    {
                        crafting.OnValueChanged();
                        if (!player.craftingIsOpen)
                            player.ShowOrHideCrafting();
                        if (!player.inventoryOpen)
                            player.ShowOrHideInventory(false);
                    }
                }
                else
                {
                    if (player.craftingIsOpen)
                        player.ShowOrHideCrafting();
                    if (player.inventoryOpen)
                        player.ShowOrHideInventory(false);
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
            AudioManager.instance.PlaySound(AudioManager.instance.teleport, gameObject);
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
            AudioManager.instance.PlaySound(AudioManager.instance.teleport, gameObject);
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
                ArrowManager.instance.PlaceArrow(transform.position, "CRAFTING BENCH", new Vector2(0, 1), gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (!ArrowManager.instance.isActive && !isTeleporting)
                ArrowManager.instance.PlaceArrow(transform.position, "CRAFTING BENCH", new Vector2(0, 1), gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            if (ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();
            if (player.craftingIsOpen)
            {
                player.ShowOrHideCrafting();
            }
        }
    }
}
