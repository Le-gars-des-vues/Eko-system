using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class Wander : BehaviorNode
{
    private Transform creature;
    private BoxCollider2D territory;
    private bool isWaiting = true;
    private int waitTime = 2;
    private float waitTimer;

    private bool isMoving;
    private int maxMovingDistance;
    private Transform targetPos;

    int minWaitTime;
    int maxWaitTime;

    public Wander(Transform _creature, Transform _targetPos, BoxCollider2D _territory, int _maxMovingDistance, int _minWaitTime = 2, int _maxWaitTime = 10)
    {
        creature = _creature;
        targetPos = _targetPos;
        territory = _territory;
        maxMovingDistance = _maxMovingDistance;
        minWaitTime = _minWaitTime;
        maxWaitTime = _maxWaitTime;
    }

    public override NodeState Evaluate()
    {
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTime = Random.Range(minWaitTime, maxWaitTime);
            }
        }
        else
        {
            if (GetData("isFlying") != null && !(bool)GetData("isFlying"))
            {
                if (!isMoving)
                {
                    Vector2 tempPos = new Vector2(creature.position.x + Random.Range(-maxMovingDistance, maxMovingDistance), creature.position.y);
                    if (territory.bounds.Contains(tempPos))
                    {
                        if ((bool)GetData("debug"))
                            Debug.Log("Target is in bounds!");
                        bool unWalkable = Physics2D.OverlapCircle(tempPos, 0.1f, LayerMask.GetMask("Ground"));
                        bool isWalkable = Physics2D.Raycast(tempPos, Vector2.down, 3f, LayerMask.GetMask("Ground"));
                        if (!unWalkable && isWalkable)
                        {
                            if ((bool)GetData("debug"))
                                Debug.Log("Found target for path!");
                            //Debug.Log("Found target for path!");
                            targetPos.position = tempPos;
                            parent.SetData("target", targetPos);
                            parent.SetData("pathTarget", "wandering");
                            parent.SetData("pathState", 1);
                            isMoving = true;
                        }
                        else
                        {
                            waitTime = 0;
                            waitTimer = 0;
                            isWaiting = true;
                        }
                    }
                    else
                    {
                        if ((bool)GetData("debug"))
                            Debug.Log("Target is outside of bounds!");
                        waitTime = 1;
                        waitTimer = 0;
                        isWaiting = true;
                    }
                }
                else
                {
                    if (GetData("pathState") != null && (int)GetData("pathState") == 0)
                    {
                        if ((bool)GetData("debug"))
                            Debug.Log("Finished Wandering");
                        isWaiting = true;
                        isMoving = false;
                        waitTimer = 0;
                    }
                }
            }
            else
            {
                if (!isMoving)
                {
                    Vector2 tempPos = new Vector2(creature.position.x + Random.Range(-maxMovingDistance, maxMovingDistance), creature.position.y + Random.Range(-maxMovingDistance, maxMovingDistance));
                    if (territory.bounds.Contains(tempPos))
                    {
                        if ((bool)GetData("debug"))
                            Debug.Log("Target is in bounds!");
                        bool isWalkable = !Physics2D.OverlapCircle(tempPos, 0.1f, LayerMask.GetMask("Ground"));
                        if (isWalkable)
                        {
                            if ((bool)GetData("debug"))
                                Debug.Log("Found target for path!");
                            targetPos.position = tempPos;
                            parent.SetData("target", targetPos);
                            parent.SetData("pathTarget", "wandering");
                            parent.SetData("pathState", 1);
                            isMoving = true;
                        }
                        else
                        {
                            waitTime = 0;
                            waitTimer = 0;
                            isWaiting = true;
                        }
                    }
                    else
                    {
                        if ((bool)GetData("debug"))
                            Debug.Log("Target is outside of bounds!");
                        waitTime = 1;
                        waitTimer = 0;
                        isWaiting = true;
                    }
                }
                else
                {
                    if (GetData("pathState") != null && (int)GetData("pathState") == 0)
                    {
                        if ((bool)GetData("debug"))
                            Debug.Log("Finished Wandering");
                        isWaiting = true;
                        isMoving = false;
                        waitTimer = 0;
                    }
                }
            }
        }
        state = NodeState.RUNNING;
        return state;
    }
}
