using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LeaderRecruitment_Panel : MonoBehaviour
{
    [Header("Instantiation")]
    [SerializeField] Transform _leaderBarArea;
    [SerializeField] UI_Recruitment_Unit_Bar _leaderBarPrefab;

    [Header("Game Logic")]
    [SerializeField] UI_Recruitment_Unit_Bar _selectedUnitBar;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var unitDefinition in Unit_Manager.Instance._recruitmentManager.UnitDefinitionsList)
        {
            if (unitDefinition.CostList.Count > 0)
            {
                AddUnitBar(unitDefinition);
            }
        }
    }

    private void AddLeaderBar(UnitScript unitDefinition)
    {
        UI_Recruitment_Leader_Bar leaderBar = Instantiate(_leaderBarPrefab, _leaderBarArea);
        leaderBar.Initialise(unitDefinition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
