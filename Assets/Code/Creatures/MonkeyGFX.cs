using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyGFX : MonoBehaviour
{
    private MonkeyPathfinding pathfinding;
    private Rigidbody2D rb;
    [SerializeField] private float monkeyHeight;
    [SerializeField] private float speed;
    public bool isFacingRight;
    private bool isMovingRight = false;

    // Start is called before the first frame update
    void Start()
    {
        pathfinding = GetComponent<MonkeyPathfinding>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        isFacingRight = pathfinding.target.position.x - transform.position.x > 0 ? true : false;
        
        if (isMovingRight != isFacingRight)
        {
            Turn();
        }

        isMovingRight = isFacingRight;

        Vector2 direction = new Vector2(rb.velocity.x, rb.velocity.y).normalized;
        transform.right = Vector2.MoveTowards(transform.right, direction, speed * Time.deltaTime);
    }

    void Turn()
    {
        Vector3 scale = gameObject.transform.localScale;
        scale.x *= -1;
        gameObject.transform.localScale = scale;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, -transform.up * monkeyHeight);
    }
}
