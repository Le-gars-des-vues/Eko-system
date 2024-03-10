using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class Wander : BehaviorNode
{
    private Transform creature;
    private BoxCollider2D territory;
    private bool isGrounded;
    private bool isWaiting = true;
    private int waitTime = 2;
    private float waitTimer;

    private bool isMoving;
    private int maxMovingDistance;
    private Transform targetPos;

    public Wander(Transform _creature, Transform _targetPos, bool _isGrounded, BoxCollider2D _territory, int _maxMovingDistance)
    {
        creature = _creature;
        targetPos = _targetPos;
        isGrounded = _isGrounded;
        territory = _territory;
        maxMovingDistance = _maxMovingDistance;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("wandering");
        if (isWaiting)
        {
            waitTimer += Time.deltaTime;
            if (waitTimer >= waitTime)
            {
                isWaiting = false;
                waitTime = Random.Range(2, 10);
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
                        targetPos.position = tempPos;
                        bool isWalkable = !Physics2D.OverlapCircle(targetPos.position, 0.1f, LayerMask.GetMask("Ground"));
                        if (isWalkable)
                            isMoving = true;
                        else
                        {
                            waitTime = 0;
                            waitTimer = 0;
                            isWaiting = true;
                        }
                    }
                    else
                    {
                        waitTime = 1;
                        waitTimer = 0;
                        isWaiting = true;
                    }
                }
                else
                {
                    if (Mathf.Abs(creature.position.x - targetPos.position.x) < 1f)
                    {
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
                        //Debug.Log("Target is in bounds!");
                        targetPos.position = tempPos;
                        bool isWalkable = !Physics2D.OverlapCircle(targetPos.position, 0.4f, LayerMask.GetMask("Ground"));
                        if (isWalkable)
                        {
                            //Debug.Log("Found target for path!");
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
                        //Debug.Log("Target is outside of bounds!");
                        waitTime = 1;
                        waitTimer = 0;
                        isWaiting = true;
                    }
                }
                else
                {
                    if (GetData("pathState") != null && (int)GetData("pathState") == 0)
                    {
                        //Debug.Log("Finished Wandering");
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
