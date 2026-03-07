using UnityEngine;

public class TempTimerTrigger : MonoBehaviour
{
    /// <summary>
    /// Stops timer when customer enters trigger
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        CustomerTimer customerTimer = other.GetComponent<CustomerTimer>();

        if (customerTimer == null) return;

        customerTimer.StopTimer();
    }
}

