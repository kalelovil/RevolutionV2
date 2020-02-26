using Kalelovil.Revolution.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

namespace Kalelovil.Revolution.UI
{
    public class UI_ElementRecruitment_Panel : MonoBehaviour
    {
        [Header("Instantiation")]
        [SerializeField] Transform _unitBarArea;
        [SerializeField] UI_Recruitment_Unit_Bar _unitBarPrefab;

        [Header("Game Logic")]
        [SerializeField] UI_Recruitment_Unit_Bar _selectedUnitBar;

        // Start is called before the first frame update
        void Start()
        {
            foreach (var unitDefinition in Unit_Manager.Instance._recruitmentManager.ElementDefinitionsList)
            {
                if (unitDefinition.CostList.Count > 0)
                {
                    AddUnitBar(unitDefinition);
                }
            }
        }

        private void AddUnitBar(BrigadeElement unitDefinition)
        {
            UI_Recruitment_Unit_Bar unitBar = Instantiate(_unitBarPrefab, _unitBarArea);
            unitBar.Initialise(unitDefinition);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}