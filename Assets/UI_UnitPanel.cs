using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UnitPanel : UI_AbstractInterfacePanel
{
    [Header("Data")]
    [SerializeField] Unit _unit;
    public Unit Unit { get => _unit; set => _unit = value; }

    //[Header("UI")]


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
