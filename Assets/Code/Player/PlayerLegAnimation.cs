using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLegAnimation : MonoBehaviour
{
    [Header("Targets")]
    [SerializeField] private Transform currentTarget;
    [SerializeField] private Transform desiredTarget;
    private GameObject player;

    [Header("Animation Variables")]
    public float speed;
    public float threshold;
    float targetDistance;
    float footMovement;
    float stepTimer;
    public AnimationCurve yCurve;
    private bool isRunning = false;
    [SerializeField] private float legOffset;

    public PlayerLegAnimation otherfoot;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
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
        float facingDirection = player.GetComponent<GroundPlayerController>().isFacingRight ? 1 : -1;
        targetDistance = Vector2.Distance(currentTarget.position, desiredTarget.position);

        if (targetDistance > threshold && otherfoot.targetDistance > 1.2f)
        {
            currentTarget.position = desiredTarget.position;
        }

        //Distance entre la cible actuelle et la position du pied
        footMovement = Vector2.Distance(transform.position, currentTarget.position);

        //Si le pied est en train de bouger
        if (footMovement > 0.1f && isRunning)
        {
            //On augmente le timer pour la courbe d'animation
            stepTimer += Time.deltaTime;

            //On bouge le pied en ajoutant de la hauteur selon la courbe d'animation
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(currentTarget.position.x, currentTarget.position.y + yCurve.Evaluate(stepTimer)), speed * Time.deltaTime);
            //Debug.Log(stepTimer);

        }
        else
        {
            //Reset le timer
            stepTimer = 0;
            transform.position = currentTarget.position;
        }

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }

        if (!isRunning)
        {
            RaycastHit2D hit = Physics2D.Raycast(new Vector2(player.transform.position.x + legOffset * facingDirection, player.transform.position.y), -Vector2.up, 12f, LayerMask.GetMask("Ground"));
            if (hit.collider != null)
            {
                currentTarget.position = Vector3.Lerp(currentTarget.position, new Vector3(hit.point.x, hit.point.y, currentTarget.position.z), speed * Time.deltaTime);
            }
        }
    }

    /*
    private void OnDrawGizmos()
    {
        RaycastHit2D hit = Physics2D.Raycast(new Vector2(player.transform.position.x + neutralDisplacement, player.transform.position.y), -Vector2.up, 12f, LayerMask.GetMask("Ground"));
        if (hit.collider != null)
        {
           Gizmos.color = Color.green;
           Gizmos.DrawSphere(new Vector3(hit.point.x, hit.point.y, 0), 0.1f);
        }
    }
    */
}
