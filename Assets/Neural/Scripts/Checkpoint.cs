using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] int ID = 0;
    [SerializeField] bool finalCheckpoint = false;
    [SerializeField] Checkpoint nextCheckpoint;

    public Checkpoint GetNextCheckpoint()
    {
        if (finalCheckpoint)
        {
            return null;
        }
        return nextCheckpoint;
    }
}
