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
        void OnEnable()
        {
            map.OnProvinceClick += (int provinceIndex, int regionIndex, int buttonIndex) => ProvinceClicked(provinceIndex, regionIndex);
        }
        void OnDisable()
        {
            map.OnProvinceClick -= (int provinceIndex, int regionIndex, int buttonIndex) => ProvinceClicked(provinceIndex, regionIndex);
        }

        private void ProvinceClicked(int provinceIndex, int regionIndex)
        {
            ProvinceData provinceData = Province_Manager.Instance.ProvinceList[provinceIndex];

            Debug.Log($"Province Clicked   {regionIndex} of {map.provinces[provinceIndex].name}");
            if (regionIndex > 0)
            {
                ProvinceRegionClicked(provinceIndex, regionIndex);
            }

            if (!Unit_Manager.Instance.SelectedUnit)
            {
                //Debug.Log("Clicked province: Open Recruitment Panel" + map.provinces[provinceIndex].name);
                UI_MainInterface.Instance.OpenProvincePanel(provinceData);
            }
        }

        private void ProvinceRegionClicked(int provinceIndex, int regionIndex)
        {
            Color color = new Color(1f, 0f, 0f, 0.5f);
            map.ToggleProvinceRegionSurface(provinceIndex, regionIndex, true, color);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}