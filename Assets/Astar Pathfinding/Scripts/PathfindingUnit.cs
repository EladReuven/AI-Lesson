using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstarPathfinding
{
    public class PathfindingUnit : MonoBehaviour
    {
        [SerializeField] private Transform _target;

        float speed = 5f;
        Vector3[] path;
        int targetIndex;

        public Transform Target { get { return _target; } set { _target = value; } }

        private void Start()
        {
            PathRequestManager.RequestPath(transform.position, _target.position, OnPathFound);
        }

        public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = newPath;
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }

        IEnumerator FollowPath()
        {
            Vector3 currenWaypoint = path[0];

            while (true)
            {
                if (transform.position == currenWaypoint)
                {
                    targetIndex++;
                    if (targetIndex >= path.Length)
                    {
                        yield break;
                    }
                    currenWaypoint = path[targetIndex];
                }
                transform.position = Vector3.MoveTowards(transform.position, currenWaypoint, speed * Time.deltaTime);
                yield return null;
            }
        }

        private void OnDrawGizmos()
        {
            if (path == null)
                return;

            Gizmos.color = Color.black;
            for (int i = targetIndex; i < path.Length; i++)
            {
                Gizmos.DrawCube(path[i], Vector3.one * 0.3f);

                if (i == targetIndex)
                    Gizmos.DrawLine(transform.position, path[i]);
                else
                    Gizmos.DrawLine(path[i - 1], path[i]);
            }
        }
    }

}
