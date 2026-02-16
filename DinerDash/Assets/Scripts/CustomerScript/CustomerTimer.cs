using UnityEngine;

public class CustomerTimer : MonoBehaviour
{

    public float CurrentTime; // do not change!!!
    public bool TimerActive;
    private CustomerData _customerData;
    public float _customerTime;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _customerData = GetComponent<CustomerData>();
        _customerTime = _customerData.CustomerTime;
        ResetTimer();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void timerFunctionality()
    {
        if (TimerActive && CurrentTime > 0)
        {
            CurrentTime -= Time.deltaTime;
        }

    }

    /// <summary>
    /// Start the PlayerTimer
    /// </summary>
    public void StartTimer()
    {
        if (TimerActive) return;
        TimerActive = true;

    }

    /// <summary>
    /// Stops the player timer
    /// </summary>
    public void StopTimer()
    {
        // If timeractive is already false, do nothing. if it isn't false, make it false.
        if (!TimerActive) return;
        TimerActive = false;
    }

    /// <summary>
    /// Resets the player timer
    /// </summary>
    public void ResetTimer()
    {
        CurrentTime = _customerTime;
    }


}