using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class UI_ProvincePanel : MonoBehaviour
{
    static UI_ProvincePanel _instance;
    public static UI_ProvincePanel Instance => _instance;

    public Province Province { get; internal set; }

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
        Unit spawnedUnit = Instantiate(unitPrefab, Province.center, unitPrefab.transform.rotation);
    }
}
