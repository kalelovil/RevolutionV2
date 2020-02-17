using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_AddAgentButton : MonoBehaviour
{
    [SerializeField] Brigade.AgentType _agentType;

    public void Clicked()
    {
        UI_MainInterface.Instance.OpenLeaderRecruitmentPanel(_agentType, transform);
    }
}
