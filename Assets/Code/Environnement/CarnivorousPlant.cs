using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    private float playerDist;
    private bool isAttacking;
    private float attackTimer;

    [SerializeField] private float attackThreshold;
    [SerializeField] private float attackGracePeriod;
    [SerializeField] private float attackCooldown;

    private bool goingRight = true;
    public bool isFacingRight;
    public bool playerToTheRight;
    public bool isDrawingGizmos;

    [SerializeField] private AnimationCurve curve;
    private float animSpeed;
    private float stepTimer;

    [SerializeField] private GameObject vine;
    private Vector3 attackPoint;

    //public float angle;

    // Start is called before the first frame update
    void Start()
    {
        pos1 = new Vector2(transform.position.x + pos1Offsets.x, transform.position.y + pos1Offsets.y);
        pos2 = new Vector2(transform.position.x + pos2Offsets.x, transform.position.y + pos2Offsets.y);

        target.position = pos1;

        attackTimer = 0;
        isFacingRight = true;

        float difference = pos1.x - pos2.x;
        animSpeed = (difference / (difference * (difference / 10)) * (speed / 3));

        distanceFromBase = vine.GetComponent<LineRendererProceduralAnim>().segmentAmount * vine.GetComponent<LineRendererProceduralAnim>().segmentsLength;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(Vector2.Distance(vine.transform.position, transform.position));
        vineTarget.position = transform.position;

        if (player != null)
            playerToTheRight = target.position.x - transform.position.x > 0 ? true : false;

        if (isOpened)
        {
            attackTimer += Time.deltaTime;

            target.position = player.transform.position;

            Vector2 direction = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            //Quaternion startAngle = transform.rotation;
            //Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
            //transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, 0, angle), rotateSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0, 0, angle);

            playerDist = Vector2.Distance(target.position, transform.position);

            if (!isAttacking)
            {
                if (playerDist > distanceFromPlayer && Vector2.Distance(vine.transform.position, transform.position) <= distanceFromBase)
                    transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

                else if (playerDist < distanceFromPlayer - 0.1f && Vector2.Distance(vine.transform.position, transform.position) <= distanceFromBase)
                {
                    Vector3 awayDir = (transform.position - target.position).normalized;
                    transform.position = Vector2.MoveTowards(transform.position, transform.position + awayDir, speed * Time.deltaTime);
                }
            }

            if (attackTimer > attackThreshold && !isAttacking)
            {
                attackPoint = target.position;
                isAttacking = true;
            }
            if (attackTimer > attackThreshold + attackGracePeriod && isAttacking)
            {
                if (Vector2.Distance(vine.transform.position, transform.position) <= distanceFromBase)
                    transform.position = Vector2.MoveTowards(transform.position, attackPoint, (speed * 10) * Time.deltaTime);

                if (attackTimer > attackThreshold + attackGracePeriod + attackCooldown)
                {
                    float pos1Dist = Vector2.Distance(transform.position, pos1);
                    float pos2Dist = Vector2.Distance(transform.position, pos2);

                    goingRight = (pos1Dist < pos2Dist) ? true : false;

                    if (goingRight)
                        target.position = pos1;
                    else
                        target.position = pos2;

                    isFacingRight = playerToTheRight;
                    if (!isFacingRight)
                        Turn(false);

                    isAttacking = false;
                    isOpened = false;
                    attackTimer = 0;
                    stepTimer = 0;
                }
            }
        }
        else
        {
            float facingDirection = goingRight ? -1 : 1;
            stepTimer += Time.deltaTime * animSpeed;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(target.position.x, target.position.y + (curve.Evaluate(stepTimer) * facingDirection)), speed * Time.deltaTime);

            if (goingRight)
            {
                if (Vector2.Distance(transform.position, pos1) < 0.1f)
                {
                    goingRight = false;
                    Turn(true);
                    target.position = pos2;
                    stepTimer = 0;
                }
            }
            else
            {
                if (Vector2.Distance(transform.position, pos2) < 0.1f)
                {
                    goingRight = true;
                    Turn(true);
                    target.position = pos1;
                    stepTimer = 0;
                }
            }
        }
    }

    void Turn(bool isTurning)
    {
        Vector2 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        if (isTurning) isFacingRight = !isFacingRight;

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
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(direction.x, 0.1f) * collision.gameObject.GetComponent<PlayerPermanent>().knockBackForce * 1.5f, ForceMode2D.Impulse);
                player = collision.gameObject;

                if (!isFacingRight)
                    Turn(false);

                isOpened = true;
            }
            else
                collision.gameObject.GetComponent<PlayerPermanent>().ChangeHp(-damage, true, gameObject);
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
