using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class FlumpfBT : BTree
{
    [Header("Movement Variables")]
    [SerializeField] Transform target;
    [SerializeField] int maxMovingDistance;
    [SerializeField] int originalDirection;
    [SerializeField] float creatureSize;

    [SerializeField] FlumpfMovement fly;
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
                new CheckForTargetInRange(transform, state.senseOfSmell, state.foodName, state.fovRange, fly.head, fly.startAngle, fly.angleStep, fly.sightAngle, fly.rayCount, fly.rayDistance, state.minFollowDistance, originalDirection, state.isAgressive, state.isPredator),
                new AssignTarget(target, gameObject),
            }),
            new Wander(gameObject.transform, target, state.territory, maxMovingDistance, creatureSize)
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
