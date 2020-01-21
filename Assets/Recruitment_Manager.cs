using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
[ExecuteAlways]
public class Recruitment_Manager : MonoBehaviour
{
    static Recruitment_Manager _instance;
    public static Recruitment_Manager Instance => _instance;

    [SerializeField] List<Unit> _unitDefinitionsList = new List<Unit>();
    internal List<Unit> UnitDefinitionsList => _unitDefinitionsList;

    // Start is called before the first frame update
    void Awake()
    {
        _instance = this;
    }

    int _childCount;

    internal Unit GetUnitTypePrefab(Unit unitType)
    {
        return _unitDefinitionsList.Find(x => x.name == unitType.name);
    }

    public void Open()
    {

    }
}
