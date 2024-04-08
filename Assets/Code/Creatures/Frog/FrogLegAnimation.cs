using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FrogLegAnimation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float normalAnimSpeed;
    [SerializeField] private float fastAnimSpeed;
    [SerializeField] private float threshold;
    float targetDistance;
    float footMovement;
    [SerializeField] AnimationCurve yCurve;
    float stepTimer;
    int facingDirection;
    bool lockedIn;
    [SerializeField] private Transform nextTarget;
    [SerializeField] private Transform currentTarget;
    [SerializeField] float legOffset;

    [SerializeField] FrogMovement frog;
    [SerializeField] Vector2 jumpPosOffsets;

    // Start is called before the first frame update
    void OnEnable()
    {
        frog = transform.parent.transform.parent.GetComponent<FrogMovement>();
        //On fixe la cible initiale au sol
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), Vector2.down, 2f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            currentTarget.position = new Vector2(hit.point.x, hit.point.y);
        }
        speed = normalAnimSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        facingDirection = frog.GetComponent<FrogMovement>().isFacingRight ? 1 : -1;

        if (!frog.isJumping)
        {
            //Calcule la distance entre la cible du pied et sa position actuelle
            targetDistance = Vector2.Distance(currentTarget.position, nextTarget.position);

            //Si la distance depasse le seuile et qu'il n'y a pas de mur devant, la position du pied devient celle de la cible
            if (targetDistance > threshold)
            {
                currentTarget.position = nextTarget.position;
            }

            //Distance entre la cible actuelle et la position du pied
            footMovement = Vector2.Distance(transform.position, currentTarget.position);

            //Si le pied est en train de bouger
            if (footMovement > 0.1f)
            {
                //On augmente le timer pour la courbe d'animation
                stepTimer += Time.deltaTime;

                //On bouge le pied en ajoutant de la hauteur selon la courbe d'animation
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, currentTarget.position.y + yCurve.Evaluate(stepTimer)), speed * Time.deltaTime);

            }
            else
            {
                //Debug.Log(stepTimer);
                //Reset le timer
                stepTimer = 0;
                transform.position = currentTarget.position;
            }

            if (frog.isStopped)
            {
                RaycastHit2D hit = Physics2D.Raycast(new Vector2(frog.gameObject.transform.position.x + legOffset * frog.facingDirection, frog.gameObject.transform.position.y), -Vector2.up, 3f, LayerMask.GetMask("Ground"));
                if (hit.collider != null)
                {
                    //Debug.Log(gameObject.name);
                    //Debug.DrawRay(new Vector2(dog.gameObject.transform.position.x + nextTarget.gameObject.GetComponent<LegTarget>().Offset * dog.facingDirection, dog.gameObject.transform.position.y), -Vector2.up * 3f, Color.green);
                    currentTarget.position = Vector2.Lerp(currentTarget.position, new Vector2(hit.point.x, hit.point.y), speed * Time.deltaTime);
                }
            }
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, currentTarget.position.y), speed * Time.deltaTime);

            if (frog.GetComponent<Rigidbody2D>().velocity.y > 0f)
            {
                if (!lockedIn)
                {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 2f, LayerMask.GetMask("Ground"));
                    if (hit.collider != null)
                    {
                        currentTarget.position = hit.point;
                        lockedIn = true;
                    }
                }
            }
            else
            {
                currentTarget.position = new Vector2(frog.transform.position.x + (jumpPosOffsets.x * facingDirection), frog.transform.position.y + jumpPosOffsets.y);
                transform.position = Vector2.MoveTowards(transform.position, currentTarget.position, 5 * Time.deltaTime);
            }
        }
    }

    public void Turn()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(frog.gameObject.transform.position.x + legOffset * frog.facingDirection, frog.gameObject.transform.position.y), -Vector2.up, 3f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            currentTarget.position = hit.point;
            transform.position = hit.point;
        }
    }

    public void ResetPosition()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, 2f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            currentTarget.position = hit.point;
            transform.position = hit.point;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector2(frog.gameObject.transform.position.x + legOffset, transform.position.y), 0.05f);
        Gizmos.DrawSphere((Vector2)frog.gameObject.transform.position + jumpPosOffsets, 0.05f);
    }
}