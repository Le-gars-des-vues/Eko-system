using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TardidogBT : BTree
{
    [Header("Movement Variables")]
    [SerializeField] Transform target;
    [SerializeField] bool isGrounded;
    [SerializeField] BoxCollider2D territory;
    [SerializeField] int maxMovingDistance;
    [SerializeField] int originalDirection;

    [Header("Senses Variables")]
    [SerializeField] TardidogMovement dog;
    [SerializeField] float senseOfSmell;
    [SerializeField] float fovRange;
    [SerializeField] float minFollowDistance;

    [SerializeField] string foodName;

    protected override BehaviorNode SetupTree()
    {
        BehaviorNode root = new Selector(new List<BehaviorNode>
        {
            new TransferInfos(gameObject), 
            new Sequence(new List<BehaviorNode>
            {
                new CheckForTargetInRange(transform, senseOfSmell, foodName, fovRange, dog.head, dog.startAngle, dog.angleStep, dog.sightAngle, dog.rayCount, dog.rayDistance, minFollowDistance, originalDirection),
                new AssignTarget(target),
            }),
            new Wander(gameObject.transform, target, isGrounded, territory, maxMovingDistance)
        });
        return root;
    }
}
