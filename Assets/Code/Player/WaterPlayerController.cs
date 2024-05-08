using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;
using System.SceneManagement;

public class WaterPlayerController : MonoBehaviour
{
    [Header("GameObjects")]
    private Rigidbody2D rb;
    private PlayerPermanent player;

    [Header("Movement Variables")]
    Vector2 movement;
    public float uwSpeed = 2f;
    public float uwDashSpeed = 10f;
    public float swimDelay = 0.7f;
    public float swimStaminaCost;
    public float dashStaminaCost;
    private bool dashing = false;
    public bool isSwimming;

    private void Awake()
    {
        SceneLoader.allScenesLoaded += StartScript;
    }

    // Start is called before the first frame update
    void StartScript()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.CanMove())
        {
            movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (Mathf.Abs(movement.x) > 0.1f || Mathf.Abs(movement.y) > 0.1f)
            {
                if (player.objectInRightHand == null || (player.objectInRightHand.tag != "TwoHandedWeapon" && player.objectInRightHand.tag != "Spear"))
                    isSwimming = true;
                //player.ChangeStamina(-swimStaminaCost * Time.deltaTime);
                if (!player.colliderShapeIsChanged)
                {
                    player.ChangeColliderShape(true);
                    //rb.mass = 0.7f;
                }
            }
            else
            {
                isSwimming = false;
                if (player.colliderShapeIsChanged)
                {
                    player.ChangeColliderShape(false);
                    //rb.mass = 1f;
                }
            }
            if (Input.GetButtonDown("Jump"))
            {
                if (CanDash())
                {
                    if (!player.staminaDepleted)
                    {
                        StartCoroutine(UnderwaterDash());
                    }
                    else
                        AudioManager.instance.PlaySound(AudioManager.instance.noStamina, gameObject);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(movement * uwSpeed, ForceMode2D.Force);
    }

    IEnumerator UnderwaterDash()
    {
        dashing = true;
        AudioManager.instance.PlaySound(AudioManager.instance.nage, gameObject);
        rb.velocity = new Vector2(rb.velocity.x / 2, rb.velocity.y / 2);
        rb.AddForce(movement * uwDashSpeed, ForceMode2D.Impulse);
        player.ChangeStamina(-dashStaminaCost);
        yield return new WaitForSeconds(swimDelay);
        dashing = false;
    }

    private bool CanDash()
    {
        return dashing == false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (SceneLoader.instance.isLoading) return;

        if (collision.gameObject.tag == "Water")
        {
            if (player.colliderShapeIsChanged)
            {
                player.ChangeColliderShape(false);
            }
            rb.mass = 1f;
            player.groundPlayerController.enabled = true;
            player.waterPlayerController.enabled = false;

            isSwimming = false;
            if (player.isUnderwater)
                player.GoUnderwater(false);
        }
    }
}
