using System.Collections;
using UnityEngine;

public class CustomerTimer : MonoBehaviour
{
    public float CurrentTime; // do not change!!!
    public bool TimerActive;

    private CustomerData _customerData;
    private PlayerGradingSystem _playerGradingSystem;

    public float CustomerTime;

    private float _timerStartDelay = 2f;

    private void Start()
    {
        _customerData = GetComponent<CustomerData>();
        _playerGradingSystem = GetComponent<PlayerGradingSystem>();

        CustomerTime = _customerData.CustomerTime;

        ResetTimer();
        StartCoroutine(TimerDelay());
    }

    private void Update()
    {
        if (TimerActive)
        {
            TimerFunctionality();
        }
    }

    /// <summary>
    /// Handles timer countdown logic
    /// </summary>
    private void TimerFunctionality()
    {
        if (CurrentTime <= 0)
        {
            CurrentTime = 0;
            StopTimer();
            Debug.Log("Timer reached zero.");
            return;
        }

        CurrentTime -= Time.deltaTime;

        // Round to 2 decimals for clean display
        CurrentTime = Mathf.Round(CurrentTime * 100f) / 100f;
    }

    /// <summary>
    /// Stops the timer and calculates tip
    /// </summary>
    public void StopTimer()
    {
        if (!TimerActive) return;

        TimerActive = false;

        Debug.Log("Timer stopped at: " + CurrentTime);

        _playerGradingSystem.CalculateTip(CurrentTime);
    }

    /// <summary>
    /// Starts the timer without resetting time
    /// </summary>
    public void StartTimer()
    {
        if (TimerActive) return;

        TimerActive = true;

        Debug.Log("Timer resumed at: " + CurrentTime);
    }

    /// <summary>
    /// Resets timer back to starting value
    /// </summary>
    public void ResetTimer()
    {
        CurrentTime = CustomerTime;
        CurrentTime = Mathf.Round(CurrentTime * 100f) / 100f;

        Debug.Log("Timer reset to: " + CurrentTime);
    }

    private IEnumerator TimerDelay()
    {
        yield return new WaitForSeconds(_timerStartDelay);
        StartTimer();
    }
}