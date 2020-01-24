using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class HeadquarterManager : MonoBehaviour
{
    [SerializeField] string _mountPointID;
    [SerializeField] Headquarters _headquartersPrefab;

    [Header("Resources")]
    [SerializeField] internal List<Unit.ResourceQuantity> _resourceStockpileList;


    static HeadquarterManager _instance;
    public static HeadquarterManager Instance => _instance;

    private void OnValidate()
    {
        _instance = this;
        SetUpStockpiles();
    }

    private void SetUpStockpiles()
    {
        foreach (var resourceType in ResourcesManager.Instance.ResourceNameToTypeMap.Values)
        {
            if (_resourceStockpileList.Find(_x => _x.Resource == resourceType) == null)
            {
                _resourceStockpileList.Add(new Unit.ResourceQuantity(resourceType, 0));
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        foreach (var mp in WMSK.instance.mountPoints)
        {
            var go = Instantiate(_headquartersPrefab, WMSK.instance.gameObject.transform);
            go.transform.localPosition = mp.unity2DLocation;
            go.transform.localScale = new Vector3(go.transform.localScale.x / WMSK.instance.transform.localScale.x,
                                                    go.transform.localScale.y / WMSK.instance.transform.localScale.y,
                                                    go.transform.localScale.z / WMSK.instance.transform.localScale.z);
            go.name = mp.name;
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
