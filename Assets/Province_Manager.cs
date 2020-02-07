using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class Province_Manager : MonoBehaviour
{
    [SerializeField] ProvinceData _provinceDataPrefab;

    public List<ProvinceData> ProvinceList { get => _provinceList; private set => _provinceList = value; }
    [SerializeField] List<ProvinceData> _provinceList;

    public static Province_Manager Instance => _instance;


    static Province_Manager _instance;

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;

        StartCoroutine(BuildProvinceList());
    }

    private IEnumerator BuildProvinceList()
    {
        // TODO Make these non-gameobjects, have a custom window to inspect them
        ProvinceList.Clear();
        yield return null;
        foreach (var prov in WMSK.instance.provinces)
        {
            ProvinceData provData = Instantiate(_provinceDataPrefab, transform);
            provData.Initialise(prov);
            ProvinceList.Add(provData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
