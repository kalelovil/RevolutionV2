using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class RegionClickCatcher : MonoBehaviour
{
    WMSK map => WMSK.instance;

    // Start is called before the first frame update
    void Start()
    {

    }
    void OnEnable()
    {
        map.OnRegionClick += (Region region, int buttonIndex) => RegionClicked(region);
    }


    void OnDisable()
    {
        map.OnRegionClick -= (Region region, int buttonIndex) => RegionClicked(region);
    }

    private void RegionClicked(Region region)
    {
        if (region.entity is Province)
        {
            //Debug.Log($"Province Region Clicked   {region.regionIndex} of {region.entity.name}");
        }
        else if (region.entity is Country)
        {
            //Debug.Log($"Country Region Clicked   {region.regionIndex} of {region.entity.name}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
