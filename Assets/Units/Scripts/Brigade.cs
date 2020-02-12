using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brigade : MonoBehaviour
{
    #region Leader
    [Header("Leader")]
    [SerializeField] BrigadeLeader _leader;
    public BrigadeLeader Leader { get => _leader; set => _leader = value; }
    #endregion

    #region Units
    [Header("Units")]
    [SerializeField] List<UnitScript> _unitList;
    public List<UnitScript> UnitList { get => _unitList; set => _unitList = value; }
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
