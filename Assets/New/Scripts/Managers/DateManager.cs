using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DateManager : MonoBehaviour
{
    public static DateManager Instance { get; internal set; }

    #region Hour
    [Header("Hour")]
    [SerializeField] int _currentHourNum;
    public int CurrentHourNum { get { return _currentHourNum; } }
    float TimeOfLastHourUpdate = 0f;
    internal static Action<int> CurrentHourChangedAction;
    #endregion

    #region Day
    [Header("Day")]
    [SerializeField] int _currentDayNum;
    public int CurrentDayNum { get { return _currentDayNum; } }
    internal static Action<int> CurrentDayChangedAction;
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
        CurrentHourChangedAction?.Invoke(0);
        CurrentDayChangedAction?.Invoke(1);
    }

    float _currentHourProgress = 0f;
    private void Update()
    {
        HandleInputs();

        _currentHourProgress += Time.deltaTime;
        if (_currentHourProgress >= SPEED_TO_SECONDS_PER_DAY[_speedIndex])
        {
            _currentHourNum++;
            TimeOfLastHourUpdate = Time.time;
            if (CurrentHourNum % 24 == 0)
            {
                _currentDayNum++;
                _currentHourNum = 0;
                CurrentDayChangedAction.Invoke(_currentDayNum);
            }
            CurrentHourChangedAction?.Invoke(_currentHourNum);
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
