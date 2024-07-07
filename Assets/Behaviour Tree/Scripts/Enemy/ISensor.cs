using System;
using UnityEngine;
using UnityEngine.Events;

public interface ISensor
{
    public abstract bool CheckForTarget();
    public abstract Vector3 GetLastPos();
    public abstract SensorType GetSensorType();
    public abstract void SetTarget(Transform target);
}
