using System.Collections.Generic;
using UnityEngine;
using BehaviorTree;

public class TardidogBT : BTree
{
    [Header("Movement Variables")]
    [SerializeField] Transform target;
    [SerializeField] bool isGrounded;
    [SerializeField] int maxMovingDistance;
    [SerializeField] int originalDirection;

    [SerializeField] TardidogMovement dog;
    [SerializeField] CreatureState state;

    protected override BehaviorNode SetupTree()
    {
        BehaviorNode root = new Selector(new List<BehaviorNode>
        {
            new TransferInfos(gameObject),
            new Sequence(new List<BehaviorNode>
            {
                new CheckHealth(gameObject),
                new Flee(target, gameObject, maxMovingDistance),
            }),
            new Sequence(new List<BehaviorNode>
            {
                new CheckForTargetInRange(transform, state.senseOfSmell, state.foodName, state.fovRange, dog.head, dog.startAngle, dog.angleStep, dog.sightAngle, dog.rayCount, dog.rayDistance, state.minFollowDistance, originalDirection, true),
                new AssignTarget(target, gameObject),
            }),
            new Wander(gameObject.transform, target, isGrounded, state.territory, maxMovingDistance)
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
