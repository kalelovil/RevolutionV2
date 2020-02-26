using Kalelovil.Revolution.Units;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Kalelovil.Revolution.Missions
{
    [CreateAssetMenu(fileName = "MissionType", menuName = "ScriptableObjects/MissionType", order = 2)]
    public class MissionType : ScriptableObject, IEquatable<ResourceType>
    {
        #region Properties
        [SerializeField] string _name;
        internal string Name { get => _name; }

        [SerializeField] Brigade.AgentType _agentType;
        internal Brigade.AgentType AgentType { get => _agentType; }

        [Range(1, 5)]
        [SerializeField] int _difficulty = 1;
        internal int Difficulty { get => _difficulty; }
        #endregion


        #region IEquatable
        public bool Equals(ResourceType other)
        {
            return null != other && Name.GetHashCode() == other.Name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as ResourceType);
        }
        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
        #endregion
    }
}
