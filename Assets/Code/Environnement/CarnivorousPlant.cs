using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivorousPlant : MonoBehaviour
{
    [SerializeField] private Transform target;

    private Vector2 pos1;
    [SerializeField] private Vector2 pos1Offsets;

    private Vector2 pos2;
    [SerializeField] private Vector2 pos2Offsets;

    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float damage;

    private bool goingRight = true;
    private bool isDrawingGizmos = true;

    public float angle;

    // Start is called before the first frame update
    void Start()
    {
        pos1 = new Vector2(transform.position.x + pos1Offsets.x, transform.position.y + pos1Offsets.y);
        pos2 = new Vector2(transform.position.x + pos2Offsets.x, transform.position.y + pos2Offsets.y);
        isDrawingGizmos = false;
        target.position = pos1;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        Vector2 direction = (target.position - transform.position).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion startAngle = transform.rotation;
        transform.rotation = Quaternion.Slerp(startAngle, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
        */
        /*
        if (Mathf.DeltaAngle(transform.rotation.z, angle) < 0.1f)
            transform.position = Vector2.MoveTowards(transform.position, pos2, speed * Time.deltaTime);
        */

        transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        if (goingRight)
        {
            if (Vector2.Distance(transform.position, pos1) < 0.1f)
            {
                goingRight = false;
                Turn();
                target.position = pos2;
            }
        }
        else
        {
            if (Vector2.Distance(transform.position, pos2) < 0.1f)
            {
                goingRight = true;
                Turn();
                target.position = pos1;
            }
        }
    }

    void Turn()
    {
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && collision.gameObject.tag == "Player")
        {
            collision.gameObject.GetComponent<PlayerPermanent>().ChangeHp(-damage, gameObject, true);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        if (isDrawingGizmos)
        {
            Gizmos.DrawSphere(new Vector2(transform.position.x + pos1Offsets.x, transform.position.y + pos1Offsets.y), 0.1f);
            Gizmos.DrawSphere(new Vector2(transform.position.x + pos2Offsets.x, transform.position.y + pos2Offsets.y), 0.1f);
        }
        
        Gizmos.DrawSphere(pos1, 0.1f);
        Gizmos.DrawSphere(pos2, 0.1f);
    }
}
