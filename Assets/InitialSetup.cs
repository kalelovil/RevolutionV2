using System.Linq;
using UnityEngine;
using WorldMapStrategyKit;

/// <summary>
/// Configure a provinces pool by creating a special hidden country that takes all provinces using the function CountryCreateProvincesPool
/// Then extract a province from this pool and create a new country using the function ProvinceToCountry
/// Adds new provinces from the pool to the new country
/// </summary>

namespace WorldMapStrategyKit
{

	public class InitialSetup : MonoBehaviour
	{

		WMSK map;

		void Start()
		{

			// 1) Get a reference to the WMSK API
			map = WMSK.instance;

			/*
			int countryIndex = -1;
			// Remove non-Italian countries
			for (int i = 0; i < map.countries.Length; i++)
			{
				if (map.countries[i].name != "Italy")
				{
					Debug.Log("Country Name: " + map.countries[i].name);
					map.CountryDelete(i, true);
					i--;
				}
				else
				{
					countryIndex = i;
				}
			}

			// 5) Refresh map and frontiers
			map.drawAllProvinces = true;
			map.Redraw(true);
			*/

			// 6) Add province names
			//map.DrawProvinceLabels(yunnanCountryIndex);

			// 7) Fly to country and fit zoom
			var italy = map.GetCountryIndex("Italy");
			float zoomLevel = map.GetCountryRegionZoomExtents(italy);
			map.FlyToCountry(italy, 2f, zoomLevel);

		}


	}

}
