using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

namespace WorldMapStrategyKit
{
    public class ProvinceClickCatcher : MonoBehaviour
    {
        WMSK map => WMSK.instance;

        // Start is called before the first frame update
        void Start()
        {
            map.OnProvinceClick += (int provinceIndex, int regionIndex, int buttonIndex) => Debug.Log("Clicked province 2" + map.provinces[provinceIndex].name);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}