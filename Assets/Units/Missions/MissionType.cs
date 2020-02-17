using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MissionType", menuName = "ScriptableObjects/MissionType", order = 2)]
public class MissionType : ScriptableObject, IEquatable<ResourceType>
{
    [SerializeField] string _name;
    internal string Name { get => _name; }

    [SerializeField] Province_Feature _provinceFeature;
    internal Province_Feature ProvinceFeature { get => _provinceFeature; }

    [SerializeField] Brigade.AgentType _agentType;
    internal Brigade.AgentType AgentType { get => _agentType; }

    [Range(1, 5)]
    [SerializeField] int _difficulty;
    internal int Difficulty { get => _difficulty; }


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
