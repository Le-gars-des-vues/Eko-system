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
        //On deplace la cible sur la position de la cible du pathfinding
        var tempTarget = (Transform)GetData("target");
        target.position = tempTarget.position;

        //Si le pathfinding est pret a debuter, on le commnence
        var pathfindingState = (int)GetData("pathState");
        if (pathfindingState == 1)
        {
            creature.GetComponent<CreaturePathfinding>().NewTarget(target.gameObject);
            Debug.Log("Set pathfinding to true");
            parent.parent.SetData("pathState", 2);
        }

        state = NodeState.RUNNING;
        return state;
    }
}
