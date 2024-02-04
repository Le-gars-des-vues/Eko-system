using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTorsoAnimation : MonoBehaviour
{
    private bool isRunning;
    [SerializeField] private AnimationCurve yCurve;
    [SerializeField] private Transform torsoNeutralPos;
    [SerializeField] private float leanFactor;
    //[SerializeField] private float leanAngle;
    [SerializeField] private GroundPlayerController player;
    [SerializeField] private float duration;
    public float timer;

    private void Start()
    {
        torsoNeutralPos.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.enabled)
        {
            float facingDirection = player.GetComponent<PlayerPermanent>().isFacingRight ? 1 : -1;
            bool isMovingRight = Input.GetAxis("Horizontal") >= 0 ? true : false;
            //float walkAngle = player.GetComponent<PlayerPermanent>().isFacingRight ? 75 : 105

            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !isRunning)
            {
                isRunning = true;
                timer = 0;
                //leanAngle = transform.eulerAngles.z - leanFactor * facingDirection;
                //StartCoroutine(RotateTorso(leanAngle));
            }
            else if (IsNotMoving() && isRunning)
            {
                isRunning = false;
                timer = 0;
                //float neutralAngle = transform.eulerAngles.z + leanFactor * facingDirection;
                //StartCoroutine(RotateTorso(neutralAngle));
                transform.position = torsoNeutralPos.position;
            }
            

            if (isRunning)
            {
                float torsoHeight = torsoNeutralPos.position.y + yCurve.Evaluate((Time.time % yCurve.length));
                transform.position = new Vector2(transform.position.x, torsoHeight);
                timer += Time.deltaTime;
                if (isMovingRight == player.GetComponent<PlayerPermanent>().isFacingRight)
                {
                    float angle = Mathf.LerpAngle(transform.eulerAngles.z, 75 * facingDirection, timer / duration);
                    transform.eulerAngles = new Vector3(0, 0, angle);
                }
                else
                {
                    float angle = Mathf.LerpAngle(transform.eulerAngles.z, 105 * facingDirection, timer / duration);
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
    }

    bool IsNotMoving()
    {
        return !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D);
    }

    IEnumerator RotateTorso(float angle)
    {
        timer = 0;
        while (timer < duration)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, 75), timer / duration);
            yield return null;
        }
        transform.eulerAngles = new Vector3(0, 0, 75);
        yield return null;
    }
}
