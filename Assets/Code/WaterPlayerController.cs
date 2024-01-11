using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class WaterPlayerController : MonoBehaviour
{
    [Header("GameObjects")]
    private Rigidbody2D rb;
    private GroundPlayerController groundController;

    [Header("Movement Variables")]
    Vector2 movement;
    public float uwSpeed = 2f;
    public float uwDashSpeed = 10f;
    public float swimDelay = 0.7f;
    private bool swimming = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        groundController = GetComponent<GroundPlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (Input.GetButtonDown("Jump"))
        {
            if (CanDash())
                StartCoroutine(UnderwaterDash());
        }
    }

    private void FixedUpdate()
    {
        rb.AddForce(movement * uwSpeed, ForceMode2D.Force);
    }

    IEnumerator UnderwaterDash()
    {
        swimming = true;
        rb.velocity = new Vector2(rb.velocity.x / 2, rb.velocity.y / 2);
        rb.AddForce(movement * uwDashSpeed, ForceMode2D.Impulse);
        yield return new WaitForSeconds(swimDelay);
        swimming = false;
    }

    private bool CanDash()
    {
        return swimming == false;
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
