using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Recruitment_Unit_Bar : MonoBehaviour
{
    [Header("Unit")]
    [SerializeField] Unit _unit;
    [Space(10)]

    [Header("Display")]
    [Header("Name")]
    [SerializeField] TextMeshProUGUI _nameText;
    [Header("Resource Costs")]
    [SerializeField] Transform _resourceCostPanelArea;
    [SerializeField] UI_Resource_Panel _resourceCostPanelPrefab;
    [SerializeField] List<UI_Resource_Panel> _resourceCostPanels;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
