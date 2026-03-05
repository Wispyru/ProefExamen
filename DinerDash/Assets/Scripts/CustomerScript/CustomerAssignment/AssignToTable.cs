using UnityEngine;

public class AssignToTable : MonoBehaviour
{
    private TableData _tableState;
    private string _tagToCompare = "CustomerGroup";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == _tagToCompare && GameData.CurrentGameState == GameState.AssignmentMode) CheckIfAvailable();
    }

    private void CheckIfAvailable()
    {
        _tableState = GetComponent<TableData>();

        if (_tableState.TableState != TableStateEnum.available) return;

        Debug.Log("This table is available!");
    }

}
