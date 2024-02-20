using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Rigidbody2D rb;
    [SerializeField] float dmgRayLenght;
    [SerializeField] float baseDamage;
    [SerializeField] float maxDamage;
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

        float hitDamange = baseDamage * velocity.magnitude;
        hitDamange = Mathf.Clamp(hitDamange, baseDamage, maxDamage);

        RaycastHit2D dmg = Physics2D.Raycast(transform.position, transform.right, dmgRayLenght, LayerMask.GetMask("Default"));
        if (dmg.collider != null && dmg.collider.gameObject.tag == "Creature")
        {
            if (!isDamaging)
            {
                isDamaging = true;
                dmg.collider.gameObject.GetComponent<CreatureHealth>().LoseHealth(hitDamange);
                Debug.Log(velocity);
                Debug.Log(hitDamange);
            }
        }
        else
            isDamaging = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.right * dmgRayLenght);
    }
}
