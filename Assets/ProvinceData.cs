using System;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

internal class ProvinceData : MonoBehaviour
{
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
}