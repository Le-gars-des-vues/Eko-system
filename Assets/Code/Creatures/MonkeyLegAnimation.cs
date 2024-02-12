using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyLegAnimation : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float normalAnimSpeed;
    [SerializeField] private float fastAnimSpeed;
    [SerializeField] private float threshold;
    float targetDistance;
    float footMovement;
    float stepTimer;

    [SerializeField] private Transform nextTarget;
    [SerializeField] private Transform currentTarget;

    // Start is called before the first frame update
    void Start()
    {
        //On fixe la cible initiale au sol
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y), -Vector2.up, 5f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
            currentTarget.position = new Vector2(hit.point.x, hit.point.y);
        }

        speed = normalAnimSpeed;
    }

    // Update is called once per frame
    void Update()
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
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, currentTarget.position.y)/* + yCurve.Evaluate(stepTimer))*/, speed * Time.deltaTime);

        }
        else
        {
            //Reset le timer
            stepTimer = 0;
            transform.position = currentTarget.position;
        }
    }
}
