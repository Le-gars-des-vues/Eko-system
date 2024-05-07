using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System.SceneManagement;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform player;
    [SerializeField] private Transform target;
    [SerializeField] private CinemachineVirtualCamera vCam;
    [SerializeField] private float xThreshold;
    [SerializeField] private float yThreshold;

    [SerializeField] private SpriteRenderer isoldatedView;
    public bool isIsoldated;
    private float desiredAlpha = 0;
    private float currentAlpha = 0;
    [SerializeField] float fadeSpeed;

    private void Awake()
    {
        SceneLoader.allScenesLoaded += StartScript;
    }

    private void StartScript()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        vCam.Follow = player;
    }

    private void Update()
    {
        if (SceneLoader.instance.isLoading) return;

        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, fadeSpeed * Time.deltaTime);
        isoldatedView.color = new Color(isoldatedView.color.r, isoldatedView.color.g, isoldatedView.color.b, currentAlpha);

        if (Input.GetKeyDown(KeyCode.Tab))
            vCam.Follow = target.transform;

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
        targetPos.z = 0;

        target.transform.position = targetPos;
    }

    public void IsolateCameraView(bool isTrue)
    {
        if (isTrue)
        {
            isIsoldated = true;
            desiredAlpha = 0.95f;
        }
        else
        {
            isIsoldated = false;
            desiredAlpha = 0f;
        }
    }
}
