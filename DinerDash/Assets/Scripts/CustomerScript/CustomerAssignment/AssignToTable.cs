using UnityEngine;

public class AssignToTable : MonoBehaviour
{
    private TableData _tableData;
    private string _tagToCompare = "CustomerGroup";

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == _tagToCompare && GameData.CurrentGameState == GameState.AssignmentMode) CheckIfAvailable();
    }

    private void CheckIfAvailable()
    {
        _tableData = GetComponent<TableData>();

        if (_tableData.TableState != TableStateEnum.available) return;

        Debug.Log("This table is available!");
    }

}
