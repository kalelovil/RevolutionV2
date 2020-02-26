using System;
using UnityEngine;

namespace Kalelovil.Revolution.Missions
{
    public class MissionProgress : MonoBehaviour
    {
        static readonly float PROGRESS_PER_DAY = 0.01f;
        [Range(0f, 1f)]
        float _progress;

        public delegate void MissionEvent(MissionProgress mission);
        public event MissionEvent OnMissionStarted;
        public event MissionEvent OnMissionEnded;

        private void OnEnable()
        {
            DateManager.CurrentDayChangedAction += CurrentDayChanged;
        }
        private void OnDisable()
        {
            DateManager.CurrentDayChangedAction -= CurrentDayChanged;
        }

        private void CurrentDayChanged(DateTime obj)
        {
            _progress += PROGRESS_PER_DAY;
        }
    }
}