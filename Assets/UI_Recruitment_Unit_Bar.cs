using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class UI_Recruitment_Unit_Bar : MonoBehaviour
{
    [Header("Unit")]
    [SerializeField] Unit _unit;
    [Space(10, order = 0)]

    [Header("Display", order = 1)]
    [Header("Name", order = 2)]
    [SerializeField] TextMeshProUGUI _nameText;

    [Header("Costs")]
    [SerializeField] Transform _resourceCostPanelArea;
    [SerializeField] UI_Resource_Panel _resourceCostPanelPrefab;
    [SerializeField] List<UI_Resource_Panel> _resourceCostPanels;

    [Header("Stats")]
    [SerializeField] TextMeshProUGUI _attackValueText;
    [SerializeField] TextMeshProUGUI _healthValueText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Initialise(Unit unitDefinition)
    {
        _unit = unitDefinition;

        // Set Unit Type Name Display
        _nameText.text = $"{unitDefinition.name.Replace("_", " ")}";
        
        // Set Unit Costs Display
        foreach (var cost in unitDefinition.CostList)
        {
            UI_Resource_Panel resourcePanel = Instantiate(_resourceCostPanelPrefab, _resourceCostPanelArea);
            resourcePanel.Initialise(cost);
        }

        // Set Unit Stats Display
        _attackValueText.text = $"{unitDefinition.Attack}";
        _healthValueText.text = $"{unitDefinition.Health}";
    }

    public void Clicked()
    {
        Debug.Log($"Unit ({_unit.name}) Bar Clicked");
        UI_MainInterface.Instance.ProvincePanel.RecruitUnit(_unit);
    }
}
