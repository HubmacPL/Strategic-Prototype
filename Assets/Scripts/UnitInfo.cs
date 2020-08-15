using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitInfo : MonoBehaviour
{
    [SerializeField] private float healthPoint;

    public float HealthPoint
    {
        get { return healthPoint; }
    }
    [SerializeField] private float weaponRange;
    public float WeaponRange
    {
        get { return weaponRange; }
    }
    [SerializeField] private float damagePerAttack;
    public float DamagePerAttack
    {
        get { return damagePerAttack; }
    }
    [SerializeField] private float timeBetweenAttack;
    public float TimeBetweenAttack
    {
        get { return timeBetweenAttack; }
    }
    [SerializeField] private float accuracy;
    public float Accuracy
    {
        get { return accuracy; }
    }
    [SerializeField] private string name;
    public string Name
    {
        get { return name; }
    }
    [SerializeField] private float speed;
    public float Speed
    {
        get { return speed; }
    }
    [SerializeField] private UnitTypes unitType;
    public UnitTypes Unittype
    {
        get { return unitType; }
    }
}