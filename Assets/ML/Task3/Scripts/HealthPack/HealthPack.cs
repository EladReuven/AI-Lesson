using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPack : MonoBehaviour
{
    [SerializeField] int healAmount = 50;
    public int HealAmount => healAmount;

    //private void OnTriggerEnter(Collider other)
    //{
    //    Destroy(gameObject);
    //}
}
