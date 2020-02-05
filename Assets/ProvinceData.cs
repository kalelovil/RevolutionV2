using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class ProvinceData : MonoBehaviour
{
    internal Province Province => _prov;

    Province _prov;

    [SerializeField] int _population;

    [Range(0f, 1f)]
    [SerializeField] float _localSupportFraction;

    internal List<Unit.ResourceQuantity> ResourceStockpileList { get => _resourceStockpileList; private set => _resourceStockpileList = value; }
    [SerializeField] List<Unit.ResourceQuantity> _resourceStockpileList;

    internal void Initialise(Province prov)
    {
        gameObject.name = $"{prov.name}";
        _prov = prov;

        SetDebugStats();
    }

    private void SetDebugStats()
    {
        _population = UnityEngine.Random.Range(5, 21);
        _localSupportFraction = UnityEngine.Random.Range(0f, 1f);

        int startingManpower = (int)(_localSupportFraction * _population);
        ResourceStockpileList.Find(x => x.Resource.Name == "Manpower").Add(startingManpower);
    }


    internal bool CanAfford(List<Unit.ResourceQuantity> cost)
    {
        foreach (var resource in cost)
        {
            switch (resource.Resource.ResourceScope)
            {
                case ResourceType.Scope.Country:
                {
                    if (!HeadquarterManager.Instance.ResourceStockpileList.Contains(resource) ||
                        HeadquarterManager.Instance.ResourceStockpileList.Find(x => x.Resource == resource.Resource).Quantity < resource.Quantity)
                    {
                        return false;
                    }
                    break;
                }
                case ResourceType.Scope.Province:
                {
                    if (!ResourceStockpileList.Contains(resource) ||
                        ResourceStockpileList.Find(x => x.Resource == resource.Resource).Quantity < resource.Quantity)
                    {
                        return false;
                    }
                    break;
                }
            }
        }
        return true;
    }
    internal void AddResources(List<Unit.ResourceQuantity> list)
    {
        foreach (var resource in list)
        {
            var toAddTo = GetStockpile(resource.Resource);
            toAddTo.Add(resource.Quantity);
        }
    }
    internal void SubtractResources(List<Unit.ResourceQuantity> list)
    {
        foreach (var resource in list)
        {
            var toAddTo = GetStockpile(resource.Resource);
            toAddTo.Add(resource.Quantity * -1);
        }
    }
    private Unit.ResourceQuantity GetStockpile(ResourceType resource)
    {
        Unit.ResourceQuantity toAddTo = null;
        switch (resource.ResourceScope)
        {
            case ResourceType.Scope.Country:
                toAddTo = HeadquarterManager.Instance.ResourceStockpileList.Find(_x => _x.Resource == resource);
                break;
            case ResourceType.Scope.Province:
                toAddTo = ResourceStockpileList.Find(_x => _x.Resource == resource);
                break;
            case ResourceType.Scope.City:
                throw new NotImplementedException();
                break;
            default:
                break;
        }
        return toAddTo;
    }
}