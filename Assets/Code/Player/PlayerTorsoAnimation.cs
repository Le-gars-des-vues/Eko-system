using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTorsoAnimation : MonoBehaviour
{
    private bool isRunning;
    [SerializeField] private AnimationCurve yCurve;
    [SerializeField] private Transform torsoNeutralPos;
    [SerializeField] private float leanAngle;
    [SerializeField] private float neutralAngle = 90;
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

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D) && !isRunning)
        {
            isRunning = true;
            leanAngle = transform.eulerAngles.z - leanFactor * facingDirection;
        }
        else if (!(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && isRunning)
        {
            isRunning = false;
            neutralAngle = transform.eulerAngles.z + leanFactor * facingDirection;
            transform.position = torsoNeutralPos.position;
        }

        if (isRunning)
        {
            //StartCoroutine(RotateTorso(leanAngle));
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, leanAngle), leanSpeed * Time.deltaTime);
            float torsoHeight = torsoNeutralPos.position.y + yCurve.Evaluate((Time.time % yCurve.length));
            transform.position = new Vector3(transform.position.x, torsoHeight, transform.position.z);
        }
        else
            //StartCoroutine(RotateTorso(neutralAngle));
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, neutralAngle), leanSpeed * Time.deltaTime);
    }

    IEnumerator RotateTorso(float leanAngle)
    {
        while (transform.eulerAngles.z != leanAngle)
        {
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0, 0, leanAngle), leanSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
