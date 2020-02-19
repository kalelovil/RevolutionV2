using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class Province_Manager : MonoBehaviour
{
    [SerializeField] ProvinceData _provinceDataPrefab;

    public List<ProvinceData> ProvinceList { get => _provinceList; private set => _provinceList = value; }
    [SerializeField] List<ProvinceData> _provinceList;

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
            ProvinceData provData = Instantiate(_provinceDataPrefab, transform);
            provData.Initialise(prov);
            ProvinceList.Add(provData);
        }

        StartCoroutine(InitialiseTerrainFeatures());
    }

    private IEnumerator InitialiseTerrainFeatures()
    {
        yield return null;
        Color color = new Color(1f, 0f, 0f, 0.5f);

        /*
        foreach (var prov in ProvinceList)
        {
            WMSK.instance.PathFindingCustomRouteMatrixSet(prov.Province, 100);
        }
        */
        foreach (var feature in _province_feature_types)
        {
            var texture = feature.Texture;
            float textureAspectRatio = (float)texture.width / (float)texture.height;
            for (int provIndex = 0; provIndex < ProvinceList.Count; provIndex++)
            {
                var name = feature.Name;
                Province province = WMSK.instance.provinces[provIndex];
                int[] featureRegionIndexes = GetProvinceFeatureRegionIndexes(province, name);
                foreach (int index in featureRegionIndexes)
                {
                    var region = province.regions[index];
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

    private int[] GetProvinceFeatureRegionIndexes(Province province, string featureName)
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
