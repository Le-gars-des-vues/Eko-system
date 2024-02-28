using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviorTree
{
    public class Selector : BehaviorNode
    {
        public Selector() : base() { }
        public Selector(List<BehaviorNode> children) : base(children) { }

        public override NodeState Evaluate()
        {
            foreach (BehaviorNode node in children)
            {
                switch (node.Evaluate())
                {
                    case NodeState.FAILURE:
                        continue;

                    case NodeState.SUCCESS:
                        state = NodeState.SUCCESS;
                        continue;

                    case NodeState.RUNNING:
                        state = NodeState.SUCCESS;
                        return state;

                    default:
                        continue;
                }
            }

            state = NodeState.FAILURE;
            return state;
        }
    }
}
