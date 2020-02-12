using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_MainInterface : MonoBehaviour
{
    static UI_MainInterface _instance;
    public static UI_MainInterface Instance => _instance;

    public UI_ProvincePanel ProvincePanel => _leftInterfacePanels.Find(e => e is UI_ProvincePanel) as UI_ProvincePanel;
    public UI_UnitPanel UnitPanel => _leftInterfacePanels.Find(e => e is UI_UnitPanel) as UI_UnitPanel;

    [SerializeField] List<UI_AbstractInterfacePanel> _leftInterfacePanels;

    [SerializeField] UI_ResourcesDisplay _resourcesDisplay;

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
    }
    void Start()
    {
        
    }

    public void OpenProvincePanel(ProvinceData province)
    {
        ClosePanels();
        UI_ProvincePanel panel = ProvincePanel;
        panel.ProvinceData = province;
        panel.Open();
    }

    public void OpenUnitPanel(UnitScript unit)
    {
        ClosePanels();
        UI_UnitPanel panel = UnitPanel;
        panel.Unit = unit;
        panel.Open();
    }

    public void ClosePanels()
    {
        foreach (var panel in _leftInterfacePanels)
        {
            panel.Close();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
