using System.Collections;
using UnityEngine;

public class CustomerTimer : MonoBehaviour
{
    public float CurrentTime; // do not change!!!
    public bool TimerActive;
    public float CustomerTime;

    private CustomerData _customerData;
    private PlayerGradingSystem _playerGradingSystem;
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

        _playerGradingSystem.CalculateTip(CurrentTime);
    }

    /// <summary>
    /// Starts the timer without resetting time
    /// </summary>
    public void StartTimer()
    {
        if (TimerActive) return;

        TimerActive = true;
    }

    /// <summary>
    /// Resets timer back to starting value
    /// </summary>
    public void ResetTimer()
    {
        CurrentTime = CustomerTime;
        CurrentTime = Mathf.Round(CurrentTime * 100f) / 100f;
    }

    private IEnumerator TimerDelay()
    {
        yield return new WaitForSeconds(_timerStartDelay);
        StartTimer();
    }
}