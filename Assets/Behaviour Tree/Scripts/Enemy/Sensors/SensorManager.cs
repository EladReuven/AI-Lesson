using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SensorManager : MonoBehaviour
{
    [Header("sensors for this enemy")]
    [SerializeField] SensorType[] _sensorsToCreate;

    [Header("for info")]
    [SerializeField] List<ISensor> _sensors;
    public List<ISensor> Sensors => _sensors;

    public ISensor GetSensor(SensorType sType)
    {
        foreach (ISensor sensor in _sensors)
        {
            if(sensor.GetSensorType() == sType)
                return sensor;
        }
        return null;
    }


}

public enum SensorType
{
    Sight,
    Sound,
}