using System;
using System.Collections;
using UnityEngine;
using WorldMapStrategyKit;

public class Headquarters : MonoBehaviour
{
    MountPoint _mountPoint;

    WMSK Map => WMSK.instance;
    ProvinceData Province => Province_Manager.Instance.ProvinceList[_mountPoint.provinceIndex];

    public void Initialise(MountPoint mountPoint)
    {
        _mountPoint = mountPoint;

        StartCoroutine(InitialiseCoroutine());
    }

    private IEnumerator InitialiseCoroutine()
    {
        yield return null;
        Province.Population = UnityEngine.Random.Range(5, 21);
        Province.LocalSupportFraction = UnityEngine.Random.Range(0f, 0.5f);
    }
}