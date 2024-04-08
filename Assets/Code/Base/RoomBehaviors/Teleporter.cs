using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Teleporter : MonoBehaviour
{
    [SerializeField] GameObject room;

    public bool isPoweredUp;
    [SerializeField] VisualEffect portal;
    [SerializeField] SpriteRenderer portalBackground;

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
    }
}
