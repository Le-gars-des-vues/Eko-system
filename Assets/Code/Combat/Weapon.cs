using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float dmgRayLenght;
    [SerializeField] int baseDamage;
    [SerializeField] int maxDamage;
    Vector3 previousPosition;
    Vector3 velocity;
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

        int hitDamange = Mathf.RoundToInt((baseDamage * velocity.magnitude) / 2);
        hitDamange = Mathf.Clamp(hitDamange, baseDamage, maxDamage);

        RaycastHit2D dmg = Physics2D.Raycast(transform.position, transform.right, dmgRayLenght, LayerMask.GetMask("Default"));
        if (dmg.collider != null && dmg.collider.gameObject.tag == "Creature")
        {
            if (!isDamaging)
            {
                isDamaging = true;
                dmg.collider.gameObject.GetComponent<CreatureHealth>().LoseHealth(hitDamange);
                int color = 0;
                if (hitDamange < maxDamage / 3)
                    color = 0;
                else if (hitDamange > maxDamage / 3 && hitDamange < (maxDamage / 3) * 2)
                    color = 1;
                else if (hitDamange > (maxDamage / 3) * 2)
                    color = 2;
                ShowDamage(hitDamange, dmg.point, color);
            }
        }
        else
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
