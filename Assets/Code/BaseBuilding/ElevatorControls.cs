using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElevatorControls : MonoBehaviour
{
    private bool inElevator=false;
    public float elevatorSpeed;

    public float max;
    public float min;

    [SerializeField] float upRaycastLength;
    [SerializeField] float downRaycastLength;
    [SerializeField] Vector2 offset;

    private void FixedUpdate()
    {
        if (inElevator && transform.parent.gameObject.GetComponent<RoomInfo>().roomUnder != null)
        {
            if (Input.GetKey(KeyCode.S))
            {
                RaycastHit2D downCheck = Physics2D.Raycast((Vector2)transform.position - offset, Vector2.down, downRaycastLength, LayerMask.GetMask("Ground", "Default"));
                if (!downCheck)
                    this.gameObject.GetComponent<Transform>().Translate(0, -elevatorSpeed, 0);
            } 
            else if (Input.GetKey(KeyCode.W))
            {
                RaycastHit2D upCheck = Physics2D.Raycast((Vector2)transform.position + offset, Vector2.up, upRaycastLength, LayerMask.GetMask("Ground", "Default"));
                if (!upCheck)
                    this.gameObject.GetComponent<Transform>().Translate(0, elevatorSpeed, 0);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.parent.SetParent(this.gameObject.transform);
            inElevator = true;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            collision.transform.parent.SetParent(null);
            inElevator = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay((Vector2)transform.position + offset, Vector2.up * upRaycastLength);
        Gizmos.DrawRay((Vector2)transform.position - offset, Vector2.down * downRaycastLength);
    }
}
