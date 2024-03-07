using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowableObject : MonoBehaviour
{
    public float force;
    [SerializeField] private GameObject consummalbePrefab;
    public float timeToMaxThrow;
    public float timer;
    private PlayerPermanent player;
    [SerializeField] float minThrowForce;
    [SerializeField] float maxThrowForce;
    public bool isThrown;

    [SerializeField] ConsummableEffects effect;
    [SerializeField] string effectName;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerPermanent>();
        force = minThrowForce;
    }

    // Update is called once per frame
    void Update()
    {
        if (!player.inventoryOpen)
        {
            if (GetComponent<PickableObject>().isPickedUp)
            {
                if (Input.GetMouseButtonDown(0))
                    timer = 0;

                if (Input.GetMouseButton(0))
                {
                    if (player.objectInRightHand != null)
                    {
                        if (player.objectInRightHand.name == gameObject.name && (gameObject.tag == "Throwable" || gameObject.tag == "Spear"))
                        {
                            GetComponent<LineRenderer>().enabled = true;
                            GetComponent<TrajectoryLine>().CalculateTrajectory();
                            timer += Time.deltaTime;
                            force = Mathf.Lerp(minThrowForce, maxThrowForce, timer / timeToMaxThrow);
                        }
                    }
                    /* Utilisation de la main gauche
                    else if (player.objectInLeftHand != null)
                    {
                        if (player.objectInLeftHand == gameObject && (gameObject.tag == "Throwable" || gameObject.tag == "Javelin"))
                        {
                            force = Mathf.Lerp(10, 100, timer / timeToMaxThrow);
                        }
                    }
                    */
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (player.objectInRightHand != null)
                    {
                        if (player.objectInRightHand.name == gameObject.name && (gameObject.tag == "Throwable" || gameObject.tag == "Spear"))
                        {
                            GetComponent<LineRenderer>().enabled = false;
                            StartCoroutine(Throw(player.objectInRightHand));
                        }
                    }
                    timer = 0;
                    /* Utilisation de la main gauche
                    else if (player.objectInLeftHand != null)
                    {
                        if (player.objectInLeftHand == gameObject && (gameObject.tag == "Throwable" || gameObject.tag == "Javelin"))
                            StartCoroutine(Throw(player.objectInLeftHand));
                    }
                    */
                }
            }
        }
    }

    IEnumerator Throw(GameObject objectToThrow)
    {
        if (objectToThrow.tag == "Spear" || objectToThrow.GetComponent<InventoryItem>().stackAmount == 1)
        {
            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - objectToThrow.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            objectToThrow.transform.rotation = Quaternion.Euler(0, 0, angle);

            objectToThrow.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
            objectToThrow.GetComponent<Rigidbody2D>().gravityScale = 1;

            objectToThrow.GetComponent<PickableObject>().isPickedUp = false;
            objectToThrow.GetComponent<PickableObject>().hasFlashed = false;

            if (objectToThrow == player.objectInRightHand)
                player.UnequipObject();

            if (objectToThrow.tag != "Spear")
                objectToThrow.GetComponent<InventoryItem>().stackAmount--;

            objectToThrow.GetComponent<ThrowableObject>().isThrown = true;

            var toDestroy = objectToThrow.GetComponent<PickableObject>().inventory.GetItem(objectToThrow.GetComponent<InventoryItem>().onGridPositionX,
                objectToThrow.GetComponent<InventoryItem>().onGridPositionY);
            toDestroy.Delete();

            yield return new WaitForSecondsRealtime(0.5f);
            Physics2D.IgnoreCollision(objectToThrow.GetComponent<CapsuleCollider2D>(), player.gameObject.GetComponent<Collider2D>(), false);

            switch (effectName)
            {
                case "Flash":
                    yield return new WaitForSeconds(effect.effectCountdown);
                    effect.Flash(objectToThrow.transform, effect.effectRange);
                    Destroy(objectToThrow);
                    break;
            }

            /* Utilisation de la main gauche
            else if (gameObject == player.objectInLeftHand)
                player.UnequipObject(false);
            */
        }
        else
        {
            var objectCloned = Instantiate(consummalbePrefab, transform.position, transform.rotation);
            objectCloned.GetComponent<PickableObject>().isPickedUp = false;

            Physics2D.IgnoreCollision(objectCloned.GetComponent<CapsuleCollider2D>(), player.gameObject.GetComponent<Collider2D>(), true);
            objectCloned.GetComponent<Rigidbody2D>().gravityScale = 0;

            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - objectCloned.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            objectCloned.transform.rotation = Quaternion.Euler(0, 0, angle);

            objectCloned.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);

            objectCloned.GetComponent<Rigidbody2D>().gravityScale = 1;

            objectToThrow.GetComponent<PickableObject>().inventory.GetItem(objectToThrow.GetComponent<InventoryItem>().onGridPositionX,
                objectToThrow.GetComponent<InventoryItem>().onGridPositionY).GetComponent<Image>().sprite =
                objectToThrow.GetComponent<InventoryItem>().sprites[(objectToThrow.GetComponent<InventoryItem>().sprites.Length + 1) - objectToThrow.GetComponent<InventoryItem>().stackAmount];

            objectToThrow.GetComponent<InventoryItem>().stackAmount--;
            objectToThrow.GetComponent<PickableObject>().itemInInventory.GetComponent<InventoryItem>().stackAmount--;

            objectCloned.GetComponent<ThrowableObject>().isThrown = true;

            yield return new WaitForSecondsRealtime(0.5f);
            Physics2D.IgnoreCollision(objectCloned.GetComponent<CapsuleCollider2D>(), player.gameObject.GetComponent<Collider2D>(), false);

            switch (effectName)
            {
                case "Flash":
                    yield return new WaitForSeconds(effect.effectCountdown);
                    effect.Flash(objectCloned.transform, effect.effectRange);
                    Destroy(objectCloned);
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.GetMask("Ground"))
        {
            isThrown = false;
        }
    }
}
