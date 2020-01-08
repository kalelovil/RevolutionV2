using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadquarterManager : MonoBehaviour
{
    [SerializeField] string _mountPointID;
    [SerializeField] Headquarters _headquartersPrefab;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var mp in WorldMapStrategyKit.WMSK.instance.mountPoints)
        {
            var go = Instantiate(_headquartersPrefab);
            go.transform.position = mp.unity2DLocation;
            go.name = mp.name;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
