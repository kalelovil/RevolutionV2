using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AddAgentButton : MonoBehaviour
{
    [SerializeField] UI_Agent_Outliner.AgentType _agentType;

    public void Clicked()
    {
        UI_MainInterface.Instance.OpenLeaderRecruitmentPanel(_agentType, transform);
    }
}
