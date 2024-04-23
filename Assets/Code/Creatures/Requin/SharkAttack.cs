using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkAttack : MonoBehaviour
{
    [SerializeField] float damage;

    [Header("Shark Variables")]
    [SerializeField] CreatureState state;
    [SerializeField] SharkMovement shark;
    [SerializeField] Transform target;
    [SerializeField] Rigidbody2D rb;

    [Header("Charge Variables")]
    Coroutine charge;
    [SerializeField] Collider2D hurtBox;
    [SerializeField] float chargeCooldown;
    [SerializeField] float chargeDelay;
    [SerializeField] float chargeForce;
    float chargeTime;
    public bool isCharging;
    Vector2 chargeTarget;

    [Header("Wiggle Variables")]
    [SerializeField] Tentacles body;
    [SerializeField] float chargeWiggleSpeed;
    [SerializeField] float chargeWiggleMagnitude;

    bool isInCombat;

    private void Update()
    {
        if (state.isAttacking && !isInCombat)
        {
            isInCombat = true;
            chargeTime = Time.time;
        }
        else if (!state.isAttacking && isInCombat)
            isInCombat = false;

        if (isInCombat)
        {
            if (Time.time - chargeTime > chargeCooldown && !isCharging)
            {
                charge = StartCoroutine(Charge());
            }
        }
    }

    IEnumerator Charge()
    {
        isCharging = true;
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
        chargeTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
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
                chargeTime = Time.time;
            }
        }
    }
}
