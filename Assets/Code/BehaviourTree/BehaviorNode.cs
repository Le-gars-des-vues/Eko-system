using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class BehaviorNode
    {
        protected NodeState state;

        public BehaviorNode parent;
        protected List<BehaviorNode> children = new List<BehaviorNode>();

        private Dictionary<string, object> dataContext = new Dictionary<string, object>();

        public BehaviorNode()
        {
            parent = null;
        }

        public BehaviorNode(List<BehaviorNode> children)
        {
            foreach (BehaviorNode child in children)
            {
                Attach(child);
            }
        }

        private void Attach(BehaviorNode node)
        {
            node.parent = this;
            children.Add(node);
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        public void SetData(string key, object value)
        {
            dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object value = null;
            if (dataContext.TryGetValue(key, out value))
                return value;

            BehaviorNode node = parent;
            while (node != null)
            {
                value = node.GetData(key);
                if (value != null)
                    return value;
                node = node.parent;
            }
            return null;
        }

        public bool ClearData(string key)
        {
            if (dataContext.ContainsKey(key))
            {
                dataContext.Remove(key);
                return true;
            }

            BehaviorNode node = parent;
            while (node != null)
            {
                bool isCleared = node.ClearData(key);
                if (isCleared)
                    return true;
                node = node.parent;
            }
            return false;
        }
    }
}
