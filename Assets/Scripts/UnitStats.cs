using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats
{
    [SerializeField] private float healthPoint;
    public float HealthPoint
    {
        get { return healthPoint; }
        set { healthPoint = value; }
    }

    public UnitStats(float hp)
    {
        HealthPoint = hp;
    }
}
