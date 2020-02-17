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

    #region UI
    [Header("UI")]
    [SerializeField] internal UI_AgentRecruitment_Panel _agentRecruitmentPanel;
    [SerializeField] internal UI_ElementRecruitment_Panel _elementRecruitmentPanel;

    [SerializeField] internal UI_Agent_Outliner _agentOutliner;
    #endregion

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

    public void OpenUnitPanel(Brigade unit)
    {
        ClosePanels();
        UI_UnitPanel panel = UnitPanel;
        panel.Unit = unit;
        panel.Open();
    }

    internal void OpenLeaderRecruitmentPanel(Brigade.AgentType agentType, Transform attachToTransform)
    {
        _agentRecruitmentPanel.transform.SetParent(attachToTransform, false);
        _agentRecruitmentPanel.gameObject.SetActive(true);
        _agentRecruitmentPanel.AgentType = agentType;
    }
    internal void CloseLeaderRecruitmentPanel()
    {
        _agentRecruitmentPanel.transform.SetParent(transform, false);
        _agentRecruitmentPanel.gameObject.SetActive(false);
        //_agentRecruitmentPanel.AgentType = agentType;
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
