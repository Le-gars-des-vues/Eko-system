using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TardidogHead : MonoBehaviour
{
    [Header("Creature Variables")]
    [SerializeField] TardidogMovement dog;
    [SerializeField] CreatureState state;

    [Header("Bend Variables")]
    [SerializeField] GameObject neck;
    [SerializeField] float duration;
    float timer;
    float ogAngle;
    [SerializeField] float bentDownAngle;
    [SerializeField] float bentUpAngle;
    [SerializeField] AnimationCurve yCurve;
    [SerializeField] float rotateFactor;
    [SerializeField] float rotateSpeed;
    float facingDirection;

    [Header("Charge Variables")]
    public bool isCharging;
    Coroutine charge;
    [SerializeField] float damage;
    [SerializeField] float headbuttThreshold;
    [SerializeField] float knockBackForce;

    [Header("Colliders")]
    [SerializeField] Collider2D headCollider;
    [SerializeField] Collider2D bodyCollider;
    [SerializeField] Collider2D hornCollider;

    private void OnEnable()
    {
        ogAngle = -15.25f;
        Physics2D.IgnoreCollision(headCollider, bodyCollider);
        Physics2D.IgnoreCollision(headCollider, hornCollider);
        Physics2D.IgnoreCollision(hornCollider, bodyCollider);
    }

    private void Update()
    {
        facingDirection = dog.facingDirection;
        if (!state.isAttacking)
        {
            if (!isCharging)
            {
                float angle = -yCurve.Evaluate(Time.time % 2) * rotateFactor;
                neck.transform.rotation = Quaternion.Lerp(neck.transform.rotation, Quaternion.Euler(0, 0, angle * facingDirection), Time.deltaTime * rotateSpeed);
            }
            else if (isCharging && !dog.targetIsInFront)
            {
                if (charge != null)
                    StopCoroutine(charge);
                charge = StartCoroutine(BendNeck(false));
            }
        }
        else if (state.isAttacking && !isCharging && dog.targetIsInFront)
        {
            isCharging = true;
            if (charge != null)
                StopCoroutine(charge);
            charge = StartCoroutine(BendNeck(true));
        }
        else
        {
            if (charge != null)
                StopCoroutine(charge);
            charge = StartCoroutine(BendNeck(false));
        }

        if (state.isTamed)
            Physics2D.IgnoreCollision(hornCollider, GameObject.FindGameObjectWithTag("Player").GetComponent<CapsuleCollider2D>());
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerPermanent>().ChangeHp(-damage, true, gameObject);
        }
    }

    public IEnumerator BendNeck(bool bentDown)
    {
        timer = 0;
        if (bentDown)
        {
            while (timer < duration)
            {
                timer += Time.deltaTime;
                Quaternion targetAngle = Quaternion.Euler(0, 0, bentDownAngle * facingDirection);
                neck.transform.rotation = Quaternion.Lerp(neck.transform.rotation, targetAngle, timer / duration);
                yield return null;
            }
        }
        else
        {
            while (timer < duration)
            {
                timer += Time.deltaTime;
                Quaternion targetAngle = Quaternion.Euler(0, 0, ogAngle * facingDirection);
                neck.transform.rotation = Quaternion.Lerp(neck.transform.rotation, targetAngle, timer / duration);
                yield return null;
            }
            isCharging = false;
        }
    }
}
