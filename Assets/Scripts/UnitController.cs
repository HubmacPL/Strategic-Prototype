using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OrderTypes
{
    MoveToPoint,
    AttackOponnent,
}
struct Order
{
    public OrderTypes orderType;
    public GameObject opponentTarger;
    public Vector3 pointTarget;
}
public enum UnitTypes
{
    Infantry,
    Tank,
}
public enum UnitState
{
    MoveState,
    Attack,
    Wait,
    Order,
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

    [SerializeField] private bool controlledByUI = true;
    public bool ControlledByUI
    {
        get { return controlledByUI; }
        set { this.controlledByUI = value; }
    }
    [SerializeField] private UnitState currentState;

    [SerializeField] private Queue<Order> ordersQueue = new Queue<Order>();

    [SerializeField] private GameObject enemyTarget;
    [SerializeField] private List<GameObject> opponents;
    [SerializeField] private float moveTargetMarginDistance = 5.0f;


    [Header("Debug Settings")]
    [SerializeField] private bool freezeMode = false;
    [SerializeField] private bool noDamageMode = false;


    private void Start()
    {
        unitInfo = gameObject.GetComponent<UnitInfo>();
        unitStats = new UnitStats(unitInfo.HealthPoint);
        unitRigidbody = gameObject.GetComponent<Rigidbody>();

        if(controlledByUI)
        {
            opponents = UnitsManager.Instance.playerTeam;
        }
        else
        {
            opponents = UnitsManager.Instance.uiTeam;
        }

        currentState = UnitState.Wait;
    }

    private void Update()
    {
        //Make unit states
        MakeState(currentState);
    }

    private void StateChange()
    {
    }

    private void MakeState(UnitState state)
    {
        switch (state)
        {
            case UnitState.Attack:
                AttackState();
                break;
            case UnitState.MoveState:
                MoveState();
                break;
            case UnitState.Wait:
                WaitState();
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
        bool mishit = false;
        float mishitCalculate;
        mishitCalculate = Random.Range(0, 100);

        if(mishitCalculate < unitInfo.Accuracy)
        {
            mishit = true;
        }
        else
        {
            mishit = false;
        }

        if(mishit == true)
        {
            Debug.Log(gameObject.name + " Zpudłował");
            return;
        }

        float damage = Random.Range(unitInfo.DamagePerAttack.x, unitInfo.DamagePerAttack.y);
        enemyTarget.GetComponent<UnitController>().Damage(damage);
        Debug.Log("OBERWAŁ: "+enemyTarget.name+" Za: "+damage+" I jego hp wynosi: "+enemyTarget.GetComponent<UnitController>().UnitStats.HealthPoint);
    }
    private void AttackState()
    {
        if(enemyTarget != null)
        {
            float distance = Vector3.Distance(enemyTarget.transform.position, transform.position);
            if (distance > unitInfo.WeaponRange)
            {
                currentState = UnitState.Wait;
            }
            else
            {
                moveVector = enemyTarget.transform.position - transform.position;
                Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
                gameObject.transform.rotation = rotation;
                AttackActivator(unitInfo.TimeBetweenAttack);
            }
        }
        else
        {
            currentState = UnitState.Wait;
        }
    }
    private void WaitState()
    {
        if(ordersQueue.Count != 0)
        {
            switch(ordersQueue.Peek().orderType)
            {
                case OrderTypes.AttackOponnent:
                    enemyTarget = ordersQueue.Peek().opponentTarger;
                    currentState = UnitState.MoveState;
                    break;
                case OrderTypes.MoveToPoint:
                    currentState = UnitState.MoveState;
                    moveTarget = ordersQueue.Peek().pointTarget;
                    break;
            }
        }
        else
        {
            if(opponents.Count > 0)
            {
                foreach (GameObject gm in opponents)
                {
                    if (Vector3.Distance(gm.transform.position, gameObject.transform.position) < unitInfo.WeaponRange)
                    {
                        enemyTarget = gm;
                        currentState = UnitState.Attack;
                    }
                }
            }
        }
    }
    private void MoveState()
    {
        if(enemyTarget != null)
        {
            Vector3 moveTrg;
            moveTrg = enemyTarget.transform.position;
            moveVector = moveTrg - transform.position;
            Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
            transform.rotation = rotation;

            if(Vector3.Distance(moveTrg, transform.position) > unitInfo.WeaponRange)
            {
                Movement();
            }
            else
            {
                ordersQueue.Dequeue();
                currentState = UnitState.Attack;
            }
        }
        else if(moveTarget != null)
        {
            moveVector = moveTarget - transform.position;
            Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
            transform.rotation = rotation;

            if(Vector3.Distance(moveTarget, transform.position) > moveTargetMarginDistance)
            {
                Movement();
            }
            else
            {
                ordersQueue.Dequeue();
                currentState = UnitState.Wait;
            }
        }
        else
        {
            ordersQueue.Dequeue();
            currentState = UnitState.Wait;
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
            Debug.Log("Unit dead");
            UnitStats.HealthPoint = UnitStats.HealthPoint - strikeForce;
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

    public void GiveOrder(GameObject gm)
    {
        ordersQueue.Clear();
        enemyTarget = null;

        Order order = new Order();
        order.orderType = OrderTypes.AttackOponnent;
        order.opponentTarger = gm;

        ordersQueue.Enqueue(order);

        currentState = UnitState.Wait;
    }
    public void GiveOrder(Vector3 pos)
    {
        ordersQueue.Clear();
        enemyTarget = null;

        Order order = new Order();
        order.orderType = OrderTypes.MoveToPoint;
        order.pointTarget = pos;

        ordersQueue.Enqueue(order);

        currentState = UnitState.Wait;
    }

}
