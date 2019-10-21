using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Rigidbody[] rbs;
    public int health = 100;
    // Start is called before the first frame update
    void Start()
    {
        rbs = GetComponentsInChildren<Rigidbody>();
        Ragdoll(false);
    }

    public void Ragdoll(bool state)
    {
        foreach (Rigidbody rb in rbs)
        {
            rb.isKinematic = !state;
        }
    }

    public void takeHit(int hitPoint)
    {
        health -= hitPoint;
    }
}
