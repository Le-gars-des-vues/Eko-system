using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class FrogBT : BTree
{
    [Header("Movement Variables")]
    [SerializeField] Transform target;
    [SerializeField] int maxMovingDistance;
    [SerializeField] int originalDirection;
    [SerializeField] float creatureSize;

    [SerializeField] FrogMovement frog;
    [SerializeField] CreatureState state;

    protected override BehaviorNode SetupTree()
    {
        BehaviorNode root = new Selector(new List<BehaviorNode>
        {
            new CheckActivationDistance(state, state.activationRange),
            new TransferInfos(gameObject, target),
            new Sequence(new List<BehaviorNode>
            {
                new CheckHealth(gameObject),
                new Flee(target, gameObject, maxMovingDistance),
            }),
            new Sequence(new List<BehaviorNode>
            {
                new CheckForTargetInRange(transform, state.senseOfSmell, state.foodName, state.fovRange, frog.head, frog.startAngle, frog.angleStep, frog.sightAngle, frog.rayCount, frog.rayDistance, state.minFollowDistance, originalDirection, state.isAgressive, state.isPredator),
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
