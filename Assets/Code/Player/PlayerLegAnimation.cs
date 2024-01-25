using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegAnimation : MonoBehaviour
{
    [Header("Targets")]
    public Transform currentTarget;
    public Transform desiredTarget;
    public Transform neutralTarget;

    [Header("Animation Variables")]
    public float speed;
    public float threshold;
    float targetDistance;
    float footMovement;
    float stepTimer;
    public AnimationCurve yCurve;
    private bool isAnimated;

    public PlayerLegAnimation otherfoot;

    // Start is called before the first frame update
    void Start()
    {
        //On fixe la cible initiale au sol
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y + 5), -Vector2.up, 12f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            currentTarget.position = new Vector3(hit.point.x, hit.point.y, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        targetDistance = Vector2.Distance(currentTarget.position, desiredTarget.position);

        if (targetDistance > threshold && otherfoot.targetDistance > 0.7f)
        {
            currentTarget.position = desiredTarget.position;
        }

        //Distance entre la cible actuelle et la position du pied
        footMovement = Vector2.Distance(transform.position, currentTarget.position);

        //Si le pied est en train de bouger
        if (footMovement > 0.1f && isAnimated)
        {
            //On augmente le timer pour la courbe d'animation
            stepTimer += Time.deltaTime;

            //On bouge le pied en ajoutant de la hauteur selon la courbe d'animation
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, currentTarget.position.y + yCurve.Evaluate(stepTimer)), speed * Time.deltaTime);

        }
        else
        {
            //Reset le timer
            stepTimer = 0;
            transform.position = currentTarget.position;
        }

        if (Mathf.Abs(Input.GetAxis("Horizontal")) < 0.1f)
        {
            speed = 1f;
            isAnimated = false;
            currentTarget.position = neutralTarget.position;
        }
        else
        {
            isAnimated = true;
            speed = 7f;
        }
    }
}
