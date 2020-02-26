using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Kalelovil.Revolution.UI
{
    internal class UI_Recruitment_Leader_Bar : MonoBehaviour
    {
        [Header("Leader")]
        [SerializeField] BrigadeLeader _leader;
        [Space(10, order = 0)]

        [Header("Display", order = 1)]
        [Header("Name", order = 2)]
        [SerializeField] TextMeshProUGUI _nameText;

        [Header("Costs")]
        [SerializeField] Transform _resourceCostPanelArea;
        [SerializeField] UI_Resource_Panel _resourceCostPanelPrefab;
        [SerializeField] List<UI_Resource_Panel> _resourceCostPanels;

        [Header("Stats")]
        [SerializeField] TextMeshProUGUI _authorityValueText;
        [SerializeField] TextMeshProUGUI _cunningValueText;
        [SerializeField] TextMeshProUGUI _managementValueText;


        internal void Initialise(BrigadeLeader leaderDefinition)
        {
            _leader = leaderDefinition;

            // Set Unit Type Name Display
            _nameText.text = $"{leaderDefinition.name.Replace("_", " ")}";

            // Set Unit Costs Display
            foreach (var cost in leaderDefinition.CostList)
            {
                UI_Resource_Panel resourcePanel = Instantiate(_resourceCostPanelPrefab, _resourceCostPanelArea);
                resourcePanel.Initialise(cost);
            }

            // Set Unit Stats Display
            _authorityValueText.text = $"{leaderDefinition.Authority}";
            _cunningValueText.text = $"{leaderDefinition.Cunning}";
            _managementValueText.text = $"{leaderDefinition.Management}";
        }

        public void Clicked()
        {
            Debug.Log($"Unit ({_leader.name}) Bar Clicked");
            UI_MainInterface.Instance._agentRecruitmentPanel.RecruitBrigade(_leader);
            UI_MainInterface.Instance.CloseLeaderRecruitmentPanel();
        }
    }
}