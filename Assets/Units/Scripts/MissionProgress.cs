using System;
using UnityEngine;

namespace Kalelovil.Revolution.Missions
{
    public class MissionProgress : MonoBehaviour
    {
        static readonly float PROGRESS_PER_DAY = 0.01f;
        [Range(0f, 1f)]
        [SerializeField] float _progress = 0f;
        public float Progress { get => _progress; set => SetProgress(value); }
        private void SetProgress(float value)
        {
            _progress = Mathf.Clamp(value, 0f, 1f);
            OnMissionProgress?.Invoke(this);
            if (_progress == 1f)
            {
                OnMissionFinished?.Invoke(this);
            }
        }

        [SerializeField] MissionType _missionType;
        public MissionType MissionType { get => _missionType; set => _missionType = value; }

        public delegate void MissionEvent(MissionProgress mission);
        public event MissionEvent OnMissionStarted;
        public event MissionEvent OnMissionResumed;
        public event MissionEvent OnMissionProgress;
        public event MissionEvent OnMissionStopped;
        public event MissionEvent OnMissionFinished;

        private void OnEnable()
        {
            DateManager.CurrentDayChangedAction += CurrentDayChanged;
            OnMissionFinished += MissionFinished;

            OnMissionResumed?.Invoke(this);
        }

        private void Start()
        {
            OnMissionStarted?.Invoke(this);
        }
        private void OnDisable()
        {
            DateManager.CurrentDayChangedAction -= CurrentDayChanged;
            OnMissionStopped.Invoke(this);
        }

        private void CurrentDayChanged(DateTime obj)
        {
            Progress += PROGRESS_PER_DAY;
        }

        private void MissionFinished(MissionProgress mission)
        {
            Debug.Log("Mission Finished");
        }


    }
}