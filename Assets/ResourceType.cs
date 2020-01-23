using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

[CreateAssetMenu(fileName = "ResourceType", menuName = "ScriptableObjects/ResourceType", order = 1)]
public class ResourceType : ScriptableObject, IEquatable<ResourceType>
{
    [SerializeField] string _name;
    internal string Name { get => _name; }

    [SerializeField] Sprite _icon;
    internal Sprite Icon { get => _icon; }

    //[SerializeField] int _amount;
    //internal int Amount { get => _amount; }
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
}