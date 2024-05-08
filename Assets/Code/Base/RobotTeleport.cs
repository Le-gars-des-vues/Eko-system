using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotTeleport : MonoBehaviour
{
    [SerializeField] Robot robot;
    [SerializeField] int index;
    bool isActive = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && isActive)
        {
            robot.teleportIndex = index;
            isActive = false;
        }
    }
}
