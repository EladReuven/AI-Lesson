using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Egg : MonoBehaviour
{
    private static int GlobalID = 0;

    public int ID {  get; private set; }
    private void Awake()
    {
        GlobalID++;
        ID = GlobalID;
    }
}
