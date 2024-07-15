using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstarPathfinding
{
    public class Node : IHeapItem<Node>
    {
        private bool _walkable;
        private Vector3 _worldPosition;
        int _gridX;
        int _gridY;
        int _gCost;
        int _hCost;
        int _movementPentaly;

        int heapIndex;
        
        public Node Parent { get; set; }
        /// <summary>
        /// units away from Start Position
        /// </summary>
        public int GCost { get { return _gCost; } set { _gCost = value; } }
        /// <summary>
        /// units away from End Position
        /// </summary>
        public int HCost { get { return _hCost; } set { _hCost = value; } }
        /// <summary>
        /// Combination of gCost + hCost
        /// </summary>
        public int FCost => _gCost + _hCost;
        public bool Walkable => _walkable;
        public int GridX => _gridX;
        public int GridY => _gridY;
        public Vector3 WorldPosition => _worldPosition;
        public int HeapIndex { get => heapIndex; set => heapIndex = value; }
        public int Penalty {get => _movementPentaly; set => _movementPentaly = value; }

        public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY, int penalty)
        {
            _walkable = walkable;
            _worldPosition = worldPosition;
            _gridX = gridX;
            _gridY = gridY;
            _movementPentaly = penalty;
        }

        public int CompareTo(Node other)
        {
            int compare = FCost.CompareTo(other.FCost);
            if(compare == 0)
            {
                compare = HCost.CompareTo(other.HCost);
            }

            return -compare;
        }
    }
}