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

    private void Awake()
    {
        _instance = this;

        SetUpStockpiles();
    }

    private void SetUpStockpiles()
    {
        foreach (var resourceType in ResourcesManager.Instance.ResourceNameToTypeMap.Values)
        {
            _resourceStockpileList.Add(new Unit.ResourceQuantity(resourceType, 0));
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

    internal void AddResources(List<Unit.ResourceQuantity> list)
    {
        foreach (var resource in list)
        {
            var toAddTo = _resourceStockpileList.Find(_x => _x == resource);
            toAddTo.Add(resource.Quantity);
        }
    }
    internal void SubtractResources(List<Unit.ResourceQuantity> list)
    {
        foreach (var resource in list)
        {
            var toAddTo = _resourceStockpileList.Find(x => x.Resource == resource.Resource);
            toAddTo.Add(resource.Quantity * -1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
