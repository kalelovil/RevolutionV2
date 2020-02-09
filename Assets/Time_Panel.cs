using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Time_Panel : MonoBehaviour
{
    [Header("Hour")]
    [SerializeField] TextMeshProUGUI _hourValueText;

    [Header("Day")]
    [SerializeField] TextMeshProUGUI _dayValueText;

    [Header("Month")]
    [SerializeField] TextMeshProUGUI _monthValueText;

    [Header("Year")]
    [SerializeField] TextMeshProUGUI _yearValueText;

    [Header("Speed")]
    [SerializeField] TextMeshProUGUI _speedValueText;

    private void OnEnable()
    {
        DateManager.CurrentHourChangedAction += CurrentHourChanged;
        DateManager.CurrentDayChangedAction += CurrentDayChanged;
        DateManager.CurrentMonthChangedAction += CurrentMonthChanged;
        DateManager.CurrentYearChangedAction += CurrentYearChanged;

        DateManager.CurrentSpeedChangedAction += CurrentSpeedChanged;
    }

    private void OnDisable()
    {
        DateManager.CurrentHourChangedAction -= CurrentHourChanged;
        DateManager.CurrentDayChangedAction -= CurrentDayChanged;
        DateManager.CurrentMonthChangedAction -= CurrentMonthChanged;
        DateManager.CurrentYearChangedAction -= CurrentYearChanged;

        DateManager.CurrentSpeedChangedAction -= CurrentSpeedChanged;
    }


    private void CurrentHourChanged(DateTime date)
    {
        _hourValueText.text = $"{date:HH:mm}";
        //Debug.Log($"Hour Changed, New Date: {date}");
    }
    private void CurrentDayChanged(DateTime date)
    {
        _dayValueText.text = $"{date:%d}";
        Debug.Log($"Day Changed, New Date: {date}");
    }
    private void CurrentMonthChanged(DateTime date)
    {
        _monthValueText.text = $"{date:MMMM}".Substring(0, 3) + ".";
        Debug.Log($"Month Changed, New Date: {date}");
    }
    private void CurrentYearChanged(DateTime date)
    {
        _yearValueText.text = $"{date:yyyy}";
        Debug.Log($"Year Changed, New Date: {date}");
    }

    private void CurrentSpeedChanged(int currentSpeedIndex)
    {
        _speedValueText.text = $"{currentSpeedIndex}";
    }
}
