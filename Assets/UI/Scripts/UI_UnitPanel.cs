using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_UnitPanel : UI_AbstractInterfacePanel
{
    [Header("Data")]
    [SerializeField] Brigade _unit; 
    public Brigade Unit { get { return _unit; } internal set { UnitSelected(value); } }
    private void UnitSelected(Brigade value)
    {
        _unit = value;
        _nameText.text = Unit.Name;
    }

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
