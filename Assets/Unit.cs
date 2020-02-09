using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldMapStrategyKit;

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

    [Header("WMSK Animator")]
    GameObjectAnimator _goAnimator;
    public GameObjectAnimator GoAnimator { get => _goAnimator; private set => _goAnimator = value; }

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
        GoAnimator.OnPointerDown += (GameObjectAnimator anim) => Debug.Log("UNIT EVENT: " + gameObject.name + " mouse button down.");

        // Ensure unit is limited terrain, avoid water
        GoAnimator.terrainCapability = TERRAIN_CAPABILITY.OnlyGround;

        GoAnimator.autoScale = false;

        //spawnedUnit.transform.localPosition = ProvinceData.Province.center;
        transform.localScale = new Vector3( transform.localScale.x / WMSK.instance.transform.localScale.x,
                                            transform.localScale.y / WMSK.instance.transform.localScale.y,
                                            transform.localScale.z / WMSK.instance.transform.localScale.z);

    }

    static internal Unit SelectedUnit;
    private void UnitClicked(GameObjectAnimator anim)
    {
        Debug.Log($"Unit Clicked: {anim.gameObject.name}");
        SelectedUnit = this;
    }
}