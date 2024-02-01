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
        if (player.enabled)
        {
            float facingDirection = player.GetComponent<PlayerPermanent>().isFacingRight ? 1 : -1;

            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && !isRunning)
            {
                isRunning = true;
                float leanAngle = transform.eulerAngles.z - leanFactor * facingDirection;
                StartCoroutine(RotateTorso(leanAngle));
            }
            else if (IsNotMoving() && isRunning)
            {
                isRunning = false;
                float neutralAngle = transform.eulerAngles.z + leanFactor * facingDirection;
                StartCoroutine(RotateTorso(neutralAngle));
                transform.position = torsoNeutralPos.position;
            }

            if (isRunning)
            {
                float torsoHeight = torsoNeutralPos.position.y + yCurve.Evaluate((Time.time % yCurve.length));
                transform.position = new Vector2(transform.position.x, torsoHeight);
            }
        }
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
        yield return null;
    }
}
