using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_LeaderRecruitment_Panel : MonoBehaviour
{
    [Header("Instantiation")]
    [SerializeField] Transform _leaderBarArea;
    [SerializeField] UI_Recruitment_Leader_Bar _leaderBarPrefab;

    [Header("Game Logic")]
    [SerializeField] UI_Recruitment_Leader_Bar _selectedUnitBar;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var leaderInPool in UI_MainInterface.Instance.ProvincePanel.ProvinceData.LeaderPool)
        {
            AddLeaderBar(leaderInPool);
        }
    }

    private void AddLeaderBar(BrigadeLeader leaderDefinition)
    {
        UI_Recruitment_Leader_Bar leaderBar = Instantiate(_leaderBarPrefab, _leaderBarArea);
        leaderBar.Initialise(leaderDefinition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
