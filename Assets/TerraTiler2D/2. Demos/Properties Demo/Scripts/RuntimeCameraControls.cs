using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class RuntimeCameraControls : MonoBehaviour
{
    private Camera myCamera;

    private Vector3 dragStartPosition;
    private Vector3 cameraDragStartPosition;

    private float dragSpeed = 0.00107f;
    private int zoomLevel = 45;

    private int maxZoomLevel = 150;
    private float maxZoomLevelOrthographic;

    // Start is called before the first frame update
    void Start()
    {
        myCamera = GetComponent<Camera>();

        maxZoomLevelOrthographic = (27f / (float)zoomLevel) * (float)maxZoomLevel;
    }

    // Update is called once per frame
    void Update()
    {
        handleMouse();

        handleScroll();
    }

    private void handleMouse()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = Input.mousePosition;
            cameraDragStartPosition = transform.position;
        }
        else if (Input.GetMouseButton(0))
        {
            transform.position = cameraDragStartPosition - ((Input.mousePosition - dragStartPosition) * (dragSpeed * zoomLevel));
        }
    }

    private void handleScroll()
    {
        if (Input.mouseScrollDelta.magnitude > 0)
        {
            zoomLevel -= (int)Input.mouseScrollDelta.y;

            zoomLevel = Mathf.Clamp(zoomLevel, 1, maxZoomLevel);

            if (myCamera.orthographic)
            {
                myCamera.orthographicSize = Mathf.Lerp(0f, maxZoomLevelOrthographic, (float)zoomLevel / (float)maxZoomLevel);
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, -zoomLevel);
            }
        }
    }
}
