using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrigadeElement : MonoBehaviour
{
    #region Brigade
    [Header("Costs")]
    [SerializeField] List<ResourceQuantity> _costList;
    internal List<ResourceQuantity> CostList => _costList;
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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
