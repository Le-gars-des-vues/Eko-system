using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UsableObject : MonoBehaviour
{
    private PlayerPermanent player;
    [SerializeField] string effectName;

    [SerializeField] ConsummableEffects effect;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.inventoryOpen)
        {
            if (GetComponent<PickableObject>().isPickedUp)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (player.objectInRightHand != null)
                    {
                        if (player.objectInRightHand.name == gameObject.name && (gameObject.tag == "Usable"))
                        {
                            StartCoroutine(UseObject(player.objectInRightHand));
                        }
                    }
                }
            }
        }
    }

    IEnumerator UseObject(GameObject objectToUse)
    {
        switch (effectName)
        {
            case "Heal":
                effect.Heal(player, effect.effectMagnitude);
                break;

            default:
                break;
        }

        if (objectToUse.GetComponent<InventoryItem>().sprites.Length >= 1)
        {
            objectToUse.GetComponent<PickableObject>().inventory.GetItem(objectToUse.GetComponent<InventoryItem>().onGridPositionX,
                objectToUse.GetComponent<InventoryItem>().onGridPositionY).GetComponent<Image>().sprite =
                objectToUse.GetComponent<InventoryItem>().sprites[(objectToUse.GetComponent<InventoryItem>().sprites.Length) - objectToUse.GetComponent<InventoryItem>().stackAmount];
        }

        objectToUse.GetComponent<PickableObject>().itemInInventory.GetComponent<InventoryItem>().stackAmount--;
        objectToUse.GetComponent<InventoryItem>().stackAmount--;

        if (objectToUse.GetComponent<InventoryItem>().stackAmount <= 0)
        {
            Destroy(objectToUse);
            player.UnequipObject();

            var toDestroy = objectToUse.GetComponent<PickableObject>().inventory.GetItem(objectToUse.GetComponent<InventoryItem>().onGridPositionX, 
                objectToUse.GetComponent<InventoryItem>().onGridPositionY);
            toDestroy.Delete();
        }

        yield return new WaitForSecondsRealtime(0.5f);
    }
}
