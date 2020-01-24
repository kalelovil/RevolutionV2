using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-200)]
[ExecuteAlways]
public class ResourcesManager : MonoBehaviour
{
    static ResourcesManager _instance;
    public static ResourcesManager Instance { get { return _instance; } }

    public Dictionary<string, ResourceType> ResourceNameToTypeMap { get => _resourceNameToTypeMap; private set => _resourceNameToTypeMap = value; }

    Dictionary<string, ResourceType> _resourceNameToTypeMap = new Dictionary<string, ResourceType>();

    private void OnValidate()
    {
        _instance = this;
        BuildResourcesMap();
    }

    private void BuildResourcesMap()
    {
        var resources = Resources.LoadAll<ResourceType>("ResourceTypes");
        foreach (var type in resources.OrderBy(x => x.Order))
        {
            ResourceNameToTypeMap.Add(type.Name, type);
        }
    }
}
