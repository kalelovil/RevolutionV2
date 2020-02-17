using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;
using Random = UnityEngine.Random;

public class ProvinceData : MonoBehaviour
{
    WMSK map => WMSK.instance;

    internal Province Province => _prov;

    Province _prov;

    [SerializeField] int _population;
    public int Population { get => _population; set => SetPopulation(value); }
    private void SetPopulation(int value)
    {
        _population = value;

        SetManpower();
    }

    #region Local_Support
    [Header("Local Support")]
    [Range(0f, 1f)]
    [SerializeField] float _localSupportFraction;
    public float LocalSupportFraction { get => _localSupportFraction; set => SetLocalSupportFraction(value); }
    private void SetLocalSupportFraction(float value)
    {
        _localSupportFraction = Mathf.Clamp(value, 0f, 1f);

        var provinceIndex = map.GetProvinceIndex(Province);
        /*
        Color color = new Color(0f, _localSupportFraction, 0f, 0.5f);
        map.ToggleProvinceSurface(provinceIndex, true, color);
        map.BlinkProvince(provinceIndex, Color.green, Color.grey, 0.8f, 0.2f);
        */

        SetManpower();
    }
    #endregion

    private void SetManpower()
    {
        int startingManpower = (int)(LocalSupportFraction * _population);
        ResourceStockpileList.Find(x => x.Resource.Name == "Manpower").Set(startingManpower);
    }

    internal List<ResourceQuantity> ResourceStockpileList { get => _resourceStockpileList; private set => _resourceStockpileList = value; }

    [SerializeField] List<ResourceQuantity> _resourceStockpileList;

    internal void Initialise(Province prov)
    {
        gameObject.name = $"{prov.name}";
        _prov = prov;

        SetDebugStats();
    }

    private void SetDebugStats()
    {
        _population = UnityEngine.Random.Range(5, 21);
        //LocalSupportFraction = UnityEngine.Random.Range(0f, 0.5f);
    }


    internal bool CanAfford(List<ResourceQuantity> cost)
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
    internal void AddResources(List<ResourceQuantity> list)
    {
        foreach (var resource in list)
        {
            var toAddTo = GetStockpile(resource.Resource);
            toAddTo.Add(resource.Quantity);
        }
    }
    internal void SubtractResources(List<ResourceQuantity> list)
    {
        foreach (var resource in list)
        {
            var toAddTo = GetStockpile(resource.Resource);
            toAddTo.Add(resource.Quantity * -1);
        }
    }
    private ResourceQuantity GetStockpile(ResourceType resource)
    {
        ResourceQuantity toAddTo = null;
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