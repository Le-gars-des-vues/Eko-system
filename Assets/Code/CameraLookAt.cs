using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraLookAt : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform player;
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private float xThreshold;
    [SerializeField] private float yThreshold;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            vCam.Follow = gameObject.transform;

        if (Input.GetKey(KeyCode.Tab))
            AimLogic();

        if (Input.GetKeyUp(KeyCode.Tab))
            vCam.Follow = player;
    }

    private void AimLogic()
    {
        Vector3 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);
        Vector3 targetPos = (player.position + mousePos) / 2;

        targetPos.x = Mathf.Clamp(targetPos.x, -xThreshold + player.position.x, xThreshold + player.position.x);
        targetPos.y = Mathf.Clamp(targetPos.y, -yThreshold + player.position.y, yThreshold + player.position.y);

        this.transform.position = targetPos;
    }
}
