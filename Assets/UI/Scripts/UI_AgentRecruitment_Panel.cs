using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class UI_AgentRecruitment_Panel : MonoBehaviour
{
    [Header("Instantiation")]
    [SerializeField] Transform _leaderBarArea;
    [SerializeField] UI_Recruitment_Leader_Bar _leaderBarPrefab;

    [Header("Game Logic")]
    [SerializeField] UI_Recruitment_Leader_Bar _selectedUnitBar;

    #region Agent Type
    [Header("Agent Type")]
    Brigade.AgentType _agentType;
    public Brigade.AgentType AgentType { get { return _agentType; } internal set { SetAgentType(value); } }
    private void SetAgentType(Brigade.AgentType value)
    {
        _agentType = value;
        Populate();
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {

    }

    private void Populate()
    {
        foreach (var leaderInPool in Unit_Manager.Instance._leaderPool.LeaderPool)
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

    internal void RecruitBrigade(BrigadeLeader leader)
    {
        Brigade brigadePrefab = Unit_Manager.Instance._recruitmentManager.BrigadePrefab;
        var cost = new List<ResourceQuantity>();//brigadePrefab.CostList;
        //bool canAfford = ProvinceData.CanAfford(cost);
        //if (canAfford)
        {
            //ProvinceData.SubtractResources(cost);

            var parent = WMSK.instance.gameObject.transform;

            Brigade spawnedUnit = Instantiate(brigadePrefab);
            spawnedUnit.Initialise(leader, HeadquarterManager.Instance.HQList[0]);
        }
    }
}
