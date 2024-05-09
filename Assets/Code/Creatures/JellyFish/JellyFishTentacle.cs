using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFishTentacle : MonoBehaviour
{
    /*
    [SerializeField] GameObject head;
    [SerializeField] Transform target;
    [SerializeField] Transform tentacleTarget;
    [SerializeField] float pullForce;
    float speed;
    [SerializeField] float minSpeed;
    [SerializeField] float maxSpeed;
    [SerializeField] float maxFollowDistance;
    public bool isGrabbing;

    [SerializeField] CreatureState state;
    [SerializeField] List<GameObject> tentacles = new List<GameObject>();
    */

    [SerializeField] float damage;
    [SerializeField] StatusEffect poison;

    /*
    private void Update()
    {
        if (state.isAttacking)
        {
            if (target != null)
            {
                if (Vector2.Distance(target.position, tentacleTarget.position) < maxFollowDistance)
                {
                    speed = Mathf.Lerp(maxSpeed, minSpeed, Vector2.Distance(target.position, tentacleTarget.position) / maxFollowDistance);
                    tentacleTarget.position = Vector2.Lerp(tentacleTarget.position, target.position, speed * Time.deltaTime);
                }
                else
                {
                    isGrabbing = false;
                    GetComponent<Tentacles>().enabled = true;
                    target = null;
                    GetComponent<LineRendererProceduralAnim>().enabled = false;
                    GetComponent<EdgeCollider2D>().isTrigger = false;
                }
            }
        }
        else
        {
            isGrabbing = false;
            GetComponent<Tentacles>().enabled = true;
            target = null;
            GetComponent<LineRendererProceduralAnim>().enabled = false;
            GetComponent<EdgeCollider2D>().isTrigger = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player" && !isGrabbing)
        {
            isGrabbing = true;
            GetComponent<Tentacles>().enabled = false;
            target = collision.gameObject.transform;
            GetComponent<LineRendererProceduralAnim>().enabled = true;
            GetComponent<EdgeCollider2D>().isTrigger = true;

            foreach (GameObject tentacle in tentacles)
            {
                tentacle.GetComponent<JellyFishTentacle>().isGrabbing = true;
                tentacle.GetComponent<Tentacles>().enabled = false;
                tentacle.GetComponent<JellyFishTentacle>().target = collision.gameObject.transform;
                tentacle.GetComponent<LineRendererProceduralAnim>().enabled = true;
                tentacle.GetComponent<EdgeCollider2D>().isTrigger = true;
            }

            if (!state.isAttacking)
                state.isAttacking = true;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && isGrabbing)
        {
            Vector2 direction = (head.transform.position - target.position).normalized;
            target.gameObject.GetComponent<Rigidbody2D>().AddForce(pullForce * direction);
        }
    }
    */

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!collision.gameObject.GetComponent<PlayerPermanent>().isInvincible)
            {
                if (collision.gameObject.GetComponent<PlayerPermanent>().poison != null)
                    collision.gameObject.GetComponent<PlayerPermanent>().StopPoison();
                collision.gameObject.GetComponent<PlayerPermanent>().poison = StartCoroutine(collision.gameObject.GetComponent<PlayerPermanent>().Poison(poison.effectDuration, poison.effectMagnitude, poison.effectFrequency));
            }
            collision.gameObject.GetComponent<PlayerPermanent>().ChangeHp(-damage, true, gameObject);
        }
    }
}
