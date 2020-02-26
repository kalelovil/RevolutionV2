using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

namespace Kalelovil.Revolution.Provinces
{
    public class Province_Manager : MonoBehaviour
    {
        [SerializeField] Province_Data _provinceDataPrefab;

        public List<Province_Data> ProvinceList { get => _provinceList; private set => _provinceList = value; }
        [SerializeField] List<Province_Data> _provinceList;

        #region Province Feature Types
        [SerializeField] List<Province_Feature_Type> _province_feature_types;
        internal List<Province_Feature_Type> Province_Feature_Types => _province_feature_types;
        #endregion

        public static Province_Manager Instance => _instance;

        static Province_Manager _instance;

        // Start is called before the first frame update
        void Awake()
        {
            _instance = this;

            BuildProvinceList();
        }

        private void BuildProvinceList()
        {
            // TODO Make these non-gameobjects, have a custom window to inspect them
            ProvinceList.Clear();
            foreach (var prov in WMSK.instance.provinces)
            {
                Province_Data provData = Instantiate(_provinceDataPrefab, transform);
                provData.Initialise(prov);
                ProvinceList.Add(provData);
            }

            StartCoroutine(InitialiseTerrainFeatures());
        }

        private IEnumerator InitialiseTerrainFeatures()
        {
            yield return null;
            Color color = new Color(1f, 0f, 0f, 0.5f);

            foreach (var feature in _province_feature_types)
            {
                foreach (Province_Data prov in ProvinceList)
                {
                    prov.ProvinceFeatureTypeMap.Add(feature, new List<Region>());
                }
                var texture = feature.Texture;
                float textureAspectRatio = (float)texture.width / (float)texture.height;
                for (int provIndex = 0; provIndex < ProvinceList.Count; provIndex++)
                {
                    var name = feature.Name;
                    var provData = ProvinceList[provIndex];
                    int[] featureRegionIndexes = GetProvinceFeatureRegionIndexes(provData.Province, name);
                    foreach (int index in featureRegionIndexes)
                    {
                        var region = provData.Province.regions[index];
                        provData.ProvinceFeatureTypeMap[feature].Add(region);

                        Vector2 textureScale = new Vector2(
                            ((1f / region.rect2D.width) / 10000f),
                            (((1f / region.rect2D.height) / 10000f) / textureAspectRatio) / 0.5f);
                        WMSK.instance.ToggleProvinceRegionSurface(
                            provIndex, index, true, color, texture, textureScale, Vector2.zero, 0f);

                        WMSK.instance.PathFindingCustomRouteMatrixSet(region, 1f / feature.Movement_Multiplier);
                    }
                }
            }
        }

        internal int[] GetProvinceFeatureRegionIndexes(Province province, string featureName)
        {
            string featureRegionIndexesString = province.attrib[featureName];
            if (string.IsNullOrEmpty(featureRegionIndexesString))
            {
                return new int[0];
            }
            else
            {
                string[] splitFeatureRegionIndexes = featureRegionIndexesString.Split(';');
                int[] regionIndexArray = new int[splitFeatureRegionIndexes.Length];
                for (int i = 0; i < splitFeatureRegionIndexes.Length; i++)
                {
                    regionIndexArray[i] = Convert.ToInt32(splitFeatureRegionIndexes[i]);
                }
                return regionIndexArray;
            }
        }

        Dictionary<Region, Province_Feature_Type> _regionFeatureTypeMap;
        internal Province_Feature_Type GetFeatureForRegion(Region region)
        {
            if (_regionFeatureTypeMap == null)
            {
                _regionFeatureTypeMap = new Dictionary<Region, Province_Feature_Type>();
                foreach (var prov in ProvinceList)
                {
                    foreach (var featureRegionListMap in prov.ProvinceFeatureTypeMap)
                    {
                        foreach (var currentRegion in featureRegionListMap.Value)
                        {
                            _regionFeatureTypeMap.Add(currentRegion, featureRegionListMap.Key);
                        }
                    }
                }
            }
            if (_regionFeatureTypeMap.TryGetValue(region, out var featureType))
            {
                return featureType;
            }
            return null;
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}