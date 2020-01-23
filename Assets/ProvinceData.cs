using System;
using UnityEngine;
using WorldMapStrategyKit;

internal class ProvinceData : MonoBehaviour
{
    Province _prov;

    [Range(0f, 1f)]
    [SerializeField] float _localSupportFraction;

    internal void Initialise(Province prov)
    {
        _prov = prov;

        SetDebugSupport();
    }

    private void SetDebugSupport()
    {
        _localSupportFraction = UnityEngine.Random.Range(0f, 1f);
    }
}