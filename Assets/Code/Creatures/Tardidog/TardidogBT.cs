using UnityEngine;
using BehaviorTree;

public class TardidogBT : BTree
{
    public Transform target;
    public bool isGrounded;
    public BoxCollider2D territory;
    public int maxMovingDistance;

    protected override BehaviorNode SetupTree()
    {
        BehaviorNode root = new Wander(gameObject.transform, target, isGrounded, territory, maxMovingDistance);
        return root;
    }
}
