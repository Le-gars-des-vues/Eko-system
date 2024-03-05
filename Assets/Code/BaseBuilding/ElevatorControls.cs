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

    private void FixedUpdate()
    {
        if (inElevator)
        {
            if (Input.GetKey(KeyCode.S))
            {
                
                    this.gameObject.GetComponent<Transform>().Translate(0, -elevatorSpeed, 0);
                
                
            } else if (Input.GetKey(KeyCode.W))
            {
                
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
}
