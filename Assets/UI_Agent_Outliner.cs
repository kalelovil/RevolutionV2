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
}
