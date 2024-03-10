using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TardidogAttack : MonoBehaviour
{
    [SerializeField] GameObject creature;
    [SerializeField] float damage;

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

    [SerializeField] CreatureState state;
    bool isAttacking;

    [SerializeField] Collider2D headCollider;
    [SerializeField] Collider2D bodyCollider;
    [SerializeField] Collider2D hornCollider;

    private void OnEnable()
    {
        ogAngle = -15.25f;
        Physics2D.IgnoreCollision(headCollider, bodyCollider);
        Physics2D.IgnoreCollision(headCollider, hornCollider);
    }

    private void Update()
    {
        facingDirection = creature.GetComponent<TardidogMovement>().facingDirection;
        if (!state.isAttacking)
        {
            if (!isAttacking)
            {
                float angle = -yCurve.Evaluate(Time.time % 2) * rotateFactor;
                neck.transform.rotation = Quaternion.Lerp(neck.transform.rotation, Quaternion.Euler(0, 0, angle * facingDirection), Time.deltaTime * rotateSpeed);
            }
            else
            {
                //Debug.Log("Up");
                StartCoroutine(BendNeck(false));
            }

        }
        else if (state.isAttacking && !isAttacking)
        {
            //Debug.Log("Down");
            //neck.transform.eulerAngles = new Vector3(0, 0, ogAngle);
            isAttacking = true;
            StartCoroutine(BendNeck(true));
        }
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
                //float angle = Mathf.LerpAngle(ogAngle, bentDownAngle, timer / duration);
                //neck.transform.eulerAngles = new Vector3(0, 0, angle * facingDirection);
                Quaternion targetAngle = Quaternion.Euler(0, 0, bentDownAngle * facingDirection);
                neck.transform.rotation = Quaternion.Lerp(neck.transform.rotation, targetAngle, timer / duration);
                yield return null;
            }
            //neck.transform.eulerAngles = new Vector3(0, 0, bentDownAngle);
        }
        else
        {
            while (timer < duration)
            {
                timer += Time.deltaTime;
                //float angle = Mathf.LerpAngle(bentDownAngle, ogAngle, timer / duration);
                //neck.transform.eulerAngles = new Vector3(0, 0, angle * facingDirection);
                Quaternion targetAngle = Quaternion.Euler(0, 0, ogAngle * facingDirection);
                neck.transform.rotation = Quaternion.Lerp(neck.transform.rotation, targetAngle, timer / duration);
                yield return null;
            }
            //neck.transform.eulerAngles = new Vector3(0, 0, ogAngle);
            isAttacking = false;
        }
    }

    /*
    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(head.position, new Vector2(head.position.x + hornOffset.x, head.position.y + hornOffset.y), hornRaycastLength, LayerMask.GetMask("Player", "Creature"));
        if (hit.collider != null && hit.collider.gameObject.tag == "Player")
        {
            hit.collider.gameObject.GetComponent<PlayerPermanent>().ChangeHp(-damage, true, gameObject);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(head.position, new Vector2(head.position.x + hornOffset.x, head.position.y + hornOffset.y) * hornRaycastLength);
    }
    */
}
