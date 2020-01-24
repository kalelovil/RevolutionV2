using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class UI_ProvincePanel : MonoBehaviour
{
    static UI_ProvincePanel _instance;
    public static UI_ProvincePanel Instance => _instance;

    public ProvinceData ProvinceData { get; internal set; }

    [SerializeField] private UI_Recruitment_Panel _recruitmentPanel;

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
        _recruitmentPanel.gameObject.SetActive(true);
    }

    internal void RecruitUnit(Unit unitType)
    {
        Unit unitPrefab = Recruitment_Manager.Instance.GetUnitTypePrefab(unitType);
        var cost = unitPrefab.CostList;
        bool canAfford = ProvinceData.CanAfford(cost);
        if (canAfford)
        {
            HeadquarterManager.Instance.SubtractResources(cost);

            var parent = WMSK.instance.gameObject.transform;
            Unit spawnedUnit = Instantiate(unitPrefab, parent);
            spawnedUnit.transform.localPosition = ProvinceData.Province.center;
            spawnedUnit.transform.localScale = new Vector3(spawnedUnit.transform.localScale.x / WMSK.instance.transform.localScale.x,
                                                        spawnedUnit.transform.localScale.y / WMSK.instance.transform.localScale.y,
                                                        spawnedUnit.transform.localScale.z / WMSK.instance.transform.localScale.z);
        }
    }
}
