using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class CheckForTargetInRange : BehaviorNode
{
    private LayerMask layerMask = LayerMask.GetMask("Player", "Pixelate");

    private Transform transform;
    private float senseOfSmell;
    private string foodName;
    int facingDirection;
    float fovRange;
    int originalDirection;
    bool isAgressive;
    bool isPredator;

    Transform head;
    float startAngle;
    float angleStep;
    float sightAngle;
    float rayCount;
    float rayDistance;
    float minFollowDistance;

    public CheckForTargetInRange(Transform _transform, float _senseOfSmell, string _foodName, float _fovRange, Transform _head, float _startAngle, float _angleStep, float _sightAngle, float _rayCount, float _rayDistance, float _minFollowDistance, int _originalDirection, bool _isAgressive, bool _isPredator)
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
        isPredator = _isPredator;
    }

    public override NodeState Evaluate()
    {
        //On regarde la cible
        object t = GetData("target");
        //Si la creature n'a pas de cible
        if  (t == null)
        {
            //On regarde les collider autour d'elle
            if ((bool)GetData("debug"))
                Debug.Log("Looking for target");
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, senseOfSmell, layerMask);

            //Si il y a un collider valide autour
            if (colliders.Length > 0)
            {
                foreach (Collider2D collider in colliders)
                {
                    //Si le collider est celui du joueur et que la creature est aggressive
                    if (collider.gameObject.tag == "Player")
                    {
                        if (isPredator)
                        {
                            parent.parent.SetData("target", collider.transform);
                            parent.parent.SetData("pathTarget", "player");
                            parent.parent.SetData("pathState", 1);
                            parent.parent.SetData("isAttacking", true);
                            state = NodeState.SUCCESS;
                            return state;
                        }
                        else if (isAgressive)
                        {
                            //Debug.Log(collider.gameObject.transform.position);
                            //Si le joueur est proche, on lance le pathfinding et l'attaque
                            if (Vector2.Distance(collider.gameObject.transform.position, transform.position) < fovRange)
                            {
                                parent.parent.SetData("target", collider.transform);
                                parent.parent.SetData("pathTarget", "player");
                                parent.parent.SetData("pathState", 1);
                                parent.parent.SetData("isAttacking", true);
                                state = NodeState.SUCCESS;
                                return state;
                            }
                            //Sinon, on regarde si le joueur est dans le champ de vision, et si oui, on commence le pathfinding et l'attaque
                            else
                            {
                                facingDirection = transform.localScale.x == 1 ? 1 : -1;
                                facingDirection *= originalDirection;
                                int targetIsRight = (collider.gameObject.transform.position.x - transform.position.x) > 0 ? 1 : -1;

                                if (targetIsRight == facingDirection)
                                {
                                    if (Vision())
                                    {
                                        //Debug.Log("Player is seen!");
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
                    }
                    else if (collider.gameObject.tag == "Bait")
                    {
                        //Debug.Log(collider.gameObject.transform.position);
                        if (Vector2.Distance(collider.gameObject.transform.position, transform.position) < fovRange)
                        {
                            //Debug.Log("Player is seen!");
                            parent.parent.SetData("target", collider.transform);
                            parent.parent.SetData("pathTarget", "bait");
                            parent.parent.SetData("pathState", 1);
                            parent.parent.SetData("checkState", collider.gameObject);
                            state = NodeState.SUCCESS;
                            return state;
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
            if (GetData("pathTarget") != null && (string)GetData("pathTarget") == "ally")
            {
                state = NodeState.SUCCESS;
                return state;
            }
            //Si la cible est deja le joueur
            else if (GetData("pathTarget") != null && (string)GetData("pathTarget") == "player")
            {
                //On regarde la distance avec la cible
                var pos = (Transform)GetData("target");
                //Si la distance est trop petite
                if (Vector2.Distance(pos.position, transform.position) > minFollowDistance)
                {
                    //Debug.Log("player is too far");
                    //On arrete le pathfinding et l'attaque, mais on ne reset pas la position de la cible, 
                    parent.parent.ClearData("target");
                    parent.parent.ClearData("pathTarget");
                    parent.parent.SetData("pathState", 0);
                    transform.gameObject.GetComponent<CreaturePathfinding>().StopPathFinding();
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
            else if (GetData("pathTarget") != null && (string)GetData("pathTarget") == "bait")
            {
                if ((GameObject)GetData("checkState") == null)
                {
                    //Debug.Log("player is too far");
                    parent.parent.ClearData("target");
                    parent.parent.ClearData("pathTarget");
                    parent.parent.SetData("pathState", 0);
                    transform.gameObject.GetComponent<CreaturePathfinding>().StopPathFinding();
                    state = NodeState.FAILURE;
                    return state;
                }
                else
                {
                    state = NodeState.SUCCESS;
                    return state;
                }
            }
            //La cible du joueur bypass les autres cibles
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
                                        //Debug.Log("Player is seen!");
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
                        else if (collider.gameObject.tag == "Bait")
                        {
                            //Debug.Log(collider.gameObject.transform.position);
                            if (Vector2.Distance(collider.gameObject.transform.position, transform.position) < fovRange)
                            {
                                //Debug.Log("Player is seen!");
                                parent.parent.SetData("target", collider.transform);
                                parent.parent.SetData("pathTarget", "bait");
                                parent.parent.SetData("pathState", 1);
                                parent.parent.SetData("checkState", collider.gameObject);
                                state = NodeState.SUCCESS;
                                return state;
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
