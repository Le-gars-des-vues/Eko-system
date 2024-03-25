using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JellyFishTentacle : MonoBehaviour
{
    [SerializeField] GameObject head;
    [SerializeField] Transform target;
    [SerializeField] Transform tentacleTarget;
    [SerializeField] float pullForce;
    [SerializeField] float speed;
    bool isGrabbing;

    private void Update()
    {
        if (target != null)
        {
            tentacleTarget.position = Vector2.Lerp(tentacleTarget.position, target.position, speed * Time.deltaTime);
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

            //GetComponent<LineRenderer>().useWorldSpace = false
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
}
