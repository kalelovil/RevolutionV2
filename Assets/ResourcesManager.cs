using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-200)]
public class ResourcesManager : MonoBehaviour
{
    static ResourcesManager _instance;
    public static ResourcesManager Instance => _instance;

    public Dictionary<string, ResourceType> ResourceNameToTypeMap { get => _resourceNameToTypeMap; private set => _resourceNameToTypeMap = value; }

    Dictionary<string, ResourceType> _resourceNameToTypeMap = new Dictionary<string, ResourceType>();

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
        BuildResourcesMap();
    }

    private void BuildResourcesMap()
    {
        var resources = Resources.LoadAll<ResourceType>("ResourceTypes");
        foreach (var type in resources)
        {
            ResourceNameToTypeMap.Add(type.Name, type);
        }
    }
}
