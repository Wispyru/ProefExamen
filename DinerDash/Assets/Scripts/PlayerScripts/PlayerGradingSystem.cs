using UnityEngine;

public class PlayerGradingSystem : MonoBehaviour
{
    private float _dishCost;
    public GameData GameData;

    public void CustomerPayment()
    {
        GameData.Money += _dishCost;
    }
}
