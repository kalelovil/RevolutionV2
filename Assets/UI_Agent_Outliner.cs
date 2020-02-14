using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Agent_Outliner : MonoBehaviour
{
    [SerializeField] UI_BrigadeLeader_Icon _agentIconPrefab;
    [SerializeField] Transform _agentIconArea;
    [SerializeField] List<UI_BrigadeLeader_Icon> _agentIconList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum AgentType
    {
        Brigade,
        Saboteur,
        Diplomat,
    }

    public void AddBrigadeButtonClicked() => AddAgentButtonClicked(AgentType.Brigade);
    public void AddSaboteurButtonClicked() => AddAgentButtonClicked(AgentType.Saboteur);
    public void AddDiplomatButtonClicked() => AddAgentButtonClicked(AgentType.Diplomat);
    void AddAgentButtonClicked(AgentType type)
    {
        Debug.Log("Add Agent Button Click");
        switch (type)
        {
            case AgentType.Brigade:
                break;
            case AgentType.Saboteur:
                break;
            case AgentType.Diplomat:
                break;
            default:
                break;
        }
    }
}
