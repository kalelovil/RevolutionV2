using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using WorldMapStrategyKit;

public class UI_ProvincePanel : UI_AbstractInterfacePanel
{
    [Header("Data")]
    [SerializeField] Province_Data _provinceData;
    public Province_Data ProvinceData { get { return _provinceData; } internal set { ProvinceSelected(value); } }
    private void ProvinceSelected(Province_Data value)
    {
        _provinceData = value;
        _nameText.text = ProvinceData.Province.name;
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
