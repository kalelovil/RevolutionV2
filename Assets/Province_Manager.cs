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

    #region Province Feature Textures
    [Header("Province Feature Textures")]
    [SerializeField] Texture2D _forestTexture;
    public Texture2D ForestTexture => _forestTexture;
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

        StartCoroutine(InitialiseForests());
    }

    private IEnumerator InitialiseForests()
    {
        yield return null;
        Color color = new Color(1f, 0f, 0f, 0.5f);
        float textureAspectRatio = (float)ForestTexture.width / (float)ForestTexture.height;
        for (int provIndex = 0; provIndex < ProvinceList.Count; provIndex++)
        {
            Province province = WMSK.instance.provinces[provIndex];
            for (int regionIndex = 1; regionIndex < province.regions.Count; regionIndex++)
            {
                var region = province.regions[regionIndex];
                Vector2 textureScale = new Vector2(((1f / region.rect2D.width) / 10000f), (((1f / region.rect2D.height) / 10000f) / textureAspectRatio) / 0.5f);
                WMSK.instance.ToggleProvinceRegionSurface(provIndex, regionIndex, true, color, ForestTexture, textureScale, Vector2.zero, 0f);

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
