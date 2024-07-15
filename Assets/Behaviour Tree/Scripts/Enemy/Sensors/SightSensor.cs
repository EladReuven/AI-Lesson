using UnityEngine;
using UnityEngine.Events;

public class SightSensor : MonoBehaviour, ISensor
{
    [SerializeField] float _range;
    [SerializeField] float _angle;
    [SerializeField] Transform _target;
    Vector3 _lastSeenPos;
    protected SensorType _sensorType = SensorType.Sight;

    public SensorType GetSensorType => _sensorType;

    public UnityEvent<Vector3> OnLastPositionUpdated = new();

    bool InRange => Vector3.Distance(_target.position, transform.position) <= _range;
    bool InAngle
    {
        get
        {
            // Calculate the direction from _transform to _playerTransform
            Vector3 directionToPlayer = (_target.position - transform.position).normalized;

            // Calculate the angle between _transform's forward and the direction to the player
            float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);

            // Check if the angle is within half of the specified angle
            return angleToPlayer <= _angle / 2.0f;
        }
    }


    public bool CheckForTarget()
    {
        if (InAngle && InRange)
        {
            //send raycast, if hit player return true
            Ray ray = new Ray(transform.position, (_target.position - transform.position).normalized);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform == _target)
                {
                    _lastSeenPos = _target.position;
                    OnLastPositionUpdated.Invoke(_lastSeenPos);
                    print("target in sight");
                    return true;
                }
            }
        }
        return false;
    }

    public Vector3 GetLastPos()
    {
        return _lastSeenPos;
    }

    public void SetTarget(Transform target)
    {
        _target = target;
    }

    SensorType ISensor.GetSensorType()
    {
        return _sensorType;
    }

    private void OnDrawGizmos()
    {
        if (InRange && InAngle)
            Gizmos.color = Color.green;
        else
            Gizmos.color = Color.red;


        Quaternion leftRayRotation = Quaternion.AngleAxis(-_angle / 2.0f, transform.up);
        Quaternion rightRayRotation = Quaternion.AngleAxis(_angle / 2.0f, transform.up);

        Vector3 leftRayDirection = leftRayRotation * transform.forward;
        Vector3 rightRayDirection = rightRayRotation * transform.forward;

        Gizmos.DrawLine(transform.position, transform.position + leftRayDirection * _range);
        Gizmos.DrawLine(transform.position, transform.position + rightRayDirection * _range);

    }
}
