using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace WorldMapStrategyKit {
	public partial class Province: AdminEntity, IExtendableAttribute {
			
		int[] _neighbours;

		/// <summary>
		/// Custom array of provinces that could be reached from this province. Useful for Province path-finding.
		/// It defaults to natural neighbours of the province but you can modify its contents and add your own potential destinations per province.
		/// </summary>
		public override int[] neighbours {
			get {
				if (_neighbours == null) {
					int cc = 0;
					List<Province> nn = new List<Province> ();
					if (regions != null) {
						regions.ForEach (r => {
							if (r != null && r.neighbours != null) {
								r.neighbours.ForEach (n => {
									if (n != null) {
										Province otherProvince = (Province)n.entity;
										if (!nn.Contains (otherProvince))
											nn.Add (otherProvince);
									}
								}
								);
							
							}
						});
						cc = nn.Count;
					}
					_neighbours = new int[cc];
					for (int k = 0; k < cc; k++) {
						_neighbours [k] = WMSK.instance.GetProvinceIndex (nn [k]);
					}
				}
				return _neighbours;
			}
			set {
				_neighbours = value;
			}
		}

		#region internal fields

		// Used internally. Don't change fields below.
		public string packedRegions;
		public int countryIndex;

		#endregion

		public Province (string name, int countryIndex, int uniqueId) {
			this.name = name;
			this.countryIndex = countryIndex;
			this.regions = null; // lazy load during runtime due to size of data
			this.center = Misc.Vector2zero;
			this.uniqueId = uniqueId;
			this.attrib = new JSONObject ();
			this.mainRegionIndex = -1;
		}

		public Province Clone () {
			Province p = new Province (name, countryIndex, uniqueId);
			p.countryIndex = countryIndex;
			p.regions = regions;
			p.center = center;
			p.mainRegionIndex = mainRegionIndex;
			p.attrib = new JSONObject ();
			p.attrib.Absorb (attrib);

			return p;
		}

	}
}

