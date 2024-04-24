using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

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
        if (GetComponent<PickableObject>().isPickedUp)
        {
            if (player.CanMove())
            {
                if (Input.GetMouseButtonDown(0))
                {
                    timer = 0;
                    AudioManager.instance.PlaySound(AudioManager.instance.aim, gameObject);
                }

                if (Input.GetMouseButton(0))
                {
                    if (player.objectInRightHand != null)
                    {
                        if (player.objectInRightHand.name == gameObject.name && (gameObject.tag == "Throwable" || gameObject.tag == "Spear" || gameObject.tag == "Bait"))
                        {
                            GetComponent<LineRenderer>().enabled = true;
                            GetComponent<TrajectoryLine>().CalculateTrajectory();
                            timer += Time.deltaTime;
                            force = Mathf.Lerp(minThrowForce, maxThrowForce, timer / timeToMaxThrow);
                        }
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (player.objectInRightHand != null)
                    {
                        if (player.objectInRightHand.name == gameObject.name && (gameObject.tag == "Throwable" || gameObject.tag == "Spear" || gameObject.tag == "Bait"))
                        {
                            GetComponent<LineRenderer>().enabled = false;
                            StartCoroutine(Throw(player.objectInRightHand));
                        }
                    }
                    timer = 0;
                    AudioManager.instance.PlaySound(AudioManager.instance.aimStop, gameObject);
                }
            }
        }
    }

    IEnumerator Throw(GameObject objectToThrow)
    {
        //Si le stack est egal ou plus petit que 1
        if (objectToThrow.tag == "Spear" || objectToThrow.GetComponent<InventoryItem>().stackAmount <= 1)
        {
            //L'objet est lance
            if (objectToThrow.GetComponent<WeaponDamage>() != null)
                objectToThrow.GetComponent<WeaponDamage>().isThrown = true;

            //On prend la direction de la souris et on tourne l'objet dans cette direction
            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - objectToThrow.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            objectToThrow.transform.rotation = Quaternion.Euler(0, 0, angle);

            //On ajoute la force pour lancer l'objet et on reactive la gravite pour l'objet
            objectToThrow.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
            objectToThrow.GetComponent<Rigidbody2D>().gravityScale = 1;

            //L'objet n'est plus picked up et n'est plus selectionner pour etre picked up
            objectToThrow.GetComponent<PickableObject>().isPickedUp = false;

            //On desequipe l'objet en main du joueur
            if (objectToThrow == player.objectInRightHand)
                player.UnequipObject();

            //Si l'objet n'est pas une arme, on enleve un stack
            if (objectToThrow.tag != "Spear")
                objectToThrow.GetComponent<InventoryItem>().stackAmount--;

            //On detruit la version "inventaire" de l'objet
            var toDestroy = objectToThrow.GetComponent<PickableObject>().inventory.GetItem(objectToThrow.GetComponent<InventoryItem>().onGridPositionX,
                objectToThrow.GetComponent<InventoryItem>().onGridPositionY);
            toDestroy.Delete();

            //On attend 0.5 secondes pour reactiver le collider de l'objet
            yield return new WaitForSecondsRealtime(0.5f);
            Physics2D.IgnoreCollision(objectToThrow.GetComponent<Collider2D>(), player.gameObject.GetComponent<Collider2D>(), false);

            //Si l'objet a un effet, il le fait
            switch (effectName)
            {
                case "Flash":
                    yield return new WaitForSeconds(effect.effectCountdown);
                    effect.Flash(objectToThrow.transform, effect.effectRange);
                    objectToThrow.GetComponentInChildren<Light2D>().enabled = true;
                    objectToThrow.GetComponent<Flashbang>().enabled = true;
                    yield return new WaitForSeconds(1.5f);
                    Destroy(objectToThrow);
                    break;
            }
        }
        else //Si le stack est a plus d'un
        {
            //On clone l'objet a lancer
            var objectCloned = Instantiate(consummalbePrefab, transform.position, transform.rotation);
            objectCloned.GetComponent<PickableObject>().isPickedUp = false;

            //On declare l'objet comme etant lancer
            if (objectToThrow.GetComponent<WeaponDamage>() != null)
                objectToThrow.GetComponent<WeaponDamage>().isThrown = true;

            //On ignore les collisions avec le joueur et ce qu'il tient en main
            Physics2D.IgnoreCollision(objectCloned.GetComponent<Collider2D>(), player.gameObject.GetComponent<Collider2D>(), true);
            Physics2D.IgnoreCollision(objectCloned.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), true);

            //On desactive la gravite au debut du lancer
            objectCloned.GetComponent<Rigidbody2D>().gravityScale = 0;

            //On prend la direction de la souris et on tourne l'objet dans cette direction
            Vector2 direction = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - objectCloned.transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            objectCloned.transform.rotation = Quaternion.Euler(0, 0, angle);

            //On ajoute la force pour lancer l'objet et on reactive la gravite pour l'objet
            objectCloned.GetComponent<Rigidbody2D>().AddForce(direction * force, ForceMode2D.Impulse);
            objectCloned.GetComponent<Rigidbody2D>().gravityScale = 1;

            //On reduit le stack
            objectToThrow.GetComponent<InventoryItem>().stackAmount--;
            objectToThrow.GetComponent<PickableObject>().itemInInventory.GetComponent<InventoryItem>().stackAmount--;

            //On change l'image pour representer le changement du stack
            objectToThrow.GetComponent<PickableObject>().inventory.GetItem(objectToThrow.GetComponent<InventoryItem>().onGridPositionX,
                objectToThrow.GetComponent<InventoryItem>().onGridPositionY).GetComponent<Image>().sprite =
                objectToThrow.GetComponent<InventoryItem>().sprites[(objectToThrow.GetComponent<InventoryItem>().sprites.Length) - objectToThrow.GetComponent<InventoryItem>().stackAmount];

            //On attend une demi-seconde et on reactive les collisions
            yield return new WaitForSecondsRealtime(0.5f);
            Physics2D.IgnoreCollision(objectCloned.GetComponent<Collider2D>(), player.gameObject.GetComponent<Collider2D>(), false);
            Physics2D.IgnoreCollision(objectCloned.GetComponent<Collider2D>(), gameObject.GetComponent<Collider2D>(), false);

            //Si l'objet a un effet
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
        }
    }

    private void OnDestroy()
    {
        AudioManager.instance.PlaySound(AudioManager.instance.aimStop, gameObject);
    }
}
