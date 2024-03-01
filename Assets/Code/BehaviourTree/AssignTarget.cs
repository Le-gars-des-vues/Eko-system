using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BehaviorTree;

public class AssignTarget : BehaviorNode
{
    private Transform target;

    public AssignTarget(Transform _target)
    {
        target = _target;
    }

    public override NodeState Evaluate()
    {
        var tempTarget = (Transform)GetData("target");
        target.position = tempTarget.position;

        state = NodeState.RUNNING;
        return state;
    }
}
