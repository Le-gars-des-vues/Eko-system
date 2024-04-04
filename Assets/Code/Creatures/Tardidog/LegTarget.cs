using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegTarget : MonoBehaviour
{
    Vector2 desiredPosition;
    [SerializeField] float Offset;
    private TardidogMovement dog;

    private void Start()
    {
        dog = transform.parent.gameObject.GetComponent<TardidogMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        float facingDirection = dog.isFacingRight ? 1 : -1;

        //Raycast qui entre en collision avec le sol
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.parent.position.x + Offset * facingDirection, transform.parent.position.y), Vector2.down, 2f, LayerMask.GetMask("Ground"));

        //Si on touche le sol, on met la position en Y de la prochaine cible a celle de la collision (on colle la cible suivant au sol en gros)     
        if (hit.collider != null)
        {
            desiredPosition = hit.point;
        }
        else
        {
            desiredPosition = transform.position;
        }

        transform.position = desiredPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(new Vector2(transform.parent.position.x + Offset, transform.position.y), 0.05f);
    }
}
