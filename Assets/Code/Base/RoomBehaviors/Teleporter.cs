using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Teleporter : MonoBehaviour
{
    [SerializeField] GameObject room;

    [Header("Portal Variables")]
    public bool isPoweredUp;
    [SerializeField] VisualEffect portal;
    [SerializeField] SpriteRenderer portalBackground;
    bool isInRange;

    [Header("Anim Variables")]
    private float desiredAlpha = 1;
    private float currentAlpha = 1;
    [SerializeField] float fadeSpeed = 1;

    private void OnEnable()
    {
        GameManager.instance.teleporter.Add(this);
        if (GameManager.instance.teleporter.Count > 0)
            QuickMenu.instance.UnlockTeleporter(true);

        room = gameObject.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        currentAlpha = Mathf.MoveTowards(currentAlpha, desiredAlpha, fadeSpeed * Time.deltaTime);
        portalBackground.color = portalBackground.color = new Color(portalBackground.color.r, portalBackground.color.g, portalBackground.color.b, currentAlpha);

        if (portal.HasAnySystemAwake() && !isPoweredUp)
        {
            portal.Stop();
            desiredAlpha = 0f;
        }

        else if (!portal.HasAnySystemAwake() && isPoweredUp)
        {
            portal.Play();
            desiredAlpha = 1f;
        }

        if (room.GetComponent<RoomInfo>().isRefunded)
        {
            GameManager.instance.teleporter.Remove(this);
            if (GameManager.instance.teleporter.Count <= 0)
                QuickMenu.instance.UnlockTeleporter(false);
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (!GameManager.instance.player.mapIsOpen)
            {
                if (isInRange && ArrowManager.instance.targetObject == gameObject)
                {
                    GameManager.instance.player.ShowOrHideMap();
                    if (!MapManager.instance.beaconMenuOpen)
                        MapManager.instance.OpenAndCloseBeaconMenu();
                    MapManager.instance.isInTeleporterMenu = true;
                    MapManager.instance.activeTeleporter = this.gameObject;
                }
            }
            else
            {
                GameManager.instance.player.ShowOrHideMap();
                MapManager.instance.isInTeleporterMenu = false;
                MapManager.instance.teleportButton.interactable = false;
                MapManager.instance.teleportText.color = MapManager.instance.unactiveColor;
                MapManager.instance.activeTeleporter = null;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (ArrowManager.instance.targetObject != gameObject)
                ArrowManager.instance.PlaceArrow(transform.position, "TELEPORT", new Vector2(0, 1), gameObject);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = true;
            if (ArrowManager.instance.targetObject != gameObject)
                ArrowManager.instance.PlaceArrow(transform.position, "TELEPORT", new Vector2(0, 1), gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            isInRange = false;
            if (ArrowManager.instance.targetObject == gameObject)
                ArrowManager.instance.RemoveArrow();
        }
    }
}
