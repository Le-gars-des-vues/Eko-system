using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnderwaterCurrent : MonoBehaviour
{
    [SerializeField] float pushForce;
    [SerializeField] float pushDuration;
    [SerializeField] float pushInterval;
    [SerializeField] Transform pushOrigin;
    bool isPushing = false;
    float pushTime;

    [SerializeField] ParticleSystem particles;

    private void Start()
    {
        float lifeTime = 0.05f * transform.localScale.x;

        var main = particles.main;
        main.startLifetime = lifeTime;
    }

    private void Update()
    {
        if (Time.time - pushTime > pushInterval && !isPushing)
        {
            isPushing = true;
            particles.Play();
            pushTime = Time.time;
        }
        else if (Time.time - pushTime > pushDuration && isPushing)
        {
            isPushing = false;
            pushTime = Time.time;
            particles.Stop();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (isPushing)
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(pushForce * transform.right);
        }
    }

    private void OnDrawGizmos()
    {
        float spriteLength = Mathf.Abs(GetComponent<SpriteRenderer>().sprite.bounds.extents.x) * 2;
        Gizmos.color = Color.green;
        Gizmos.DrawRay(pushOrigin.position, spriteLength * transform.localScale.x * transform.right);
    }
}
