using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiTool : MonoBehaviour
{
    PlayerPermanent player;
    SpriteRenderer sprite;

    [SerializeField] LineRenderer laser;
    [SerializeField] Transform firePoint;

    [SerializeField] GameObject startVFX;
    [SerializeField] GameObject endVFX;
    private List<ParticleSystem> particles = new List<ParticleSystem>();

    public AK.Wwise.Event multitoolCharge;
    uint playingSoundID;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerPermanent>();
        sprite = GetComponent<SpriteRenderer>();
        sprite.enabled = false;
        FillList();
        DisableLaser();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.isHarvesting)
            UpdateLaser();
    }

    public void UseMultiTool(bool isUsing)
    {
        if (isUsing)
        {
            player.isUsingMultiTool = true;
            sprite.enabled = true;
        }
        else
        {
            player.isUsingMultiTool = false;
            sprite.enabled = false;
        }
    }

    public void EnableLaser()
    {
        laser.enabled = true;
        playingSoundID = multitoolCharge.Post(gameObject);
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Play();
        }
    }

    void UpdateLaser()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        laser.SetPosition(0, firePoint.position);
        laser.SetPosition(1, mousePos);

        startVFX.transform.position = (Vector2)firePoint.position;

        Vector2 direction = mousePos - (Vector2)transform.position;
        /*
        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, direction.normalized, direction.magnitude);
        if (hit)
        {
            laser.SetPosition(1, hit.point);
        }
        */

        endVFX.transform.position = laser.GetPosition(1);
    }

    public void DisableLaser()
    {
        laser.enabled = false;
        AkSoundEngine.StopPlayingID(playingSoundID);
        for (int i = 0; i < particles.Count; i++)
        {
            particles[i].Stop();
        }
    }

    void FillList()
    {
        for (int i = 0; i < startVFX.transform.childCount; i++)
        {
            var ps = startVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
                particles.Add(ps);
        }

        for (int i = 0; i < endVFX.transform.childCount; i++)
        {
            var ps = endVFX.transform.GetChild(i).GetComponent<ParticleSystem>();
            if (ps != null)
                particles.Add(ps);
        }
    }
}
