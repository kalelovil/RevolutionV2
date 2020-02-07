using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class DateManager : MonoBehaviour
{
    public static DateManager Instance { get; internal set; }

    public DateTime CurrentDate { get { return _currentDate; } set { SetCurrentDate(value); } }
    private void SetCurrentDate(DateTime newDate)
    {
        if (newDate.Hour != _currentDate.Hour) CurrentHourChangedAction?.Invoke(newDate);
        if (newDate.Day != _currentDate.Day) CurrentDayChangedAction?.Invoke(newDate);
        if (newDate.Month != _currentDate.Month) CurrentMonthChangedAction?.Invoke(newDate);
        if (newDate.Year != _currentDate.Year) CurrentYearChangedAction?.Invoke(newDate);

        _currentDate = newDate;

        _currentDateSerialized = _currentDate.ToLongTimeString();
    }
    [SerializeField] DateTime _currentDate;
    [SerializeField] string _currentDateSerialized;
    readonly CultureInfo Provider = CultureInfo.InvariantCulture;

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
        0.05f,
        0.025f,
        0.001f,
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
        CurrentDate = DateTime.ParseExact(_currentDateSerialized, "d", Provider);
    }

    float _currentHourProgress = 0f;
    private void Update()
    {
        HandleInputs();

        _currentHourProgress += Time.deltaTime;
        if (_currentHourProgress >= SPEED_TO_SECONDS_PER_DAY[_speedIndex])
        {
            CurrentDate = CurrentDate.AddHours(1);
            TimeOfLastHourUpdate = Time.time;
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
