using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WorldMapStrategyKit;

public class UI_ProvincePanel : UI_AbstractInterfacePanel
{
    [Header("Data")]
    [SerializeField] ProvinceData _provinceData;
    public ProvinceData ProvinceData { get { return _provinceData; } internal set { ProvinceSelected(value); } }
    private void ProvinceSelected(ProvinceData value)
    {
        _provinceData = value;
        _nameText.text = ProvinceData.Province.name;
    }

    [Header("UI")]
    [SerializeField] private UI_LeaderRecruitment_Panel _leaderRecruitmentPanel;
    [SerializeField] private UI_UnitRecruitment_Panel _unitRecruitmentPanel;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void RecruitUnit(BrigadeElement unitType)
    {
        BrigadeElement unitPrefab = Unit_Manager.Instance._recruitmentManager.GetUnitTypePrefab(unitType);
        var cost = unitPrefab.CostList;
        bool canAfford = ProvinceData.CanAfford(cost);
        if (canAfford)
        {
            ProvinceData.SubtractResources(cost);

            var parent = WMSK.instance.gameObject.transform;

            BrigadeElement spawnedUnit = Instantiate(unitPrefab);
            spawnedUnit.Initialise(ProvinceData.Province.center);}
    }

    internal void RecruitBrigade(BrigadeLeader leader)
    {
        throw new NotImplementedException();
    }
}
