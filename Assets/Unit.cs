﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldMapStrategyKit;

public class Unit : MonoBehaviour
{
    [Header("Name")]
    [SerializeField] string _name;
    internal string Name => gameObject.name;//_name;

    [Header("Costs")]
    [SerializeField] List<ResourceQuantity> _costList;
    internal List<ResourceQuantity> CostList => _costList;

    [Header("Stats")]
    [SerializeField] float _attack;
    public float Attack { get => _attack; }
    [SerializeField] float _health;
    public float Health { get => _health; }
    [SerializeField] float _speed;
    public float Speed { get => _speed; }

    [Header("WMSK Animator")]
    GameObjectAnimator _goAnimator;
    public GameObjectAnimator GoAnimator { get => _goAnimator; private set => _goAnimator = value; }

    [Header("Visual")]
    [SerializeField] Renderer[] _renderers;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    internal void Initialise(Vector2 position)
    {
        GoAnimator = gameObject.WMSK_MoveTo(position.x, position.y);
        GoAnimator.OnPointerDown += (GameObjectAnimator anim) => UnitClicked(anim);

        // Ensure unit is limited terrain, avoid water
        GoAnimator.terrainCapability = TERRAIN_CAPABILITY.OnlyGround;
    }

    static Unit _selectedUnit;
    static internal Unit SelectedUnit { get { return _selectedUnit; } set { SetSelectedUnit(value); } }
    private static void SetSelectedUnit(Unit value)
    {
        _selectedUnit = value;
        if (_selectedUnit)
        {
            UI_MainInterface.Instance.OpenUnitPanel(_selectedUnit);
        }
        else
        {
            UI_MainInterface.Instance.ClosePanels();
        }
    }

    private void UnitClicked(GameObjectAnimator anim)
    {
        Debug.Log($"Unit Clicked: {anim.gameObject.name}");
        if (SelectedUnit == this)
        {
            SelectedUnit = null;
        }
        else
        {
            SelectedUnit = this;
        }
    }
}