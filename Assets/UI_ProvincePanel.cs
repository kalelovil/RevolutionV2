using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ProvincePanel : MonoBehaviour
{
    static UI_ProvincePanel _instance;
    public static UI_ProvincePanel Instance => _instance;

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
}
