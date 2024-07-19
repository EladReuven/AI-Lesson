using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstarPathfinding
{
    public class PathfindingUnit : MonoBehaviour
    {
        const float pathUpdateMoveThreshold = 0.3f;
        const float minPathUpdateTime = 0.2f;

        [SerializeField] private Transform _target;

        [SerializeField] float baseSpeed = 5f;
        [SerializeField] float baseTurnSpeed = 2f;
        [SerializeField] float turnDist = 5f;

        Path path;

        private void Start()
        {
            StartCoroutine(UpdatePath());
        }

        public Transform Target { get { return _target; } set { _target = value; } }

        public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new Path(waypoints, transform.position, turnDist);
                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }

        IEnumerator UpdatePath()
        {
            if (Time.timeSinceLevelLoad < 0.3f)
                yield return new WaitForSeconds(0.3f);

            PathRequestManager.RequestPath(transform.position, _target.position, OnPathFound);

            float sqrtMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = Target.position;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);
                if ((Target.position - targetPosOld).sqrMagnitude > sqrtMoveThreshold)
                {
                    PathRequestManager.RequestPath(transform.position, _target.position, OnPathFound);
                    targetPosOld = Target.position;
                }
            }
        }

        IEnumerator FollowPath()
        {
            bool followingPath = true;
            int pathIndex = 0;
            transform.LookAt(path.lookPoints[0]);
            float speedPercent = 1;
            while (followingPath)
            {
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.z);
                while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }
                    else
                        pathIndex++;
                }

                if (followingPath)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(path.lookPoints[pathIndex] - transform.position);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * baseTurnSpeed);
                    transform.Translate(Vector3.forward * Time.deltaTime * baseSpeed * speedPercent, Space.Self);
                }
                yield return null;
            }
        }

        static float gizmoLineLength = 5f;
        static float gizmoCubeSize = 0.3f;
        private void OnDrawGizmos()
        {
            if (path == null)
                return;

            path.DrawWithGizmos(turnDist, gizmoCubeSize);
        }
    }

}
