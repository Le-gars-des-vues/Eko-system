using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkAttack : MonoBehaviour
{
    [Header("Shark Variables")]
    [SerializeField] CreatureState state;
    [SerializeField] SharkMovement shark;
    [SerializeField] Transform jaw;
    [SerializeField] Transform target;
    [SerializeField] Rigidbody2D rb;

    [Header("Charge Variables")]
    [SerializeField] Collider2D hurtBox;
    [SerializeField] float chargeDamage;
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
    [SerializeField] float biteDelay = 0.2f;

    float damage;
    float attackTime;
    public bool isBiting;

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
                    attackTime = Time.time;
                    Debug.Log("is biting!");
                }
            }
        }
    }

    IEnumerator Charge()
    {
        //Commencer la charge et assigne le degats
        isCharging = true;
        damage = chargeDamage;

        //Empeche la tete de tourner durant la charge
        shark.head.GetComponent<BodyRotation>().enabled = false;
        jaw.gameObject.GetComponent<BodyRotation>().enabled = false;

        //Assigne la cible de la charge
        chargeTarget = target.position;

        //Commence a shaker et arrete le deplacement
        body.wiggleSpeed = chargeWiggleSpeed;
        body.wiggleMagnitude = chargeWiggleMagnitude;
        rb.velocity = Vector2.zero;

        //Shake et reste immobile le temps d'ouvrir la machoire
        float elapsedTime = 0;

        //Angle selon la direction du requin
        float angle = shark.isFacingRight ? 20 : -20;
        while (elapsedTime < chargeDelay)
        {
            elapsedTime += Time.deltaTime;
            //Ouvre la machoire
            shark.head.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, angle), elapsedTime / chargeDelay);
            jaw.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, -angle), elapsedTime / chargeDelay);

            //Fait grandir la machoire pour donner plus d'impacte
            float scale = Mathf.Lerp(1, 1.5f, elapsedTime / biteDelay);
            shark.head.localScale = new Vector2(scale, shark.head.localScale.y);
            jaw.localScale = new Vector2(scale, shark.head.localScale.y);
            yield return null;
        }

        //Assigne les valeur final de l'ouverture
        shark.head.localRotation = Quaternion.Euler(0, 0, angle);
        jaw.localRotation = Quaternion.Euler(0, 0, -angle);
        shark.head.localScale = new Vector2(1.5f, shark.head.localScale.y);
        jaw.localScale = new Vector2(1.5f, shark.head.localScale.y);

        //Empeche le corps de s'etirer
        body.canShorten = false;

        //Arrete de shaker
        body.wiggleSpeed = 0;
        body.wiggleMagnitude = 0;

        //Prend la direction de la charge et fonce en activant la hurtbox
        Vector2 direction = (chargeTarget - rb.position).normalized;
        hurtBox.enabled = true;
        rb.AddForce(direction * chargeForce, ForceMode2D.Impulse);

        //Attend un moment puis referme la bouche
        yield return new WaitForSeconds(chargeDelay / 2);
        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime;
            shark.head.localRotation = Quaternion.Slerp(shark.head.localRotation, Quaternion.Euler(0, 0, 0), elapsedTime / biteDelay);
            jaw.localRotation = Quaternion.Slerp(jaw.localRotation, Quaternion.Euler(0, 0, 0), elapsedTime / biteDelay);
            float scale = Mathf.Lerp(1.5f, 1, elapsedTime / biteDelay);
            shark.head.localScale = new Vector2(scale, shark.head.localScale.y);
            jaw.localScale = new Vector2(scale, shark.head.localScale.y);
            yield return null;
        }
        //Remet les valeurs de base
        shark.head.localRotation = Quaternion.Euler(0, 0, 0);
        jaw.localRotation = Quaternion.Euler(0, 0, 0);
        shark.head.localScale = new Vector2(1, shark.head.localScale.y);
        jaw.localScale = new Vector2(1, shark.head.localScale.y);
        shark.head.GetComponent<BodyRotation>().enabled = true;
        jaw.gameObject.GetComponent<BodyRotation>().enabled = true;

        //Desactive la hurtbox et reinitialise l'attaque
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
        jaw.gameObject.GetComponent<BodyRotation>().enabled = false;

        float elapsedTime = 0;
        float angle = shark.isFacingRight ? 20 : -20;
        while (elapsedTime < biteDelay)
        {
            elapsedTime += Time.deltaTime;
            shark.head.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, angle), elapsedTime / biteDelay);
            jaw.localRotation = Quaternion.Slerp(Quaternion.Euler(0, 0, 0), Quaternion.Euler(0, 0, -angle), elapsedTime / biteDelay);
            float scale = Mathf.Lerp(1, 1.5f, elapsedTime / biteDelay);
            shark.head.localScale = new Vector2(scale, shark.head.localScale.y);
            jaw.localScale = new Vector2(scale, shark.head.localScale.y);
            yield return null;
        }
        shark.head.localRotation = Quaternion.Euler(0, 0, angle);
        jaw.localRotation = Quaternion.Euler(0, 0, -angle);
        shark.head.localScale = new Vector2(1.5f, shark.head.localScale.y);
        jaw.localScale = new Vector2(1.5f, shark.head.localScale.y);

        yield return new WaitForSeconds(0.1f);
        hurtBox.enabled = true;
        elapsedTime = 0;
        while (elapsedTime < 0.1f)
        {
            elapsedTime += Time.deltaTime;
            shark.head.localRotation = Quaternion.Slerp(shark.head.localRotation, Quaternion.Euler(0, 0, 0), elapsedTime / biteDelay);
            jaw.localRotation = Quaternion.Slerp(jaw.localRotation, Quaternion.Euler(0, 0, 0), elapsedTime / biteDelay);
            float scale = Mathf.Lerp(1.5f, 1, elapsedTime / biteDelay);
            shark.head.localScale = new Vector2(scale, shark.head.localScale.y);
            jaw.localScale = new Vector2(scale, shark.head.localScale.y);
            yield return null;
        }
        shark.head.localRotation = Quaternion.Euler(0, 0, 0);
        jaw.localRotation = Quaternion.Euler(0, 0, 0);
        shark.head.localScale = new Vector2(1, shark.head.localScale.y);
        jaw.localScale = new Vector2(1, shark.head.localScale.y);
        shark.head.GetComponent<BodyRotation>().enabled = true;
        jaw.gameObject.GetComponent<BodyRotation>().enabled = true;

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
                jaw.localRotation = Quaternion.Euler(0, 0, 0);
                shark.head.localScale = new Vector2(1, shark.head.localScale.y);
                jaw.localScale = new Vector2(1, shark.head.localScale.y);
                shark.head.GetComponent<BodyRotation>().enabled = true;
                jaw.gameObject.GetComponent<BodyRotation>().enabled = true;
                hurtBox.enabled = false;
                body.canShorten = true;
                isCharging = false;
                attackTime = Time.time;
            }
        }
    }
}
