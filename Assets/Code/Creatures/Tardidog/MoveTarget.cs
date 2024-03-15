using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTarget : MonoBehaviour
{
    [SerializeField] float raycastLength;
    [SerializeField] float heightCheckLength;
    [SerializeField] float raycastOffset;

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D groundCheck1 = Physics2D.Raycast(new Vector2(transform.position.x + raycastOffset, transform.position.y), Vector2.down, raycastLength, LayerMask.GetMask("Ground"));
        RaycastHit2D groundCheck2 = Physics2D.Raycast(new Vector2(transform.position.x - raycastOffset, transform.position.y), Vector2.down, raycastLength, LayerMask.GetMask("Ground"));
        RaycastHit2D heightCheck = Physics2D.Raycast(transform.position, Vector2.down, heightCheckLength, LayerMask.GetMask("Ground"));

        if (heightCheck)
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, transform.position.y - 0.2f), 0.2f);

        if (!groundCheck1)
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x - 0.2f, transform.position.y), 0.2f);
        else if (!groundCheck2)
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x + 0.2f, transform.position.y), 0.2f);

    }

    void OnDrawGizmo()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector2.down * heightCheckLength);
        Gizmos.DrawRay(new Vector2(transform.position.x + raycastOffset, transform.position.y), Vector2.down * raycastLength);
        Gizmos.DrawRay(new Vector2(transform.position.x - raycastOffset, transform.position.y), Vector2.down * raycastLength);
    }
}
