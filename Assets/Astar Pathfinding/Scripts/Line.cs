using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

namespace AstarPathfinding
{
    public struct Line
    {
        const float verticalLineGradient = 1e5f;
        float gradient;
        float yIntercept;
        Vector2 pointOnLine1;
        Vector2 pointOnLine2;
        float gradientPerpendicular;
        bool approachSide;

        public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
        {
            float dx = pointOnLine.x - pointPerpendicularToLine.x;
            float dy = pointOnLine.y - pointPerpendicularToLine.y;
            if (dx == 0)
                gradientPerpendicular = dx / dy;
            else
                gradientPerpendicular = dy / dx;

            if (gradientPerpendicular == 0)
                gradient = verticalLineGradient;
            else
                gradient = -1 / gradientPerpendicular;

            yIntercept = pointOnLine.y - gradient * pointOnLine.x;
            pointOnLine1 = pointOnLine;
            pointOnLine2 = pointOnLine + new Vector2(1, gradient);

            approachSide = false;
            approachSide = GetSide(pointPerpendicularToLine);
        }

        bool GetSide(Vector2 p)
        {
            return (p.x - pointOnLine1.x) * (pointOnLine2.y - pointOnLine1.y) > (p.y - pointOnLine1.y) * (pointOnLine2.x - pointOnLine1.x);
        }

        public bool HasCrossedLine(Vector2 p)
        {
            return GetSide(p) != approachSide;
        }

        public float DistanceFromPoint(Vector2 p)
        {
            float yInterceptPerpendicular = p.y = gradientPerpendicular * p.x;
            float intersectX = (yInterceptPerpendicular - yIntercept) / (gradient - gradientPerpendicular);

            float intersectY = gradient * intersectX + yIntercept;
            return Vector2.Distance(p, new Vector2(intersectX, intersectY));
        }

        public void DrawWithGizmos(float length)
        {
            Vector3 lineDir = new Vector3(1, 0, gradient).normalized;
            Vector3 lineCenter = new Vector3(pointOnLine1.x, 0, pointOnLine1.y);
            Gizmos.DrawLine(lineCenter - lineDir * length / 2f, lineCenter + lineDir * length / 2f);
        }
    }
}