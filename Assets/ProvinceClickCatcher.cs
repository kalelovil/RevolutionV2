using System;
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
            map.OnProvinceClick += (int provinceIndex, int regionIndex, int buttonIndex) => ProvinceClicked(provinceIndex);
        }

        private void ProvinceClicked(int provinceIndex)
        {
            Debug.Log("Clicked province: Open Recruitment Panel" + map.provinces[provinceIndex].name);
            UI_ProvincePanel.Instance.OpenRecruitmentPanel();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}