using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float dmgRayLenght;
    [SerializeField] int baseDamage;
    [SerializeField] int maxDamage;
    [SerializeField] float piercingAngleThreshold;
    Vector3 previousPosition;
    Vector3 velocity;
    int hitDamage;
    bool isDamaging;

    private void OnEnable()
    {
        rb = GetComponent<Rigidbody2D>();
        previousPosition = transform.position;
    }

    private void Update()
    {
        velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        hitDamage = Mathf.RoundToInt((baseDamage * velocity.magnitude) / 2);
        hitDamage = Mathf.Clamp(hitDamage, baseDamage, maxDamage);

        /*
        RaycastHit2D dmg = Physics2D.Raycast(transform.position, transform.right, dmgRayLenght, LayerMask.GetMask("Default"));
        if (dmg.collider != null && dmg.collider.gameObject.tag == "Creature")
        {
            if (gameObject.tag == "Spear")
            {
                Vector3 hitDirection = dmg.point - new Vector2(transform.position.x, transform.position.y);
                float angle = Vector3.Angle(hitDirection, transform.right);

                // Check if the angle is within the piercing threshold
                if (angle < piercingAngleThreshold)
                {
                    if (!isDamaging && !dmg.collider.gameObject.GetComponent<CreatureHealth>().isInvincible)
                    {
                        isDamaging = true;
                        dmg.collider.gameObject.GetComponent<CreatureHealth>().LoseHealth(hitDamage);
                        int color = 0;
                        if (hitDamage < maxDamage / 3)
                            color = 0;
                        else if (hitDamage > maxDamage / 3 && hitDamage < (maxDamage / 3) * 2)
                            color = 1;
                        else if (hitDamage > (maxDamage / 3) * 2)
                            color = 2;
                        ShowDamage(hitDamage, dmg.point, color);
                    }
                }
            }
        }
        else
            isDamaging = false;
        */
    }

    private void OnCollisionEnter2D(Collision2D dmg)
    {
        if (dmg.collider != null && dmg.collider.gameObject.tag == "Creature")
        {
            if (gameObject.tag == "Spear")
            {
                ContactPoint2D contact = dmg.GetContact(0);
                Vector2 hitDirection = contact.point - (Vector2)transform.position;
                Vector2 normal = contact.normal;
                float angle = Vector2.Angle(hitDirection, -normal);

                // Check if the angle is within the piercing threshold
                if (angle < piercingAngleThreshold)
                {
                    if (!isDamaging && !dmg.collider.gameObject.GetComponent<CreatureHealth>().isInvincible)
                    {
                        isDamaging = true;
                        dmg.collider.gameObject.GetComponent<CreatureHealth>().LoseHealth(hitDamage);
                        int color = 0;
                        if (hitDamage < maxDamage / 3)
                            color = 0;
                        else if (hitDamage > maxDamage / 3 && hitDamage < (maxDamage / 3) * 2)
                            color = 1;
                        else if (hitDamage > (maxDamage / 3) * 2)
                            color = 2;
                        ShowDamage(hitDamage, dmg.GetContact(0).point, color);
                    }
                }
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isDamaging = false;
    }

    void ShowDamage(int damage, Vector2 pos, int color)
    {
        MessageSystem.instance.WriteMessage(damage.ToString(), pos, color);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * dmgRayLenght);
    }
}
