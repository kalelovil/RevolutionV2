using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

namespace Kalelovil.Revolution.Structures
{
    public class HeadquarterManager : MonoBehaviour
    {
        [SerializeField] string _mountPointID;
        [SerializeField] Headquarters _headquartersPrefab;
        [SerializeField] Province_Feature _forestFeaturePrefab;

        [Header("Resources")]
        [SerializeField] private List<ResourceQuantity> _resourceStockpileList;
        internal List<ResourceQuantity> ResourceStockpileList { get => _resourceStockpileList; private set => _resourceStockpileList = value; }

        [Header("HQ List")]
        [SerializeField] List<Headquarters> _hqList;
        internal List<Headquarters> HQList => _hqList;

        static HeadquarterManager _instance;
        public static HeadquarterManager Instance => _instance;


        private void OnValidate()
        {
            _instance = this;
            SetUpStockpiles();
        }

        private void SetUpStockpiles()
        {
            foreach (var resourceType in ResourcesManager.Instance.ResourceNameToTypeMap.Values)
            {
                if (ResourceStockpileList.Find(_x => _x.Resource == resourceType) == null)
                {
                    ResourceStockpileList.Add(new ResourceQuantity(resourceType, 0));
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            GameObject mountPointTypePrefab = null;
            foreach (var mp in WMSK.instance.mountPoints)
            {
                switch (mp.type)
                {
                    case 0:
                        mountPointTypePrefab = _headquartersPrefab.gameObject;
                        break;
                    case 1:
                        mountPointTypePrefab = _forestFeaturePrefab.gameObject;
                        break;
                    default:
                        break;
                }
                var go = Instantiate(mountPointTypePrefab, WMSK.instance.gameObject.transform);
                var province = WMSK.instance.GetProvince(mp.unity2DLocation);
                mp.provinceIndex = WMSK.instance.GetProvinceIndex(province);
                go.transform.localPosition = mp.unity2DLocation;
                go.transform.localScale = new Vector3(go.transform.localScale.x / WMSK.instance.transform.localScale.x,
                                                        go.transform.localScale.y / WMSK.instance.transform.localScale.y,
                                                        go.transform.localScale.z / WMSK.instance.transform.localScale.z);
                go.name = mp.name;

                switch (mp.type)
                {
                    case 0:
                        Headquarters hq = go.GetComponent<Headquarters>();
                        hq.Initialise(mp);
                        HQList.Add(hq);
                        break;
                    case 1:

                        break;
                    default:
                        break;
                }
            }

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}