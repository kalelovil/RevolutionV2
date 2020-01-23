using System;
using UnityEngine;
using WorldMapStrategyKit;

internal class ProvinceData : MonoBehaviour
{
    Province _prov;

    [Range(0f, 1f)]
    [SerializeField] float _localSupportFraction;

    const int Max_Manpower = 20;
    [Range(0, Max_Manpower)]
    [SerializeField] int _manpower;
    public int Manpower { get => _manpower; set => _manpower = value; }

    internal void Initialise(Province prov)
    {
        _prov = prov;

        SetDebugSupport();
    }

    private void SetDebugSupport()
    {
        _localSupportFraction = UnityEngine.Random.Range(0f, 1f);

        Manpower = (int)(Max_Manpower * _localSupportFraction);
    }
}