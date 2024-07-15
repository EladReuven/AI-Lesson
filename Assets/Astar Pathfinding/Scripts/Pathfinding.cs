using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AstarPathfinding
{
    public class Pathfinding : MonoBehaviour
    {
        Grid grid;
        PathRequestManager requestManager;
        private void Awake()
        {
            grid = GetComponent<Grid>();
            requestManager = GetComponent<PathRequestManager>();
        }

        public void StartFindPath(Vector3 startPos, Vector3 targetPos)
        {
            StartCoroutine(FindPath(startPos, targetPos));
        }

        IEnumerator FindPath(Vector3 startPos, Vector3 endPos)
        {
            Vector3[] waypoints = new Vector3[0];
            bool pathSuccess = false;

            Node startNode = grid.NodeFromWorldPoint(startPos);
            Node endNode = grid.NodeFromWorldPoint(endPos);

            if (startNode.Walkable || endNode.Walkable)
            {
                Heap<Node> openSet = new Heap<Node>(grid.MaxSize); //set of nodes to be evaluated
                HashSet<Node> closedSet = new HashSet<Node>(); //set of nodes already evaluated
                openSet.Add(startNode);

                while (openSet.Count > 0)
                {
                    Node currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    if (currentNode == endNode)
                    {
                        pathSuccess = true;
                        //endNode.Parent = currentNode;
                        break;
                    }

                    foreach (Node neighbor in grid.GetNeighbors(currentNode))
                    {
                        if (!neighbor.Walkable || closedSet.Contains(neighbor))
                            continue;

                        int newMovementCostToNeighbor = currentNode.GCost + GetDistance(currentNode, neighbor) + neighbor.Penalty;
                        if (newMovementCostToNeighbor < neighbor.GCost || !openSet.Contains(neighbor))
                        {
                            neighbor.GCost = newMovementCostToNeighbor;
                            neighbor.HCost = GetDistance(neighbor, endNode);
                            neighbor.Parent = currentNode;

                            if (!openSet.Contains(neighbor))
                                openSet.Add(neighbor);
                            else
                                openSet.UpdateItem(neighbor);
                        }
                    }
                }
            }

            yield return null;
            if (pathSuccess)
                waypoints = RetracePath(startNode, endNode);
            requestManager.FinishedProcessingPath(waypoints, pathSuccess);
        }

        Vector3[] RetracePath(Node startNode, Node endNode)
        {
            List<Node> path = new List<Node>();
            Node currentNode = endNode;

            while (currentNode != startNode)
            {
                path.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            Vector3[] waypoints = SimplifyPath(path);
            Array.Reverse(waypoints);
            return waypoints;
        }

        Vector3[] SimplifyPath(List<Node> path)
        {
            List<Vector3> waypoints = new List<Vector3>();
            Vector2 directionOld = Vector2.zero;

            for(int i = 1; i < path.Count; i++)
            {
                Vector2 directionNew = new Vector2(path[i-1].GridX - path[i].GridX, path[i-1].GridY - path[i].GridY);
                if(directionNew != directionOld)
                    waypoints.Add(path[i].WorldPosition);

                directionOld = directionNew;
            }

            return waypoints.ToArray();
        }

        int GetDistance(Node nodeA, Node nodeB)
        {
            int disX = Mathf.Abs(nodeA.GridX - nodeB.GridX);
            int disY = Mathf.Abs(nodeA.GridY - nodeB.GridY);

            if (disX > disY)
                return 14 * disY + 10 * (disX - disY);
            return 14 * disX + 10 * (disY - disX);
        }



    }
}
