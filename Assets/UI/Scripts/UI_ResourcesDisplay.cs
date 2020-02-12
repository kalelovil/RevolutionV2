﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
            var resourceQuantity = new UnitScript.ResourceQuantity(resourceType, 0);
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
            List<UnitScript.ResourceQuantity> updateWith = new List<UnitScript.ResourceQuantity>();
            switch (resourcePanel.Resource.ResourceScope)
            {
                case ResourceType.Scope.Country:
                    updateWith.Add(HeadquarterManager.Instance.ResourceStockpileList.Find(x => x.Resource == resourcePanel.Resource));
                    break;
                case ResourceType.Scope.Province:
                    foreach (var prov in Province_Manager.Instance.ProvinceList)
                    {
                        updateWith.Add(prov.ResourceStockpileList.Find(x => x.Resource == resourcePanel.Resource));
                    }
                    break;
                case ResourceType.Scope.City:
                    break;
                default:
                    break;
            }
            resourcePanel.Update(updateWith.Sum(_x => _x.Quantity));
        }
    }
}