using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class AssignToTable : MonoBehaviour
{
    private TableData _tableData;
    private string _tagToCompare = "CustomerGroup";
    private GameObject _selectedTable;

    [SerializeField]
    private List<GameObject> _seatingPositions;


    public bool CanPlaceAtTabel = false;


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == _tagToCompare && GameData.CurrentGameState == GameState.AssignmentMode)
        {
            _selectedTable = other.gameObject;
            CheckIfAvailable();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        _selectedTable = null;
    }

    private void CheckIfAvailable()
    {
        _tableData = GetComponent<TableData>();

        if (_tableData.TableState != TableStateEnum.available) return;
        CanPlaceAtTabel = true;

        Debug.Log("This table is available!");
    }


    public void PlaceAtTable(GameObject customers)
    {
        if (!CanPlaceAtTabel) return;


    }
}
