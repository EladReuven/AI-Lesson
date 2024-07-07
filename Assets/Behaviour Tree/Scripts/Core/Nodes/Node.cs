using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BehaviourTree
{
    public class Node
    {
        public Node parent;

        protected NodeState _state;
        protected List<Node> _children = new();

        private Dictionary<string, object> _dataContext = new Dictionary<string, object>();

        public Node()
        {
            parent = null;
        }
        public Node(List<Node> children)
        {
            foreach (var child in children)
            {
                _Attach(child);
            }
        }

        void _Attach(Node node)
        {
            node.parent = this;
            _children.Add(node);
        }

        /// <summary>
        /// Gets the parent node recursively until desired layer is reached
        /// </summary>
        /// <param name="layersAbove">How many layers above the current node. '0' is identical to parent variable.</param>
        /// <returns></returns>
        protected Node GetParent(int layersAbove = 1)
        {
            if (parent == null)
                return null;

            if (layersAbove == 0)
                return parent;

            return GetParent(layersAbove - 1);
        }

        protected Node GetRoot()
        {
            Node rootNode = this;
            while (rootNode.parent != null)
            {
                rootNode = rootNode.parent;
            }
            return rootNode;
        }

        public virtual NodeState Evaluate() => NodeState.RUNNING;

        public void SetData(string key, object value)
        {
            _dataContext[key] = value;
        }

        public object GetData(string key)
        {
            object value = null;
            if (_dataContext.TryGetValue(key, out value))
                return value;

            Node node = parent;
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
            if (_dataContext.ContainsKey(key))
            {
                _dataContext.Remove(key);
                return true;
            }

            Node node = parent;
            while (node != null)
            {
                bool cleared = node.ClearData(key);
                if (cleared)
                    return true;
                node = node.parent;
            }
            return false;
        }

    }

    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FALIURE,
    }
}
