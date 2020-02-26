using Kalelovil.Revolution.Missions;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MissionProgress_UI : MonoBehaviour
{
    [SerializeField] MissionProgress _missionProgress;
    public MissionProgress MissionProgress { get => _missionProgress; set => SetMissionProgress(value); }
    private void SetMissionProgress(MissionProgress value)
    {
        _missionProgress = value;
        _missionNameText.text = $"{value.MissionType.Name}";
    }

    [Header("UI")]
    [SerializeField] TextMeshProUGUI _missionNameText;
    [SerializeField] TextMeshProUGUI _progressValueText;

    private void OnDisable()
    {
        _missionProgress.OnMissionProgress -= MissionProgresses;
        _missionProgress.OnMissionStarted -= MissionStarted;
    }
    private void OnEnable()
    {
        _missionProgress.OnMissionProgress += MissionProgresses;
        _missionProgress.OnMissionStarted += MissionStarted;
    }
    private void MissionProgresses(MissionProgress progress)
    {
        _progressValueText.text = $"{progress.Progress:P0}";
    }
    private void MissionStarted(MissionProgress progress)
    {
        _missionNameText.text = $"{progress.MissionType.Name}";
    }
}
