using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WorldMapStrategyKit;

public class UI_ProvincePanel : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] ProvinceData _provinceData;
    public ProvinceData ProvinceData { get { return _provinceData; } internal set { ProvinceSelected(value); } }
    private void ProvinceSelected(ProvinceData value)
    {
        _provinceData = value;
        _provinceNameText.text = ProvinceData.Province.name;
    }

    [Header("UI")]
    [SerializeField] GameObject _visualsParentObject;
    [SerializeField] TextMeshProUGUI _provinceNameText;

    [SerializeField] private UI_Recruitment_Panel _recruitmentPanel;

    static UI_ProvincePanel _instance;
    public static UI_ProvincePanel Instance => _instance;

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void OpenRecruitmentPanel()
    {
        _visualsParentObject.SetActive(true);
    }

    internal void RecruitUnit(Unit unitType)
    {
        Unit unitPrefab = Recruitment_Manager.Instance.GetUnitTypePrefab(unitType);
        var cost = unitPrefab.CostList;
        bool canAfford = ProvinceData.CanAfford(cost);
        if (canAfford)
        {
            ProvinceData.SubtractResources(cost);

            var parent = WMSK.instance.gameObject.transform;

            Unit spawnedUnit = Instantiate(unitPrefab, parent);
            spawnedUnit.Initialise(ProvinceData.Province.center);}
    }
}
