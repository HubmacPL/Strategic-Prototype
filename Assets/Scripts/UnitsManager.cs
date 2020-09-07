using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsManager : MonoBehaviour
{
    public static UnitsManager Instance;

    public List<UnitInfo> unitsInfo;

    public List<GameObject> playerTeam;
    public List<GameObject> uiTeam;

    public List<GameObject> selectedUnits;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(Instance.gameObject);

            Instance = this;
        }
    }

    private void Update()
    {
        SelectUnit();
    }

    private void SelectUnit()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out hit))
        {
            if (hit.collider.gameObject.tag == "Unit")
            {
                if(hit.collider.gameObject.GetComponentInParent<UnitController>().ControlledByUI == true)
                {
                    if(selectedUnits.Count != 0 && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        MakeOrders(hit.collider.gameObject);
                    }
                }
                else
                {
                    if(selectedUnits.Count != 0 && Input.GetKeyDown(KeyCode.Mouse0) && Input.GetKeyDown(KeyCode.LeftShift))
                    {
                        selectedUnits.Add(hit.collider.gameObject.transform.parent.gameObject);
                    }
                    else if(selectedUnits.Count != 0 && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        selectedUnits.Clear();
                        selectedUnits.Add(hit.collider.gameObject.transform.parent.gameObject);
                    }
                    else if(selectedUnits.Count == 0 && Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        selectedUnits.Add(hit.collider.gameObject.transform.parent.gameObject);
                    }
                }
            }
            else
            {
                if(selectedUnits.Count != 0 && Input.GetKeyDown(KeyCode.Mouse0))
                {
                    MakeOrders(hit.point);
                    selectedUnits.Clear();
                }
            }
        }
    }

    private void MakeOrders(GameObject gm)
    {
        foreach(GameObject gmf in selectedUnits)
        {
            gmf.GetComponent<UnitController>().GiveOrder(gm.transform.parent.gameObject);
        }
    }
    private void MakeOrders(Vector3 pos)
    {
        foreach(GameObject gm in selectedUnits)
        {
            gm.GetComponent<UnitController>().GiveOrder(pos);
        }
    }
}
