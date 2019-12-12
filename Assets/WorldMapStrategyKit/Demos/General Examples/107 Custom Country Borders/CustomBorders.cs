using UnityEngine;


namespace WorldMapStrategyKit {
	public class CustomBorders : MonoBehaviour {

		WMSK map;
		public Texture2D borderTexture;

		void Start () {
			// Get a reference to the World Map API:
			map = WMSK.instance;

			// Color and add outline to Algeria
			int countryIndex = map.GetCountryIndex ("Algeria");
			map.ToggleCountrySurface (countryIndex, true, new Color (1, 1, 1, 0.25f));
			map.ToggleCountryOutline (countryIndex, true, borderTexture, 0.5f, Color.gray);

			// Color and add outline to Niger
			countryIndex = map.GetCountryIndex ("Niger");
			map.ToggleCountrySurface (countryIndex, true, new Color (0, 1, 0, 0.25f));
			map.ToggleCountryOutline (countryIndex, true, borderTexture, 0.5f, Color.green);

			// Merge two country regions and add a common border
			Region area = map.RegionMerge(map.GetCountry("Spain").mainRegion, map.GetCountry("France").mainRegion);
			map.DrawRegionOutline ("AreaOutline", area, borderTexture, 0.5f, Color.blue);

			// Zoom into the zone
			map.FlyToCountry (countryIndex, 0, 0.3f);
		}


	}

}

