using Kalelovil.Revolution.Provinces;
using Kalelovil.Revolution.UI;
using Kalelovil.Revolution.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

namespace Kalelovil.Revolution.UI
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
            Province_Data provinceData = Province_Manager.Instance.ProvinceList[provinceIndex];

            Debug.Log($"Province Clicked   {regionIndex} of {map.provinces[provinceIndex].name}");
            //if (regionIndex > 0)
            {
                ProvinceRegionClicked(provinceIndex, regionIndex);
            }

            if (!Unit_Manager.Instance.SelectedUnit)
            {
                //Debug.Log("Clicked province: Open Recruitment Panel" + map.provinces[provinceIndex].name);
                UI_MainInterface.Instance.OpenProvincePanel(provinceData);
            }
        }

        [SerializeField] Texture2D texture;
        [SerializeField] Vector2 textureScale;
        [SerializeField] Vector2 textureOffset;
        [SerializeField] float textureRotation;
        private void ProvinceRegionClicked(int provinceIndex, int regionIndex)
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}