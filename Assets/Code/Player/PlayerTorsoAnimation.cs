using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTorsoAnimation : MonoBehaviour
{
    private bool isRunning;
    [SerializeField] private AnimationCurve yCurve;
    [SerializeField] private Transform torsoNeutralPos;
    [SerializeField] private float leanFactor;
    private float leanForwardAngle;
    private float leanBackwardAngle;

    [Header("Underwater Variables")]
    private float fbInitialAngle;
    private bool isSwimming;

    //[SerializeField] private float leanAngle;
    [SerializeField] private PlayerPermanent player;
    [SerializeField] private float duration;
    public float timer;

    private void Start()
    {
        torsoNeutralPos.position = transform.position;
        leanForwardAngle = 90 - leanFactor;
        leanBackwardAngle = 90 + leanFactor;

        fbInitialAngle = transform.eulerAngles.z;
    }

    // Update is called once per frame
    void Update()
    {
        float facingDirection = player.isFacingRight ? 1 : -1;
        bool isMovingRight = Input.GetAxis("Horizontal") >= 0 ? true : false;
        //float walkAngle = player.GetComponent<PlayerPermanent>().isFacingRight ? 75 : 105

        if (player.CanMove())
        {
            if (player.gameObject.GetComponent<GroundPlayerController>().enabled)
            {
                if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !isRunning)
                {
                    isRunning = true;
                    timer = 0;
                }
                else if (IsNotMoving() && isRunning)
                {
                    isRunning = false;
                    timer = 0;
                    transform.position = torsoNeutralPos.position;
                }

                if (isRunning)
                {
                    float torsoHeight = torsoNeutralPos.position.y + yCurve.Evaluate((Time.time % yCurve.length));
                    transform.position = new Vector2(transform.position.x, torsoHeight);
                    timer += Time.deltaTime;
                    if (isMovingRight == player.isFacingRight)
                    {
                        float angle = Mathf.LerpAngle(transform.eulerAngles.z, leanForwardAngle * facingDirection, timer / duration);
                        transform.eulerAngles = new Vector3(0, 0, angle);
                    }
                    else
                    {
                        float angle = Mathf.LerpAngle(transform.eulerAngles.z, leanBackwardAngle * facingDirection, timer / duration);
                        transform.eulerAngles = new Vector3(0, 0, angle);
                    }
                }
                else
                {
                    timer += Time.deltaTime;
                    float angle = Mathf.LerpAngle(transform.eulerAngles.z, 90 * facingDirection, timer / duration);
                    transform.eulerAngles = new Vector3(0, 0, angle);
                }
            }
            else if (player.gameObject.GetComponent<WaterPlayerController>().enabled)
            {
                if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) && !isSwimming)
                {
                    isSwimming = true;
                    timer = 0;
                }
                else if (IsNotMoving() && isSwimming)
                {
                    isSwimming = false;
                    timer = 0;
                }

                if (isSwimming)
                {
                    timer += Time.deltaTime;
                    Vector2 direction = new Vector2(transform.position.x + Input.GetAxis("Horizontal"), transform.position.y + Input.GetAxis("Vertical"));
                    transform.right = Vector2.Lerp(transform.right, ((direction - (Vector2)transform.position).normalized * facingDirection), 2 * Time.deltaTime);
                }
                else
                {
                    timer += Time.deltaTime;
                    float angle1 = Mathf.LerpAngle(transform.eulerAngles.z, fbInitialAngle * facingDirection, timer / duration);
                    transform.eulerAngles = new Vector3(0, 0, angle1);
                }
            }
        }
        else
        {
            timer += Time.deltaTime;
            float angle = Mathf.LerpAngle(transform.eulerAngles.z, 90 * facingDirection, timer / duration);
            transform.eulerAngles = new Vector3(0, 0, angle);
        }
    }

    bool IsNotMoving()
    {
        return !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S);
    }
}
