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
            Debug.Log($"Province Clicked   {regionIndex} of {map.provinces[provinceIndex].name}");

            if (!Unit_Manager.Instance.SelectedUnit)
            {
                //Debug.Log("Clicked province: Open Recruitment Panel" + map.provinces[provinceIndex].name);
                UI_MainInterface.Instance.OpenProvincePanel(Province_Manager.Instance.ProvinceList[provinceIndex]);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}