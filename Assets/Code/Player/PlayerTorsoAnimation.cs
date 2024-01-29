using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTorsoAnimation : MonoBehaviour
{
    public bool isRunning;
    [SerializeField] private AnimationCurve yCurve;
    [SerializeField] private Transform torsoNeutralPos;
    [SerializeField] private float leanFactor;
    [SerializeField] private float leanSpeed;
    [SerializeField] private GroundPlayerController player;

    private void Start()
    {
        torsoNeutralPos.position = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float facingDirection = player.GetComponent<GroundPlayerController>().isFacingRight ? 1 : -1;
        //leanAngle = player.GetComponent<GroundPlayerController>().isFacingRight ? -15f : 15f;

        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !isRunning)
        {
            isRunning = true;
            float leanAngle = transform.eulerAngles.z - leanFactor * facingDirection;
            StartCoroutine(RotateTorso(leanAngle));
            //transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z - (leanAngle * facingDirection));
        }
        else if (IsNotMoving() && isRunning)
        {
            isRunning = false;
            float neutralAngle = transform.eulerAngles.z + leanFactor * facingDirection;
            StartCoroutine(RotateTorso(neutralAngle));
            //transform.eulerAngles = new Vector3(0, 0, transform.eulerAngles.z + (leanAngle * facingDirection));
            transform.position = torsoNeutralPos.position;
        }

        if (isRunning)
        {
            //transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, leanAngle), leanSpeed * Time.deltaTime);
            float torsoHeight = torsoNeutralPos.position.y + yCurve.Evaluate((Time.time % yCurve.length));
            transform.position = new Vector3(transform.position.x, torsoHeight, transform.position.z);
        }
        /*
        else
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, neutralAngle), leanSpeed * Time.deltaTime);
        */
    }

    bool IsNotMoving()
    {
        return !Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D);
    }

    IEnumerator RotateTorso(float angle)
    {
        for (float t = 0f; t < 5f; t += Time.deltaTime)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, angle), Time.deltaTime * leanSpeed);
        }
        //transform.eulerAngles = new Vector3(0, 0, angle);
        yield return null;
    }
}
