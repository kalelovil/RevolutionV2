using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateManager : MonoBehaviour
{
    public static DateManager Instance { get; internal set; }

    [SerializeField] DateTime _currentDate;
    #region Hour
    [Header("Hour")]
    float TimeOfLastHourUpdate = 0f;
    internal static Action<DateTime> CurrentHourChangedAction;
    #endregion

    #region Day
    [Header("Day")]
    internal static Action<DateTime> CurrentDayChangedAction;
    #endregion

    #region Month
    [Header("Month")]
    internal static Action<DateTime> CurrentMonthChangedAction;
    #endregion

    #region Year
    [Header("Year")]
    internal static Action<DateTime> CurrentYearChangedAction;
    #endregion

    static List<float> SPEED_TO_SECONDS_PER_DAY = new List<float>
    {
        2f,
        1f,
        0.5f,
        0.25f,
        0.1f,
    };
    int SpeedIndex { get { return _speedIndex; } set { _speedIndex = value; CurrentSpeedChangedAction?.Invoke(SpeedIndex); } }
    [SerializeField] int _speedIndex = 1;
    internal float CurrentSecondsPerDay => SPEED_TO_SECONDS_PER_DAY[_speedIndex];
    internal static Action<int> CurrentSpeedChangedAction;

    internal float CurrentFrameAsFractionOfHourStep()
    {
        float timeSinceLastHourStep = Time.time - TimeOfLastHourUpdate;
        float frameAsFractionOfHourStep = timeSinceLastHourStep / CurrentSecondsPerDay;
        return frameAsFractionOfHourStep;
    }

    void Awake()
    {
        Instance = this;
    }

    private IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();
        CurrentHourChangedAction?.Invoke(new DateTime());
        CurrentDayChangedAction?.Invoke(new DateTime(1));
    }

    float _currentHourProgress = 0f;
    private void Update()
    {
        HandleInputs();

        _currentHourProgress += Time.deltaTime;
        if (_currentHourProgress >= SPEED_TO_SECONDS_PER_DAY[_speedIndex])
        {
            _currentDate = _currentDate.AddHours(1);
            TimeOfLastHourUpdate = Time.time;

            CurrentHourChangedAction?.Invoke(_currentDate);
            if (_currentDate.Hour == 0)
            {
                CurrentDayChangedAction?.Invoke(_currentDate);
                if (_currentDate.Day == 0)
                {
                    CurrentMonthChangedAction?.Invoke(_currentDate);
                    if (_currentDate.Month == 0)
                    {
                        CurrentYearChangedAction?.Invoke(_currentDate);
                    }
                }
            }
            _currentHourProgress = 0f;
        }
    }

    private void HandleInputs()
    {
        /*
        if (Input.anyKey)
        {
            Debug.Log($"Key Down");
        }
        */
        if (Input.GetKeyDown(KeyCode.KeypadPlus) || Input.GetKeyDown(KeyCode.Equals) || Input.GetKeyDown(KeyCode.Plus))
        {
            if (_speedIndex < SPEED_TO_SECONDS_PER_DAY.Count - 1) SpeedIndex++;
        }
        else if (Input.GetKeyDown(KeyCode.KeypadMinus) || Input.GetKeyDown(KeyCode.Minus))
        {
            if (_speedIndex > 0) SpeedIndex--;
        }
    }
}
