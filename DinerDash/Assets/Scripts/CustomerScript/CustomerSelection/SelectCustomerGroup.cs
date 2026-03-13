using System.Text.RegularExpressions;
using UnityEngine;

public class SelectCustomerGroup : MonoBehaviour
{
    public GameData Game;
    public GameObject Group;

    //private variables
    private bool _isAttached;
    private DragCustomers _dragScript;
    private AssignToTable _tableAssignment;

    private void Start()
    {
        _dragScript = GetComponent<DragCustomers>();
    }

    private void Update()
    {
        if (Game == null || Group == null) return;
        Game.CustomerGroup = Group;
    }


    public void OnMouseDrag()
    {
        _isAttached = true;
        Group = gameObject;
        //AttachCustomersToFinger(true);
        GameData.CurrentGameState = GameState.AssignmentMode;
        Debug.Log(GameData.CurrentGameState);

        if(_tableAssignment != null)
        {
            _tableAssignment.PlaceAtTable(Group);
        }
    }



    public void OnMouseUp()
    {
        _isAttached = false;
        Group = null;
        GameData.CurrentGameState = GameState.IdleMode;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Table")
        {
            _tableAssignment = other.gameObject.GetComponent<AssignToTable>();
        }
       
    }

    /* private void AttachCustomersToFinger(bool _isFloating)
     {
         if (_isAttached)
         {
             //TODO: Deactivate Animation
         }
         foreach(GameObject child in Group.GetComponentsInChildren<GameObject>())
         {
             Debug.Log("group is floating");
             //TODO: Activate Animation
         }
     }*/



}
