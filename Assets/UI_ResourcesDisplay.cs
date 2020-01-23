using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ResourcesDisplay : MonoBehaviour
{
    [SerializeField] UI_Resource_Panel _resourcePanelPrefab;
    [SerializeField] Transform _resourceListArea;

    [SerializeField] List<UI_Resource_Panel> _resourcePanelList = new List<UI_Resource_Panel>();

    // Start is called before the first frame update
    void Start()
    {
        foreach (var resourceType in ResourcesManager.Instance.ResourceNameToTypeMap.Values)
        {
            var panel = Instantiate(_resourcePanelPrefab, _resourceListArea);
            var resourceQuantity = new Unit.ResourceQuantity(resourceType, 0);
            panel.Initialise(resourceQuantity);
            _resourcePanelList.Add(panel);
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach(var resourcePanel in _resourcePanelList)
        {
            // TODO Replace Update with events and LINQ usages with maps
            var updateWith = HeadquarterManager.Instance._resourceStockpileList.Find(x => x.Resource == resourcePanel.Resource);
            resourcePanel.Update(updateWith.Quantity);
        }
    }
}
