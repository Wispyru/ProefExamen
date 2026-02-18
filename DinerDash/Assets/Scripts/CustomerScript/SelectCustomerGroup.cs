using UnityEngine;

public class SelectCustomerGroup : MonoBehaviour
{
    public GameData Game;
    [SerializeField]
    private CanvasGroup _PopUpPanel;


    private void Start()
    {
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("BING BONG");
            Game.SelectedCustomerGroup = gameObject;
            TogglePopUp();
        }

    }

    private void TogglePopUp()
    {
        switch (_PopUpPanel.alpha)
        {
            case 0:
                _PopUpPanel.alpha = 1;
                break;
            case 1:
                _PopUpPanel.alpha = 0;
            break;
        }
    }
}
