using Kalelovil.Revolution.Missions;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalelovil.Revolution.Provinces
{
    [CreateAssetMenu(fileName = "Province_Feature_Type", menuName = "ScriptableObjects/Province Feature Type", order = 3)]
    public class Province_Feature_Type : ScriptableObject, IEquatable<Province_Feature_Type>
    {
        #region Properties
        [SerializeField] string _name;
        internal string Name { get => _name; }

        [SerializeField] Sprite _icon;
        internal Sprite Icon { get => _icon; }

        [SerializeField] Texture2D _texture;
        internal Texture2D Texture { get => _texture; }

        [Range(0f, 1f)]
        [SerializeField] float _movement_multiplier = 1f;
        internal float Movement_Multiplier { get => _movement_multiplier; }

        [Range(0f, 2f)]
        [SerializeField] float _view_range_multiplier = 1f;
        internal float View_Range_Multiplier { get => _view_range_multiplier; }

        [SerializeField] bool _blocks_view_range;
        internal bool Blocks_View_Range { get => _blocks_view_range; }

        [SerializeField] List<MissionType> _mission_types;
        internal List<MissionType> Mission_Types { get => _mission_types; }
        #endregion


        #region IEquatable
        public bool Equals(Province_Feature_Type other)
        {
            return null != other && Name.GetHashCode() == other.Name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as Province_Feature_Type);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        #endregion
    }
}
