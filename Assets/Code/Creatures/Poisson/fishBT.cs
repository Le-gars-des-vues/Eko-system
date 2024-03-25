using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class FishBT : BTree
{
    [Header("Movement Variables")]
    [SerializeField] Transform target;
    [SerializeField] bool isAgressive;

    [SerializeField] int minWaitTime;
    [SerializeField] int maxWaitTime;
    [SerializeField] int maxMovingDistance;
    [SerializeField] int originalDirection;

    [SerializeField] FishMovement fish;
    [SerializeField] CreatureState state;

    protected override BehaviorNode SetupTree()
    {
        BehaviorNode root = new Selector(new List<BehaviorNode>
        {
            new TransferInfos(gameObject, target), 
            new Sequence(new List<BehaviorNode>
            {
                new CheckHealth(gameObject),
                new Flee(target, gameObject, maxMovingDistance),
            }),
            new Sequence(new List<BehaviorNode>
            {
                new CheckForTargetInRange(transform, state.senseOfSmell, state.foodName, state.fovRange, fish.head, fish.startAngle, fish.angleStep, fish.sightAngle, fish.rayCount, fish.rayDistance, state.minFollowDistance, originalDirection, isAgressive),
                new AssignTarget(target, gameObject),
            }),
            new Wander(gameObject.transform, target, state.territory, maxMovingDistance, minWaitTime, maxWaitTime)
        });
        return root;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, state.fovRange);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, state.senseOfSmell);
    }
}
