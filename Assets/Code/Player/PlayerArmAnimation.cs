using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmAnimation : MonoBehaviour
{
    [SerializeField] private GameObject player;
    [SerializeField] private PlayerArmAnimation otherArm;

    //Position initiale des bras
    [SerializeField] private float armXOffset;
    [SerializeField] private float armYOffset;

    //Vitesse de mouvement des bras
    [SerializeField] private float speed;

    //Cible pour les bras
    [SerializeField] private Transform armTarget;

    //Courbe d'animation pour la position en Y des bras
    [SerializeField] private AnimationCurve yCurve;

    //Offset pour les points ou la cible va s'arreter
    [HideInInspector] public float fXOffset;
    [HideInInspector] public float fYOffset;
    [HideInInspector] public float bXOffset;
    [HideInInspector] public float bYOffset;

    //Vector de position
    private Vector3 forwardArmPos;
    private Vector3 backwardArmPos;

    public bool goingForward;
    public bool isRunning;
    private bool reachedPos = false;
    private float targetDistance;
    private float stepTimer;

    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(player.transform.position.x + armXOffset, player.transform.position.y + armYOffset, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        targetDistance = Mathf.Abs(Vector2.Distance(transform.position, armTarget.position));
        stepTimer += Time.deltaTime;

        float facingDirection = player.GetComponent<GroundPlayerController>().isFacingRight ? 1 : -1;

        forwardArmPos = new Vector3(player.transform.position.x + fXOffset * facingDirection, player.transform.position.y + fYOffset, transform.position.z);
        backwardArmPos = new Vector3(player.transform.position.x + bXOffset * facingDirection, player.transform.position.y + bYOffset, transform.position.z);

        armTarget.position = goingForward ? forwardArmPos : backwardArmPos;

        if (targetDistance <= 0.01f && stepTimer > 0.2f)
            reachedPos = true;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
            reachedPos = false;
            stepTimer = 0;
        }

        if (isRunning)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector3(armTarget.position.x, armTarget.position.y + yCurve.Evaluate(stepTimer), transform.position.z), speed * Time.deltaTime);
            if (reachedPos == true && otherArm.targetDistance < 0.01f)
            {
                reachedPos = false;
                goingForward = !goingForward;
                //Debug.Log(stepTimer);
                stepTimer = 0;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(player.transform.position.x + armXOffset * facingDirection, player.transform.position.y + armYOffset, transform.position.z), speed * Time.deltaTime);
            if (otherArm.goingForward == true)
                goingForward = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(new Vector3(player.transform.position.x + armXOffset, player.transform.position.y + armYOffset, transform.position.z), 0.05f);
        Gizmos.DrawSphere(forwardArmPos, 0.05f);
        Gizmos.DrawSphere(backwardArmPos, 0.05f);
    }
}
