using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Time_Panel : MonoBehaviour
{
    [Header("Day")]
    [SerializeField] TextMeshProUGUI _dayValueText;

    [Header("Hour")]
    [SerializeField] TextMeshProUGUI _hourValueText;

    [Header("Speed")]
    [SerializeField] TextMeshProUGUI _speedValueText;

    private void OnEnable()
    {
        DateManager.CurrentHourChangedAction += CurrentHourChanged;
        DateManager.CurrentDayChangedAction += CurrentDayChanged;

        DateManager.CurrentSpeedChangedAction += CurrentSpeedChanged;
    }

    private void OnDisable()
    {
        DateManager.CurrentHourChangedAction -= CurrentHourChanged;
        DateManager.CurrentDayChangedAction -= CurrentDayChanged;

        DateManager.CurrentSpeedChangedAction -= CurrentSpeedChanged;
    }

    private void CurrentHourChanged(int currentHour)
    {
        _hourValueText.text = $"{currentHour}";
    }
    private void CurrentDayChanged(int currentDay)
    {
        _dayValueText.text = $"{currentDay}";
    }

    private void CurrentSpeedChanged(int currentSpeedIndex)
    {
        _speedValueText.text = $"{currentSpeedIndex}";
    }
}
