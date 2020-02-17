using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WorldMapStrategyKit;

public class Brigade : MonoBehaviour
{
    public enum AgentType
    {
        Brigade,
        Saboteur,
        Diplomat,
    }

    [Header("Name")]
    [SerializeField] string _name;
    internal string Name => gameObject.name;//_name;


    #region Leader
    [Header("Leader")]
    [SerializeField] BrigadeLeader _leader;
    public BrigadeLeader Leader { get => _leader; set => _leader = value; }
    #endregion

    #region Units
    [Header("Units")]
    [SerializeField] List<BrigadeElement> _unitList;
    public List<BrigadeElement> UnitList { get => _unitList; set => _unitList = value; }
    #endregion

    #region Stats
    [Header("Stats")]
    [SerializeField] float _attack;
    public float Attack { get => _attack; }
    [SerializeField] float _health;
    public float Health { get => _health; }
    [SerializeField] float _speed;
    public float Speed { get => _speed; }
    #endregion

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

    internal void Initialise(BrigadeLeader leader, Headquarters hq)
    {
        Vector2 position = hq.MountPoint.unity2DLocation;
        GoAnimator = gameObject.WMSK_MoveTo(position.x, position.y);
        GoAnimator.OnPointerDown += (GameObjectAnimator anim) => UnitClicked(anim);
        GoAnimator.OnMoveEnd += (GameObjectAnimator anim) => MoveEnded(anim);

        // Ensure unit is limited terrain, avoid water
        GoAnimator.terrainCapability = TERRAIN_CAPABILITY.OnlyGround;
    }

    private void UnitClicked(GameObjectAnimator anim)
    {
        Debug.Log($"Unit Clicked: {anim.gameObject.name}");
        Unit_Manager.Instance.SelectedUnit = (Unit_Manager.Instance.SelectedUnit == this) ? null : this;
    }

    private void MoveEnded(GameObjectAnimator anim)
    {
        Region region = WMSK.instance.GetProvinceRegion(anim.currentMap2DLocation);
        Debug.Log($"Unit Movement Ended: {anim.gameObject.name} In Region #{region.regionIndex} of Province {region.entity.name}");
    }
}