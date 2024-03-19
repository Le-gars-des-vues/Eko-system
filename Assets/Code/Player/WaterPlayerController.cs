using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class WaterPlayerController : MonoBehaviour
{
    [Header("GameObjects")]
    private Rigidbody2D rb;
    private GroundPlayerController groundController;
    private PlayerPermanent player;

    [Header("Movement Variables")]
    Vector2 movement;
    public float uwSpeed = 2f;
    public float uwDashSpeed = 10f;
    public float swimDelay = 0.7f;
    public float swimStaminaCost;
    public float dashStaminaCost;
    private bool dashing = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundController = GetComponent<GroundPlayerController>();
        player = GetComponent<PlayerPermanent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.uiOpened)
        {
            movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            if (Mathf.Abs(movement.x) > 0.1f || Mathf.Abs(movement.y) > 0.1f)
                player.ChangeStamina(-swimStaminaCost * Time.deltaTime);
            if (Input.GetButtonDown("Jump"))
            {
                if (CanDash())
                    StartCoroutine(UnderwaterDash());
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
        if (collision.gameObject.tag == "Water")
        {
            GetComponent<GroundPlayerController>().enabled = true;
            groundController.SetGravityScale(groundController.gravityScale);
            rb.drag = 0f;
            GetComponent<WaterPlayerController>().enabled = false;
        }
    }
}
