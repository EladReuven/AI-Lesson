using AstarPathfinding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AstarPathfinding
{
    public class PathRequestManager : MonoBehaviour
    {
        static PathRequestManager instance;

        [SerializeField] Pathfinding pathfinding;

        Queue<PathRequest> pathRequestQueue = new();
        PathRequest currentPathRequest;
        bool isProcessingPath;

        private void Awake()
        {
            instance = this;
            if (pathfinding == null)
                pathfinding = GetComponent<Pathfinding>();
        }

        public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
        {
            PathRequest newRequest = new PathRequest(pathStart, pathEnd, callback);
            instance.pathRequestQueue.Enqueue(newRequest);
            instance.TryProcessNext();
        }

        void TryProcessNext()
        {
            if (!isProcessingPath && pathRequestQueue.Count > 0)
            {
                currentPathRequest = pathRequestQueue.Dequeue();
                isProcessingPath = true;
                pathfinding.StartFindPath(currentPathRequest.start, currentPathRequest.end);
            }
        }

        public void FinishedProcessingPath(Vector3[] path, bool success)
        {
            currentPathRequest.callback(path, success);
            isProcessingPath = false;
            TryProcessNext();
        }

        struct PathRequest
        {
            public Vector3 start;
            public Vector3 end;
            public Action<Vector3[], bool> callback;

            public PathRequest(Vector3 start, Vector3 end, Action<Vector3[], bool> callback)
            {
                this.start = start;
                this.end = end;
                this.callback = callback;
            }
        }
    }
}
