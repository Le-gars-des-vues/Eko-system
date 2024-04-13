using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDamage : MonoBehaviour
{
    Rigidbody2D rb;
    [Header("Damage Variables")]
    [SerializeField] int minDamage;
    [SerializeField] int maxDamage;
    int hitDamage;
    bool isDamaging;

    int durabilityDamage = 1;

    //[SerializeField] float piercingAngleThreshold;
    [Header("State Variables")]
    Vector3 previousPosition;
    public Vector3 velocity;
    [SerializeField] [Range(0, 1)] float angleThreshold;
    public bool isThrown;
    [SerializeField] LayerMask groundLayer;


    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        previousPosition = transform.position;
    }

    private void Update()
    {
        hitDamage = Mathf.RoundToInt((1 * velocity.magnitude) / 2);
        hitDamage = Mathf.Clamp(hitDamage, minDamage, maxDamage);
    }

    private void FixedUpdate()
    {
        velocity = (transform.position - previousPosition) / Time.deltaTime;
        velocity.x = Mathf.Abs(velocity.x);
        velocity.y = Mathf.Abs(velocity.y);

        previousPosition = transform.position;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider != null && collision.collider.gameObject.tag == "Creature" && (GetComponent<PickableObject>().isPickedUp || isThrown) && !GetComponent<InventoryItem>().broken)
        {
            CreatureHealth hp = GetScript(collision.collider.gameObject);
            if (gameObject.tag == "Spear" || gameObject.tag == "Ammo")
            {
                if (!isDamaging && !hp.isInvincible)
                {
                    ContactPoint2D contact = collision.GetContact(0);

                    /*
                    if (angle < piercingAngleThreshold)
                    {

                    }
                    Vector2 normal = contact.normal;
                    float angle = Vector2.Angle(hitDirection, -normal);
                    Debug.Log("Angle : " + angle + " Collider : " + dmg.collider.gameObject.name);
                    */


                    //Check if the angle is within the piercing threshold
                    if (Vector2.Dot(contact.normal, transform.right) <= -0.700f)
                    {
                        isDamaging = true;
                        hp.LoseHealth(hitDamage, GameObject.FindGameObjectWithTag("Player"));
                        int color = 0;
                        if (hitDamage < maxDamage / 3)
                            color = 0;
                        else if (hitDamage > maxDamage / 3 && hitDamage < (maxDamage / 3) * 2)
                            color = 1;
                        else if (hitDamage > (maxDamage / 3) * 2)
                            color = 2;
                        ShowDamage(hitDamage, collision.GetContact(0).point, color);
                        GetComponent<PickableObject>().DurabilityDamage(durabilityDamage);

                        if (isThrown)
                        {
                            isThrown = false;
                            Vector2 hitDirection = contact.point - (Vector2)transform.position;
                            Vector2 stickPos = (Vector2)transform.position + hitDirection.normalized;
                            StartCoroutine(Stick(collision.collider, stickPos));
                        }
                    }
                }
            }
            else if (gameObject.tag == "OneHandedWeapon")
            {
                isDamaging = true;
                hp.LoseHealth(hitDamage, GameObject.FindGameObjectWithTag("Player"));
                int color = 0;
                if (hitDamage < maxDamage / 3)
                    color = 0;
                else if (hitDamage > maxDamage / 3 && hitDamage < (maxDamage / 3) * 2)
                    color = 1;
                else if (hitDamage > (maxDamage / 3) * 2)
                    color = 2;
                ShowDamage(hitDamage, collision.GetContact(0).point, color);
                GetComponent<PickableObject>().DurabilityDamage(durabilityDamage);
            }
        }

        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && !GetComponent<PickableObject>().isPickedUp)
        {
            isThrown = false;
        }
    }

    IEnumerator Stick(Collider2D collider, Vector2 targetPos)
    {
        float elapsedTime = 0;
        float duration = 0.1f;
        Physics2D.IgnoreCollision(GetComponent<CapsuleCollider2D>(), collider, true);
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().angularVelocity = 0;
        GetComponent<Rigidbody2D>().simulated = false;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector2.Lerp(transform.position, targetPos, elapsedTime / duration);
            yield return null;
        }
        transform.SetParent(collider.gameObject.transform);
    }

    CreatureHealth GetScript(GameObject objectToSearch)
    {
        if (objectToSearch.GetComponent<CreatureHealth>() != null)
        {
            //Debug.Log(objectToSearch.name);
            return objectToSearch.GetComponent<CreatureHealth>();
        }
        else
        {
            return GetScript(objectToSearch.transform.parent.gameObject);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.tag == "Creature")
            isDamaging = false;


        if (((1 << collision.gameObject.layer) & groundLayer) != 0 && !GetComponent<PickableObject>().isPickedUp)
        {
            isThrown = true;
        }
    }

    void ShowDamage(int damage, Vector2 pos, int color)
    {
        MessageSystem.instance.WriteMessage(damage.ToString(), pos, color);
    }
}
