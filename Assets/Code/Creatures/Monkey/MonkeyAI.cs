using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonkeyAI : MonoBehaviour
{
    private CreaturePathfinding pathfinding;
    private MonkeyMovement monkey;
    public bool followPath;

    private void OnEnable()
    {
        pathfinding = GetComponent<CreaturePathfinding>();
        monkey = GetComponent<MonkeyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (followPath)
        {
            followPath = false;
            pathfinding.NewTarget(GameObject.FindGameObjectWithTag("Player"));
        }
    }
}