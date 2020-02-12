using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractLeader : MonoBehaviour
{
    [Header("Costs")]
    [SerializeField] List<UnitScript.ResourceQuantity> _costList;
    internal List<UnitScript.ResourceQuantity> CostList => _costList;

    #region Authority
    [Header("Authority")]
    [SerializeField] int _authority;
    public int Authority { get => _authority; set => _authority = value; }
    #endregion

    #region Cunning
    [Header("Cunning")]
    [SerializeField] int _cunning;
    public int Cunning { get => _cunning; set => _cunning = value; }
    #endregion

    #region Management
    [Header("Management")]
    [SerializeField] int _management;
    public int Management { get => _management; set => _management = value; }
    #endregion
}
