using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    [Header("Name")]
    [SerializeField] string _name;

    [Header("Costs")]
    [SerializeField] List<ResourceQuantity> _costList;
    internal List<ResourceQuantity> CostList => _costList;

    [Header("Stats")]
    [SerializeField] float _attack;
    public float Attack { get => _attack; }
    [SerializeField] float _health;
    public float Health { get => _health; }


    [Header("Visual")]
    [SerializeField] SpriteRenderer _image;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    internal class ResourceQuantity
    {
        internal ResourceQuantity(ResourceType resource, int quantity)
        {
            _resource = resource;
            _quantity = quantity;
        }

        [SerializeField] ResourceType _resource;
        internal ResourceType Resource { get => _resource; }

        [SerializeField] int _quantity;
        internal int Quantity { get => _quantity; }
    }
}