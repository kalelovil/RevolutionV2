using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ResourcesDisplay : MonoBehaviour
{
    [SerializeField] UI_Resource_Panel _resourcePanelPrefab;
    [SerializeField] Transform _resourceListArea;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var resourceType in ResourcesManager.Instance.ResourceNameToTypeMap.Values)
        {
            var panel = Instantiate(_resourcePanelPrefab, _resourceListArea);
            var resourceQuantity = new Unit.ResourceQuantity(resourceType, 0);
            panel.Initialise(resourceQuantity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
