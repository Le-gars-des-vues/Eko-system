using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static MapManager instance;

    [SerializeField] Camera mapCamera;
    bool mapIsSelected;

    float initOrthoSize;

    void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    void Start()
    {
        initOrthoSize = mapCamera.orthographicSize;
    }

    public void ResetMap()
    {
        mapCamera.orthographicSize = initOrthoSize;
        mapCamera.transform.position = Camera.main.transform.position;
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        mapIsSelected = true;
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        mapIsSelected = false;
    }

    private void Update()
    {
        if (mapIsSelected)
        {
            if (Input.GetMouseButton(0))
            {
                mapCamera.transform.position = new Vector3(mapCamera.transform.position.x - Input.GetAxis("Mouse X"), mapCamera.transform.position.y - Input.GetAxis("Mouse Y"), mapCamera.transform.position.z);

                //Vector3 pos = mapCamera.ScreenToViewportPoint(dragOrigin - Input.mousePosition);
                //Vector3 move = new Vector3(pos.x * dragSpeed, pos.y * dragSpeed, 0);
                //mapCamera.transform.Translate(move, Space.World);
            }

            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
            {
                mapCamera.orthographicSize -= Input.mouseScrollDelta.y;
            }
        }
    }
}
