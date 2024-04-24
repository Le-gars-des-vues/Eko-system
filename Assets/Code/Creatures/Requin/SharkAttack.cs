using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkAttack : MonoBehaviour
{
    [SerializeField] float chargeDamage;

    [Header("Shark Variables")]
    [SerializeField] CreatureState state;
    [SerializeField] SharkMovement shark;
    [SerializeField] Transform target;
    [SerializeField] Rigidbody2D rb;

    [Header("Charge Variables")]
    [SerializeField] Collider2D hurtBox;
    [SerializeField] float chargeDelay;
    [SerializeField] float chargeForce;
    [SerializeField] float chargeCooldown;
    Coroutine charge;
    public bool isCharging;
    Vector2 chargeTarget;
    [SerializeField] float chargeDistanceThreshold;

    [Header("Wiggle Variables")]
    [SerializeField] Tentacles body;
    [SerializeField] float chargeWiggleSpeed;
    [SerializeField] float chargeWiggleMagnitude;

    bool isInCombat;

    [Header("Bite Variables")]
    [SerializeField] float biteDamage;
    [SerializeField] float biteDistanceThreshold;
    [SerializeField] float biteCooldown;

    float damage;
    float attackTime;
    bool isBiting;

    private void Update()
    {
        if (state.isAttacking && !isInCombat)
        {
            isInCombat = true;
            attackTime = Time.time;
        }
        else if (!state.isAttacking && isInCombat)
            isInCombat = false;

        if (isInCombat)
        {
            if (Vector2.Distance(transform.position, target.position) > chargeDistanceThreshold)
            {
                if (Time.time - attackTime > chargeCooldown && !isCharging && !isBiting)
                {
                    charge = StartCoroutine(Charge());
                }
            }
            else if (Vector2.Distance(transform.position, target.position) < biteDistanceThreshold)
            {
                if (Time.time - attackTime > biteCooldown && !isBiting && !isCharging)
                {
                    StartCoroutine(BiteAttack());
                    Debug.Log("is biting!");
                }
            }
        }
    }

    IEnumerator Charge()
    {
        isCharging = true;
        damage = chargeDamage;
        shark.head.GetComponent<BodyRotation>().enabled = false;
        chargeTarget = target.position;
        body.wiggleSpeed = chargeWiggleSpeed;
        body.wiggleMagnitude = chargeWiggleMagnitude;
        rb.velocity = Vector2.zero;

        float elapsedTime = 0;
        float angle = shark.isFacingRight ? 60 : -60;
        while (elapsedTime < chargeDelay)
        {
            elapsedTime += Time.deltaTime;
            shark.head.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, angle), elapsedTime / chargeDelay);
            //Vector2 scale = Vector2.Lerp(shark.head.localScale, new Vector2(2, shark.head.localScale.y), elapsedTime / chargeDelay);
            //shark.head.localScale = scale;
            yield return null;
        }
        shark.head.localRotation = Quaternion.Euler(0, 0, angle);
        body.canShorten = false;
        body.wiggleSpeed = 0;
        body.wiggleMagnitude = 0;
        Vector2 direction = (chargeTarget - rb.position).normalized;
        hurtBox.enabled = true;
        rb.AddForce(direction * chargeForce, ForceMode2D.Impulse);
        shark.head.localScale = new Vector2(2, shark.head.localScale.y);



        yield return new WaitForSeconds(chargeDelay / 2);
        shark.head.localRotation = Quaternion.Euler(0, 0, 0);
        shark.head.localScale = new Vector2(1, shark.head.localScale.y);
        shark.head.GetComponent<BodyRotation>().enabled = true;

        yield return new WaitForSeconds(chargeDelay);
        hurtBox.enabled = false;
        body.canShorten = true;
        isCharging = false;
        attackTime = Time.time;
    }

    IEnumerator BiteAttack()
    {
        isBiting = true;
        damage = biteDamage;
        shark.head.GetComponent<BodyRotation>().enabled = false;

        float elapsedTime = 0;
        float angle = shark.isFacingRight ? 60 : -60;
        while (elapsedTime < 0.5f)
        {
            elapsedTime += Time.deltaTime;
            shark.head.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, angle), elapsedTime / 0.5f);
            yield return null;
        }
        shark.head.localRotation = Quaternion.Euler(0, 0, angle);
        hurtBox.enabled = true;

        yield return new WaitForSeconds(0.1f);
        shark.head.localRotation = Quaternion.Euler(0, 0, 0);
        shark.head.GetComponent<BodyRotation>().enabled = true;
        hurtBox.enabled = false;
        attackTime = Time.time;
        isBiting = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerPermanent>().ChangeHp(-damage, true, gameObject);
            if (charge != null)
            {
                StopCoroutine(charge);
                shark.head.localRotation = Quaternion.Euler(0, 0, 0);
                shark.head.localScale = new Vector2(1, shark.head.localScale.y);
                shark.head.GetComponent<BodyRotation>().enabled = true;
                hurtBox.enabled = false;
                body.canShorten = true;
                isCharging = false;
                attackTime = Time.time;
            }
        }
    }
}
