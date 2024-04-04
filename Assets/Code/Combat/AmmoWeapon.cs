using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoWeapon : MonoBehaviour
{
    public float force;
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private string ammoType;
    [SerializeField] private Transform firePoint;

    [SerializeField] bool canCharge;
    public float timeToMaxThrow;
    public float timer;
    private PlayerPermanent player;
    [SerializeField] float minThrowForce;
    [SerializeField] float maxThrowForce;
    public bool isThrown;
    float throwTime;
    float pickableCooldown = 1f;

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
        if (GetComponent<PickableObject>().isPickedUp)
        {
            if (player.CanMove())
            {
                if (Input.GetMouseButtonDown(0))
                    timer = 0;

                if (Input.GetMouseButton(0))
                {
                    if (player.objectInRightHand != null)
                    {
                        if (player.hasOptics)
                        {
                            GetComponent<LineRenderer>().enabled = true;
                            GetComponent<TrajectoryLine>().CalculateTrajectory();
                        }
                        if (canCharge)
                        {
                            timer += Time.deltaTime;
                            force = Mathf.Lerp(minThrowForce, maxThrowForce, timer / timeToMaxThrow);
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (player.objectInRightHand != null)
                    {
                        if (player.hasOptics)
                            GetComponent<LineRenderer>().enabled = false;
                        StartCoroutine(Launch());
                    }
                    timer = 0;
                }
            }
        }
        else
        {
            if (Time.time - throwTime > pickableCooldown && isThrown)
            {
                isThrown = false;
                GetComponent<PickableObject>().hasFlashed = false;
            }
        }
    }

    IEnumerator Launch()
    {
        bool hasAmmo = false;

        float height = GetComponent<PickableObject>().playerInventory.GetGridSizeHeight();
        float width = GetComponent<PickableObject>().playerInventory.GetGridSizeWidth();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                InventoryItem anItem = GetComponent<PickableObject>().playerInventory.CheckIfItemPresent(x, y);
                if (anItem != null)
                {
                    if (anItem.itemData.itemName == ammoType)
                    {
                        hasAmmo = true;
                        anItem.stackAmount--;
                        if (anItem.stackAmount <= 0)
                            anItem.Delete();
                        else
                            anItem.gameObject.GetComponent<Image>().sprite = anItem.sprites[anItem.sprites.Length - anItem.stackAmount];
                    }
                }
            }
        }

        yield return null;

        if (hasAmmo)
        {
            var objectCloned = Instantiate(ammoPrefab, firePoint.position, transform.rotation);
            objectCloned.GetComponent<PickableObject>().isPickedUp = false;

            //On declare l'objet comme etant lancer
            objectCloned.GetComponent<WeaponDamage>().isThrown = true;

            Physics2D.IgnoreCollision(objectCloned.GetComponent<Collider2D>(), player.gameObject.GetComponent<Collider2D>(), true);
            Physics2D.IgnoreCollision(objectCloned.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);
            objectCloned.GetComponent<Rigidbody2D>().gravityScale = 0;

            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - objectCloned.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            objectCloned.transform.rotation = Quaternion.Euler(0, 0, angle);

            objectCloned.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);

            objectCloned.GetComponent<Rigidbody2D>().gravityScale = 1;

            yield return new WaitForSecondsRealtime(0.5f);
            Physics2D.IgnoreCollision(objectCloned.GetComponent<Collider2D>(), player.gameObject.GetComponent<Collider2D>(), false);
            Physics2D.IgnoreCollision(objectCloned.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), false);
        }
        else
            Debug.Log("No Ammo");

        /*
        switch (effectName)
        {
            case "Flash":
                yield return new WaitForSeconds(effect.effectCountdown);
                effect.Flash(objectCloned.transform, effect.effectRange);
                objectCloned.GetComponentInChildren<Light2D>().enabled = true;
                objectCloned.GetComponent<Flashbang>().enabled = true;
                yield return new WaitForSeconds(1.5f);
                Destroy(objectCloned);
                break;
        }
        */
    }
}
