﻿using Kalelovil.Revolution.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalelovil.Revolution.Units
{
    public class Unit_Manager : MonoBehaviour
    {
        #region Sub Managers
        [SerializeField] internal Recruitment_Manager _recruitmentManager;
        [SerializeField] internal Leader_Pool _leaderPool;
        #endregion

        #region Selected Unit
        Brigade _selectedUnit;
        internal Brigade SelectedUnit { get { return _selectedUnit; } set { SetSelectedUnit(value); } }
        private void SetSelectedUnit(Brigade value)
        {
            _selectedUnit = value;
            if (_selectedUnit)
            {
                UI_MainInterface.Instance.OpenUnitPanel(_selectedUnit);

                _unitSelectionCircle.transform.SetParent(_selectedUnit.transform, false);
                _unitSelectionCircle.gameObject.SetActive(true);
            }
            else
            {
                UI_MainInterface.Instance.ClosePanels();

                _unitSelectionCircle.transform.SetParent(transform, false);
                _unitSelectionCircle.gameObject.SetActive(false);
            }
        }
        [SerializeField] GameObject _unitSelectionCircle;
        #endregion

        #region Singleton Reference
        static Unit_Manager _instance;
        public static Unit_Manager Instance => _instance;
        #endregion

        void Awake()
        {
            _instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}