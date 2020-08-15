using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitTypes
{
    Infantry,
    Tank,
}
public enum UnitState
{
    Looking,
    Attack,
}

public class UnitController : MonoBehaviour
{
    [SerializeField] private static UnitInfo unitInfo;
    [SerializeField] private UnitStats unitStats;

    public UnitStats UnitStats
    {
        get { return unitStats; }
        set { this.unitStats = value; }
    }

    private UnitsManager unitsManagerInstance;
    [SerializeField] private bool controlledByUI = true;
    [SerializeField] private UnitState currentState;

    [SerializeField] private GameObject enemyTarget;
    [SerializeField] private List<GameObject> opponents;

    private void Start()
    {
        unitInfo = gameObject.GetComponent<UnitInfo>();
        unitStats = new UnitStats(unitInfo.HealthPoint);
        unitsManagerInstance = GameObject.FindGameObjectWithTag("GameController").GetComponent<UnitsManager>();
        unitRigidbody = gameObject.GetComponent<Rigidbody>();

        if(controlledByUI)eefe
        {
            opponents = unitsManagerInstance.playerTeam;
        }
        else
        {
            opponents = unitsManagerInstance.uiTeam;
        }

        currentState = UnitState.Looking;
    }

    private void Update()
    {
        MakeState(currentState);
    }

    private void StateChange()
    {
    }

    private void MakeState(UnitState state)
    {
        switch(state)
        {
            case UnitState.Attack:
                AttackState();
                    break;
            case UnitState.Looking:
                LookingState();
                    break;
        }
    }
    private bool attackPossible = true;

    public void AttackActivator(float time)
    {
        if(attackPossible)
        {
            StartCoroutine(IAttack(time));
        }
    }

    IEnumerator IAttack(float time)
    {
        attackPossible = false;
        yield return new WaitForSeconds(time);
        attackPossible = true;
        MakeAttack();
    }
    private void MakeAttack()
    {
        enemyTarget.GetComponent<UnitController>().Damage(unitInfo.DamagePerAttack);
        Debug.Log("BOOM KURWO");
    }
    private void AttackState()
    {
        float distance = Vector3.Distance(enemyTarget.transform.position, transform.position);
        if (distance > unitInfo.WeaponRange)
        {
            currentState = UnitState.Looking;
        }
        else
        {
            AttackActivator(unitInfo.TimeBetweenAttack);
        }
    }
    private void LookingState()
    {
        if(enemyTarget == null)
        {
            List<float> distances = new List<float>();
            List<float> distancesSortCopy = new List<float>();
            List<GameObject> opponentsCopy = new List<GameObject>();

            opponentsCopy = opponents;

            foreach (GameObject gm in opponentsCopy)
            {
                distances.Add(Vector3.Distance(gm.transform.position, transform.position));
            }

            distancesSortCopy = distances;

            distancesSortCopy.Sort();

            if (distancesSortCopy[0] > distancesSortCopy[distancesSortCopy.Count - 1])
            {
                int i = 0;
                foreach(float fl in distances)
                {
                    if(fl == distancesSortCopy[distancesSortCopy.Count - 1])
                    {
                        enemyTarget = opponents[i];
                    }
                    i++;
                }
            }
            else
            {
                int i = 0;
                foreach (float fl in distances)
                {
                    if (fl == distancesSortCopy[0])
                    {
                        enemyTarget = opponents[i];
                    }
                    i++;
                }
            }
        }
        else
        {
            if (Vector3.Distance(enemyTarget.transform.position, transform.position) < unitInfo.WeaponRange)
            {
                currentState = UnitState.Attack;
            }
            else
            {
                moveTarget = enemyTarget.transform.position;
                moveVector = moveTarget - transform.position;
                Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
                transform.rotation = rotation;

                Movement();
            }
        }
    }

    private Vector3 moveTarget;
    private Vector3 moveVector = new Vector3();
    [SerializeField] private Rigidbody unitRigidbody;
    private void Movement()
    {
        Vector3 unitVelocity = transform.rotation * Vector3.forward;
        unitVelocity = unitVelocity * unitInfo.Speed;

        unitRigidbody.velocity = unitVelocity;
    }

    public void Damage(float strikeForce)
    {
        if(strikeForce > unitStats.HealthPoint)
        {
            UnitDead();
        }
        else
        {
            unitStats.HealthPoint = unitStats.HealthPoint - strikeForce;
        }
    }

    private void UnitDead()
    {
        Destroy(gameObject);
    }

}
