using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Recruitment_Panel : MonoBehaviour
{
    [Header("Instantiation")]
    [SerializeField] Transform _unitBarArea;
    [SerializeField] UI_Recruitment_Unit_Bar _unitBarPrefab;

    [Header("Game Logic")]
    [SerializeField] UI_Recruitment_Unit_Bar _selectedUnitBar;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var unitDefinition in Recruitment_Manager.Instance.UnitDefinitionsList)
        {
            AddUnitBar(unitDefinition);
        }
    }

    private void AddUnitBar(Unit unitDefinition)
    {
        UI_Recruitment_Unit_Bar unitBar = Instantiate(_unitBarPrefab, _unitBarArea);
        unitBar.Initialise(unitDefinition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
