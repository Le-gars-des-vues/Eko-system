using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class MapManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static MapManager instance;

    [Header("Map Variables")]
    [SerializeField] Camera mapCamera;
    public float maxZoom = 15;
    [SerializeField] float minZoom = 5;
    public float maxDistanceFromOrigin;
    bool mapIsSelected;

    [Header("Beacon Variables")]
    [SerializeField] Animator beaconAnim;
    public List<GameObject> beacons = new List<GameObject>();
    public List<GameObject> buttons = new List<GameObject>();
    public bool beaconMenuOpen;
    public int activeBeaconsCount = 0;
    public GameObject activeBeacon;
    public Button teleportButton;
    public GameObject activeTeleporter;
    public bool isInTeleporterMenu;
    public TextMeshProUGUI teleportText;
    [SerializeField] GameObject noBeaconText;
    bool noBeacon = true;
    public Color activeColor;
    public Color unactiveColor;

    void Awake()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;
    }

    public void ResetMap()
    {
        mapCamera.orthographicSize = maxZoom;
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
                Vector2 direction = mapCamera.transform.position - Camera.main.transform.position;
                if (direction.magnitude > maxDistanceFromOrigin)
                {
                    direction = direction.normalized * maxDistanceFromOrigin;
                    mapCamera.transform.position = (Vector2)Camera.main.transform.position + direction;
                }
            }

            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0)
            {
                mapCamera.orthographicSize -= Input.mouseScrollDelta.y;
                mapCamera.orthographicSize = Mathf.Clamp(mapCamera.orthographicSize, minZoom, maxZoom);
            }
        }

        if (isInTeleporterMenu)
        {
            if (activeBeacon != null)
            {
                teleportButton.interactable = true;
                teleportText.color = activeColor;
            }
            else
            {
                teleportButton.interactable = false;
                teleportText.color = unactiveColor;
            }
        }

        if (beacons.Count > 0 && noBeacon)
        {
            noBeacon = false;
            noBeaconText.SetActive(false);
        }
        else if (beacons.Count <= 0 && !noBeacon)
        {
            noBeacon = true;
            noBeaconText.SetActive(false);
        }
    }

    public void OpenAndCloseBeaconMenu()
    {
        if (!beaconMenuOpen)
        {
            beaconMenuOpen = true;
            beaconAnim.SetBool("isOpen", true);
        }
        else
        {
            beaconMenuOpen = false;
            beaconAnim.SetBool("isOpen", false);
        }
    }

    public void MoveCameraToBeacon1()
    {
        activeBeacon = beacons[0];
        mapCamera.transform.position = new Vector3(beacons[0].transform.position.x, beacons[0].transform.position.y, mapCamera.transform.position.z);
    }

    public void MoveCameraToBeacon2()
    {
        activeBeacon = beacons[1];
        mapCamera.transform.position = new Vector3(beacons[1].transform.position.x, beacons[1].transform.position.y, mapCamera.transform.position.z);
    }

    public void MoveCameraToBeacon3()
    {
        activeBeacon = beacons[2];
        mapCamera.transform.position = new Vector3(beacons[2].transform.position.x, beacons[2].transform.position.y, mapCamera.transform.position.z);
    }

    public void MoveCameraToBeacon4()
    {
        activeBeacon = beacons[3];
        mapCamera.transform.position = new Vector3(beacons[3].transform.position.x, beacons[3].transform.position.y, mapCamera.transform.position.z);
    }

    public void MoveCameraToBeacon5()
    {
        activeBeacon = beacons[4];
        mapCamera.transform.position = new Vector3(beacons[4].transform.position.x, beacons[4].transform.position.y, mapCamera.transform.position.z);
    }

    public void MoveCameraToBeacon6()
    {
        activeBeacon = beacons[5];
        mapCamera.transform.position = new Vector3(beacons[5].transform.position.x, beacons[5].transform.position.y, mapCamera.transform.position.z);
    }

    public void Teleport()
    {
        if (activeTeleporter.GetComponent<Teleporter>().isPoweredUp)
        {
            GameManager.instance.player.ShowOrHideMap();
            isInTeleporterMenu = false;
            Base.instance.Teleport(true, false, MapManager.instance.activeBeacon.transform.position);
            MapManager.instance.activeBeacon.GetComponent<Beacon>().DeactivateButton();
            Destroy(MapManager.instance.activeBeacon);
            activeTeleporter.GetComponent<Teleporter>().isPoweredUp = false;
            activeTeleporter = null;
            teleportButton.interactable = false;
            teleportText.color = unactiveColor;
        }
    }
}
