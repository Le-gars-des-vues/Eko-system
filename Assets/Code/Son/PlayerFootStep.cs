using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStep : MonoBehaviour
{
    [SerializeField] PlayerPermanent player;

    public AK.Wwise.Event footstepSound;
    public AK.Wwise.Switch baseFootstep;
    public AK.Wwise.Switch groundFootstep;
    bool isInBase;

    private void Start()
    {
        isInBase = player.spawnAtBase;
    }

    private void Update()
    {
        if (player.isInBase && !isInBase)
        {
            isInBase = true;
            baseFootstep.SetValue(this.gameObject);
        }
        else if (!player.isInBase && isInBase)
        {
            isInBase = false;
            groundFootstep.SetValue(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            Debug.Log("Footstep");
            //GetComponent<TriggerOnFootstep>().Footstep();
            footstepSound.Post(gameObject);
        }
    }
}
