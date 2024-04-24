using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    [SerializeField] private float detectionRaycastRange;
    [SerializeField] private float spikeRaycastRange;
    [SerializeField] private float spikeUpTime;
    [SerializeField] private float spikeCooldown;
    [SerializeField] private float spikeUpHeight;
    [SerializeField] private float spikeGracePeriod;
    [SerializeField] private float damage;

    private bool isSpiking;
    private bool hasSpiked;
    private float timer;

    // Update is called once per frame
    void Update()
    {

        RaycastHit2D detected = Physics2D.Raycast(transform.position, Vector2.up, detectionRaycastRange, LayerMask.GetMask("Player", "Creature"));
        if (detected.collider != null && detected.collider.gameObject.tag == "Player" && !isSpiking)
        {
            isSpiking = true;
            Vector2 pos = new Vector2(transform.position.x, transform.position.y + spikeUpHeight);
            StartCoroutine(Spiking(pos));
        }

        if (isSpiking)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, spikeRaycastRange, LayerMask.GetMask("Player", "Creature"));
            if (hit.collider != null && !hasSpiked)
            {
                if (hit.collider.gameObject.tag == "Player")
                {
                    hasSpiked = true;
                    hit.collider.gameObject.GetComponent<PlayerPermanent>().ChangeHp(-damage, true, gameObject);
                }
            }
        }
        else
            hasSpiked = false;
    }

    IEnumerator Spiking(Vector2 posUp)
    {
        yield return new WaitForSeconds(spikeGracePeriod);
        timer = 0;
        var startPos = transform.position;
        AudioManager.instance.PlaySound(AudioManager.instance.spikeTrap, gameObject);
        while (timer < spikeUpTime)
        {
            timer += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, posUp, timer / spikeUpTime);
            yield return null;
        }
        yield return new WaitForSeconds(2f);
        Vector2 pos = new Vector2(transform.position.x, transform.position.y - spikeUpHeight);
        StartCoroutine(UnSpiking(pos));
    }

    IEnumerator UnSpiking(Vector2 posDown)
    {
        timer = 0;
        var startPos = transform.position;
        hasSpiked = false;
        while (timer < spikeCooldown)
        {
            timer += Time.deltaTime;
            transform.position = Vector2.Lerp(startPos, posDown, timer / spikeCooldown);
            yield return null;
        }
        isSpiking = false;
        yield return null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position,transform.position + Vector3.up * detectionRaycastRange);
        Gizmos.DrawLine(transform.position, transform.position + Vector3.up * spikeRaycastRange);
        Gizmos.DrawSphere(new Vector2(transform.position.x, transform.position.y + spikeUpHeight), 0.1f);
    }
}
