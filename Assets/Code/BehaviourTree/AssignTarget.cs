using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AssignTarget : BehaviorNode
{
    private Transform target;
    private GameObject creature;

    public AssignTarget(Transform _target, GameObject _creature)
    {
        target = _target;
        creature = _creature;
    }

    public override NodeState Evaluate()
    {
        var tempTarget = (Transform)GetData("target");
        target.position = tempTarget.position;

        var pathfindingState = (int)GetData("pathState");
        if (pathfindingState == 1)
        {
            creature.GetComponent<CreaturePathfinding>().NewTarget(target.gameObject);
            if ((bool)GetData("debug"))
                Debug.Log("Set pathfinding to true");
            parent.parent.SetData("pathState", 2);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
