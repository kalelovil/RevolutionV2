using Kalelovil.Revolution.Provinces;
using System;
using System.Collections;
using UnityEngine;
using WorldMapStrategyKit;

namespace Kalelovil.Revolution.Structures
{
    public class Headquarters : MonoBehaviour
    {
        MountPoint _mountPoint;
        internal MountPoint MountPoint => _mountPoint;

        WMSK Map => WMSK.instance;
        Province_Data Province => Province_Manager.Instance.ProvinceList[_mountPoint.provinceIndex];

        public void Initialise(MountPoint mountPoint)
        {
            _mountPoint = mountPoint;

            StartCoroutine(InitialiseCoroutine());
        }

        private IEnumerator InitialiseCoroutine()
        {
            yield return null;
            Province.Population = UnityEngine.Random.Range(5, 21);
            Province.LocalSupportFraction = UnityEngine.Random.Range(0.25f, 0.5f);
        }
    }
}