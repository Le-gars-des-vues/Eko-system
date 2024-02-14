using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarnivorousPlant : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Transform[] tentacleTargets;
    [SerializeField] private Transform vineTarget;

    private GameObject player;
    [SerializeField] private float distanceFromPlayer;
    [SerializeField] private float distanceFromBase;
    [SerializeField] private float getawayDistance;

    private Vector2 pos1;
    [SerializeField] private Vector2 pos1Offsets;

    private Vector2 pos2;
    [SerializeField] private Vector2 pos2Offsets;

    [SerializeField] private float speed;
    [SerializeField] private float rotateSpeed;
    [SerializeField] private float damage;

    private bool isOpened = false;
    public float playerDist;
    public float playerDistCheck;

    private bool goingRight = true;
    private bool isDrawingGizmos = true;

    //public float angle;

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
        vineTarget.position = transform.position;
        /*
        if (Mathf.DeltaAngle(transform.rotation.z, angle) < 0.1f)
            transform.position = Vector2.MoveTowards(transform.position, pos2, speed * Time.deltaTime);
        */

        if (isOpened)
        {
            target.position = player.transform.position;

            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //Quaternion startAngle = transform.rotation;
            //Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);

            playerDist = Vector2.Distance(target.position, transform.position);

            if (playerDist > distanceFromPlayer && Vector2.Distance(transform.parent.transform.position, transform.position) <= distanceFromBase)
                transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

            else if (playerDist < distanceFromPlayer - 0.1f)
            {
                Vector3 awayDir = (transform.position - target.position).normalized;
                transform.position = Vector2.MoveTowards(transform.position, transform.position + awayDir, speed * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            Debug.Log(Vector2.Distance(transform.position, pos1));

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
    }

    void Turn()
    {
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        foreach (Transform target in tentacleTargets)
            target.right = -target.right;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision != null && collision.gameObject.tag == "Player")
        {
            if (!isOpened)
            {
                Vector2 direction = (collision.gameObject.transform.position - transform.position).normalized;
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction.x, 0.2f) * collision.gameObject.GetComponent<PlayerPermanent>().knockBackForce * 1.5f, ForceMode2D.Impulse);
                player = collision.gameObject;
                Turn();
                isOpened = true;
            }
            else
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
