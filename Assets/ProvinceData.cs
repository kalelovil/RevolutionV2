using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class ProvinceData : MonoBehaviour
{
    internal Province Province => _prov;
    Province _prov;

    [Range(0f, 1f)]
    [SerializeField] float _localSupportFraction;

    [SerializeField] List<Unit.ResourceQuantity> _resourceStockpileList;

    internal void Initialise(Province prov)
    {
        gameObject.name = $"{prov.name}";
        _prov = prov;

        SetDebugSupport();
    }

    private void SetDebugSupport()
    {
        _localSupportFraction = UnityEngine.Random.Range(0f, 1f);

        int startingManpower = (int)(_localSupportFraction * 20f);
        _resourceStockpileList.Find(x => x.Resource.Name == "Manpower").Add(startingManpower);
    }


    internal bool CanAfford(List<Unit.ResourceQuantity> cost)
    {
        foreach (var resource in cost)
        {
            switch (resource.Resource.ResourceScope)
            {
                case ResourceType.Scope.Country:
                {
                    if (!HeadquarterManager.Instance._resourceStockpileList.Contains(resource) ||
                        HeadquarterManager.Instance._resourceStockpileList.Find(x => x.Resource == resource.Resource).Quantity < resource.Quantity)
                    {
                        return false;
                    }
                    break;
                }
                case ResourceType.Scope.Province:
                {
                    if (!_resourceStockpileList.Contains(resource) ||
                        _resourceStockpileList.Find(x => x.Resource == resource.Resource).Quantity < resource.Quantity)
                    {
                        return false;
                    }
                    break;
                }
            }
        }
        return true;
    }
}