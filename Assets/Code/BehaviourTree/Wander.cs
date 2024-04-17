using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class Wander : BehaviorNode
{
    private Transform creature;
    private Collider2D territory;
    private bool isWaiting = true;
    private int waitTime = 2;
    private float waitTimer;

    private bool isMoving;
    private int maxMovingDistance;
    private float creatureSize;
    private Transform targetPos;

    int minWaitTime;
    int maxWaitTime;

    public Wander(Transform _creature, Transform _targetPos, Collider2D _territory, int _maxMovingDistance, float _creatureSize, int _minWaitTime = 2, int _maxWaitTime = 10)
    {
        creature = _creature;
        targetPos = _targetPos;
        territory = _territory;
        maxMovingDistance = _maxMovingDistance;
        creatureSize = _creatureSize;
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
            //Si la creature n'est pas en train de bouger
            if (!isMoving)
            {
                //La destination du wander est un point random autour de la creature, on considere uniquement l'horizontal pour les creature qui ne vole pas
                Vector2 tempPos = (bool)GetData("isFlying") ? new Vector2(creature.position.x + Random.Range(-maxMovingDistance, maxMovingDistance), creature.position.y + Random.Range(-maxMovingDistance, maxMovingDistance)) : new Vector2(creature.position.x + Random.Range(-maxMovingDistance, maxMovingDistance), creature.position.y);
                //Si le point est dans le territoire
                if (territory.bounds.Contains(tempPos))
                {
                    if ((bool)GetData("debug"))
                        Debug.Log("Target is in bounds!");

                    //On regarde si la destination est walkable, et si elle est proche du sol pour les creatures qui ne volent pas
                    bool unWalkable = Physics2D.OverlapCircle(tempPos, 0.1f, LayerMask.GetMask("Ground"));
                    if (unWalkable)
                    {
                        RaycastHit2D heightOffset = Physics2D.Raycast(new Vector2(tempPos.x, tempPos.y + 5), Vector2.down, 5f, LayerMask.GetMask("Ground"));
                        if (heightOffset)
                            tempPos.y += heightOffset.point.y - tempPos.y + 1;

                        unWalkable = Physics2D.OverlapCircle(tempPos, 0.1f, LayerMask.GetMask("Ground"));
                    }
                    RaycastHit2D isWalkable = Physics2D.Raycast(tempPos, Vector2.down, 3f, LayerMask.GetMask("Ground"));
                    bool isUnderwater = Physics2D.OverlapCircle(tempPos, 0.1f, LayerMask.GetMask("Water"));

                    //Si oui, on commence le pathfinding vers la destination
                    if ((!unWalkable && isWalkable || (bool)GetData("isFlying")) && ((bool)GetData("isUnderwater") || (!(bool)GetData("isUnderwater") && !isUnderwater)))
                    {
                        if ((bool)GetData("debug"))
                            Debug.Log("Found target for path!");

                        //Si la creature no vole pas
                        if (!(bool)GetData("isFlying"))
                        {
                            //On regarde si la destination a suffisamment de place des deux cotes pour accueillir la creature
                            bool hasEnoughRightSpace = Physics2D.Raycast(new Vector2((tempPos.x + creatureSize / 2), tempPos.y), Vector2.down, 3f, LayerMask.GetMask("Ground"));

                            //Si on manque de place de l'un des deux bord, on tasse la tempPos
                            if (!hasEnoughRightSpace)
                            {
                                RaycastHit2D sideOffset = Physics2D.Raycast(new Vector2((tempPos.x + creatureSize / 2), tempPos.y - isWalkable.distance), Vector2.left, creatureSize / 2, LayerMask.GetMask("Ground"));
                                if (sideOffset)
                                    tempPos.x -= sideOffset.distance;
                            }

                            bool hasEnoughLeftSpace = Physics2D.Raycast(new Vector2((tempPos.x - creatureSize / 2), tempPos.y), Vector2.down, 3f, LayerMask.GetMask("Ground"));
                            if (!hasEnoughLeftSpace)
                            {
                                RaycastHit2D sideOffset = Physics2D.Raycast(new Vector2((tempPos.x - creatureSize / 2), tempPos.y - isWalkable.distance), Vector2.right, creatureSize / 2, LayerMask.GetMask("Ground"));
                                if (sideOffset)
                                    tempPos.x += sideOffset.distance;
                            }

                            //On regarde a nouveau si il y a de la place des deux bord apres la changements
                            hasEnoughRightSpace = Physics2D.Raycast(new Vector2((tempPos.x + creatureSize / 2), tempPos.y), Vector2.down, 3f, LayerMask.GetMask("Ground"));
                            hasEnoughLeftSpace = Physics2D.Raycast(new Vector2((tempPos.x - creatureSize / 2), tempPos.y), Vector2.down, 3f, LayerMask.GetMask("Ground"));

                            //Si oui on lance le pathfinding, sinon on revient a la premier etape
                            if (hasEnoughRightSpace && hasEnoughLeftSpace)
                            {
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
                            targetPos.position = tempPos;
                            parent.SetData("target", targetPos);
                            parent.SetData("pathTarget", "wandering");
                            parent.SetData("pathState", 1);
                            isMoving = true;
                        }
                    }
                    //Sinon, on skip la premier etape et on essaye une nouvelle destination random
                    else
                    {
                        waitTime = 0;
                        waitTimer = 0;
                        isWaiting = true;
                    }
                }
                //Sinon, on skip la premier etape et on essaye une nouvelle destination random
                else
                {
                    if ((bool)GetData("debug"))
                        Debug.Log("Target is outside of bounds!");
                    waitTime = 1;
                    waitTimer = 0;
                    isWaiting = true;
                }
            }
            //Si la creature bouge, elle qu'elle a atteint sa destination, on reset le pathfinding
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
        state = NodeState.RUNNING;
        return state;
    }
}
