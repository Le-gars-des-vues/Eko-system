using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class CheckForTargetInRange : BehaviorNode
{
    private LayerMask layerMask = LayerMask.GetMask("Player", "Default");

    private Transform transform;
    private float senseOfSmell;
    private string foodName;
    int facingDirection;
    float fovRange;
    int originalDirection;
    bool isAgressive;

    Transform head;
    float startAngle;
    float angleStep;
    float sightAngle;
    float rayCount;
    float rayDistance;
    float minFollowDistance;

    public CheckForTargetInRange(Transform _transform, float _senseOfSmell, string _foodName, float _fovRange, Transform _head, float _startAngle, float _angleStep, float _sightAngle, float _rayCount, float _rayDistance, float _minFollowDistance, int _originalDirection, bool _isAgressive)
    {
        transform = _transform;
        senseOfSmell = _senseOfSmell;
        foodName = _foodName;
        fovRange = _fovRange;
        head = _head;
        startAngle = _startAngle;
        angleStep = _angleStep;
        sightAngle = _sightAngle;
        rayCount = _rayCount;
        rayDistance = _rayDistance;
        minFollowDistance = _minFollowDistance;
        originalDirection = _originalDirection;
        isAgressive = _isAgressive;
    }

    public override NodeState Evaluate()
    {
        object t = GetData("target");
        if  (t == null)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, senseOfSmell, layerMask);

            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    if (collider.gameObject.tag == "Player" && isAgressive)
                    {
                        //Debug.Log(collider.gameObject.transform.position);
                        if (Vector2.Distance(collider.gameObject.transform.position, transform.position) < fovRange)
                        {
                            //Debug.Log("Player is seen!");
                            parent.parent.SetData("target", collider.transform);
                            parent.parent.SetData("pathTarget", "player");
                            parent.parent.SetData("pathState", 1);
                            parent.parent.SetData("isAttacking", true);
                            state = NodeState.SUCCESS;
                            return state;
                        }
                        else
                        {
                            facingDirection = transform.localScale.x == 1 ? 1 : -1;
                            facingDirection *= originalDirection;
                            int targetIsRight = (collider.gameObject.transform.position.x - transform.position.x) > 0 ? 1 : -1;

                            if (targetIsRight == facingDirection)
                            {
                                if (Vision())
                                {
                                    Debug.Log("Player is seen!");
                                    parent.parent.SetData("target", collider.transform);
                                    parent.parent.SetData("pathTarget", "player");
                                    parent.parent.SetData("pathState", 1);
                                    parent.parent.SetData("isAttacking", true);
                                    state = NodeState.SUCCESS;
                                    return state;
                                }
                            }
                        }
                    }
                    else if (collider.gameObject.tag == "Plant")
                    {
                        if (collider.gameObject.GetComponent<PlantConsummable>().foodName == foodName)
                        {
                            if (GetData("isFull") != null && !(bool)GetData("isFull"))
                            {
                                parent.parent.SetData("target", collider.transform);
                                parent.parent.SetData("pathTarget", "food");
                                parent.parent.SetData("pathState", 1);
                                state = NodeState.SUCCESS;
                                return state;
                            }
                            //Debug.Log("Creature is full!");
                        }
                    }
                }
            }
            state = NodeState.FAILURE;
            return state;
        }
        else
        {
            if (GetData("pathTarget") != null && (string)GetData("pathTarget") == "player")
            {
                var pos = (Transform)GetData("target");
                if (Vector2.Distance(pos.position, transform.position) > minFollowDistance)
                {
                    //Debug.Log("player is too far");
                    parent.parent.ClearData("target");
                    parent.parent.ClearData("pathTarget");
                    parent.parent.SetData("pathState", 0);
                    if (GetData("isAttacking") != null && (bool)GetData("isAttacking") == true)
                        parent.parent.SetData("isAttacking", false);

                    state = NodeState.FAILURE;
                    return state;
                }
                else
                {
                    state = NodeState.SUCCESS;
                    return state;
                }
            }
            else
            {
                Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, senseOfSmell, layerMask);

                if (colliders.Length > 0)
                {
                    foreach (Collider2D collider in colliders)
                    {
                        if (collider.gameObject.tag == "Player" && isAgressive)
                        {
                            //Debug.Log(collider.gameObject.transform.position);
                            if (Vector2.Distance(collider.gameObject.transform.position, transform.position) < fovRange)
                            {
                                //Debug.Log("Player is seen!");
                                parent.parent.SetData("target", collider.transform);
                                parent.parent.SetData("pathTarget", "player");
                                parent.parent.SetData("pathState", 1);
                                parent.parent.SetData("isAttacking", true);
                                state = NodeState.SUCCESS;
                                return state;
                            }
                            else
                            {
                                facingDirection = transform.localScale.x == 1 ? 1 : -1;
                                facingDirection *= originalDirection;
                                int targetIsRight = (collider.gameObject.transform.position.x - transform.position.x) > 0 ? 1 : -1;

                                if (targetIsRight == facingDirection)
                                {
                                    if (Vision())
                                    {
                                        Debug.Log("Player is seen!");
                                        parent.parent.SetData("target", collider.transform);
                                        parent.parent.SetData("pathTarget", "player");
                                        parent.parent.SetData("pathState", 1);
                                        parent.parent.SetData("isAttacking", true);
                                        state = NodeState.SUCCESS;
                                        return state;
                                    }
                                }
                            }
                        }
                        else
                        {
                            state = NodeState.SUCCESS;
                            return state;
                        }
                    }
                }
                state = NodeState.SUCCESS;
                return state;
            }
        }
    }

    bool Vision()
    {
        angleStep = sightAngle / (rayCount - 1);

        // Start angle from left limit of sight angle
        startAngle = -sightAngle / 2;

        for (int i = 0; i < rayCount; i++)
        {
            // Calculate current angle
            float angle = startAngle + angleStep * i;
            // Calculate direction of the ray
            Vector3 direction = Quaternion.Euler(0, 0, angle) * head.transform.right * facingDirection;

            LayerMask layer = LayerMask.GetMask("Ground", "Player");

            // Cast a ray in the calculated direction
            RaycastHit2D hit = Physics2D.Raycast(head.transform.position, direction, rayDistance, layer);

            //Debug.DrawRay(head.transform.position, direction * rayDistance, Color.green);

            // Check if the ray hits a platform collider
            if (hit.collider != null && hit.collider.tag == "Player")
            {
                return true;
            }
        }
        //Debug.Log(targetPosition);
        return false;
    }
}
