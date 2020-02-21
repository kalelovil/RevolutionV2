using System;
using System.Collections.Generic;
using System.Text;
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
		GUIStyle labelStyle, labelStyleShadow, buttonStyle;

		[SerializeField] bool Show_City_Names;

		void Start()
		{

			// 1) Get a reference to the WMSK API
			map = WMSK.instance;

			// UI Setup - non-important, only for this demo
			labelStyle = new GUIStyle();
			labelStyle.alignment = TextAnchor.MiddleCenter;
			labelStyle.normal.textColor = Color.white;
			labelStyleShadow = new GUIStyle(labelStyle);
			labelStyleShadow.normal.textColor = Color.black;
			buttonStyle = new GUIStyle(labelStyle);
			buttonStyle.alignment = TextAnchor.MiddleLeft;
			buttonStyle.normal.background = Texture2D.whiteTexture;
			buttonStyle.normal.textColor = Color.white;

			// setup GUI resizer - only for the demo
			GUIResizer.Init(800, 500);

			/* Register events: this is optionally but allows your scripts to be informed instantly as the mouse enters or exits a country, province or city */
			/*
			map.OnCityEnter += (int cityIndex) => Debug.Log("Entered city " + map.cities[cityIndex].name);
			map.OnCityExit += (int cityIndex) => Debug.Log("Exited city " + map.cities[cityIndex].name);
			map.OnCityClick += (int cityIndex, int buttonIndex) => Debug.Log("Clicked city " + map.cities[cityIndex].name);
			map.OnProvinceEnter += (int provinceIndex, int regionIndex) => Debug.Log("Entered province " + map.provinces[provinceIndex].name);
			map.OnProvinceExit += (int provinceIndex, int regionIndex) => Debug.Log("Exited province " + map.provinces[provinceIndex].name);
			map.OnProvinceClick += (int provinceIndex, int regionIndex, int buttonIndex) => Debug.Log("Clicked province " + map.provinces[provinceIndex].name);
			*/


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
			*/

			// 5) Refresh map and frontiers
			map.drawAllProvinces = true;
			map.Redraw(true);

			// 6) Add province names
			//map.DrawProvinceLabels(yunnanCountryIndex);

			// 7) Fly to country and fit zoom
			var country = map.GetCountryIndex("Republic of Serbia");
			//float zoomLevel = map.GetCountryRegionZoomExtents(country);
			//map.FlyToCountry(country, 2f, zoomLevel);
			int provIndex = map.GetProvinceIndex("Republic of Serbia", "Grad Beograd");
			float zoomLevel = map.GetProvinceZoomExtents(country);
			map.FlyToProvince(provIndex, 1.5f, zoomLevel * 2.5f);

			// Draw City Names
			if (Show_City_Names)
			{
				AddCityNames();
			}
		}

		private void AddCityNames()
		{
			foreach (City city in map.cities)
			{
				bool isCapital = city.cityClass == CITY_CLASS.COUNTRY_CAPITAL;
				Vector2 markerOffset = (isCapital) ? Vector2.down * 0.00075f : Vector2.down * 0.0005f;
				float markerScale = (isCapital) ? 0.001125f : 0.001f;

				map.AddMarker2DText(city.name, city.unity2DLocation + markerOffset, markerScale);
			}
		}

		// Update is called once per frame
		void OnGUI()
		{

			// Do autoresizing of GUI layer
			GUIResizer.AutoResize();

			// Check whether a country or city is selected, then show a label with the entity name and its neighbours (new in V4.1!)
			if (map.countryHighlighted != null || map.cityHighlighted != null || map.provinceHighlighted != null)
			{
				string text;
				if (map.cityHighlighted != null)
				{
					if (!map.cityHighlighted.name.Equals(map.cityHighlighted.province))
					{ // show city name + province & country name
						text = "City: " + map.cityHighlighted.name + " (" + map.cityHighlighted.province + ", " + map.countries[map.cityHighlighted.countryIndex].name + ")";
					}
					else
					{   // show city name + country name (city is a capital with same name as province)
						text = "City: " + map.cityHighlighted.name + " (" + map.countries[map.cityHighlighted.countryIndex].name + ")";
					}
				}
				else if (map.provinceHighlighted != null)
				{
					text = map.provinceHighlighted.name + ", " + map.countryHighlighted.name;
					List<Province> neighbours = map.ProvinceNeighboursOfCurrentRegion();
					if (neighbours.Count > 0)
						text += "\n" + EntityListToString<Province>(neighbours);
				}
				else
				{
					text = "";
				}
				float x, y;
				x = Screen.width / 2.0f;
				y = Screen.height - 40;

				// shadow
				GUI.Label(new Rect(x - 1, y - 1, 0, 10), text, labelStyleShadow);
				GUI.Label(new Rect(x + 1, y + 2, 0, 10), text, labelStyleShadow);
				GUI.Label(new Rect(x + 2, y + 3, 0, 10), text, labelStyleShadow);
				GUI.Label(new Rect(x + 3, y + 4, 0, 10), text, labelStyleShadow);
				// texst face
				GUI.Label(new Rect(x, y, 0, 10), text, labelStyle);
			}
		}


		// Utility functions called from OnGUI:
		string EntityListToString<T>(List<T> entities)
		{
			StringBuilder sb = new StringBuilder("Neighbours: ");
			for (int k = 0; k < entities.Count; k++)
			{
				if (k > 0)
				{
					sb.Append(", ");
				}
				sb.Append(((IAdminEntity)entities[k]).name);
			}
			return sb.ToString();
		}
	}
}