using System;
using UnityEngine;

[Serializable]
internal class ResourceQuantity : IEquatable<ResourceQuantity>
{
    internal ResourceQuantity(ResourceType resource, int quantity)
    {
        _resource = resource;
        _quantity = quantity;
    }

    [SerializeField] ResourceType _resource;
    internal ResourceType Resource { get => _resource; }

    [SerializeField] int _quantity;
    internal int Quantity { get => _quantity; private set { _quantity = value; } }

    public override bool Equals(object obj)
    {
        return Equals((obj as ResourceQuantity));
    }
    public bool Equals(ResourceQuantity other)
    {
        return Resource.Equals(other.Resource);
    }

    internal void Add(int quantity)
    {
        Quantity = Mathf.Max(0, Quantity + quantity);
    }

    internal void Set(int quantity)
    {
        Quantity = Mathf.Max(0, quantity);
    }
}