using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class Province_Manager : MonoBehaviour
{
    public static Province_Manager Instance => _instance;
    static Province_Manager _instance;

    [SerializeField] List<ProvinceData> _provinceList;

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;

        BuildProvinceList();
    }

    private void BuildProvinceList()
    {
        // TODO Make these non-gameobjects, have a custom window to inspect them
        _provinceList.Clear();
        foreach (var prov in WMSK.instance.provinces)
        {
            GameObject go = new GameObject($"{prov.name}");
            go.transform.SetParent(transform);
            ProvinceData provData = go.AddComponent<ProvinceData>();
            provData.Initialise(prov);
            _provinceList.Add(provData);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
