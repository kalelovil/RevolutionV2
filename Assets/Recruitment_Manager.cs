using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
[ExecuteAlways]
public class Recruitment_Manager : MonoBehaviour
{
    [SerializeField] List<Unit> _unitDefinitionsList = new List<Unit>();
    internal List<Unit> UnitDefinitionsList => _unitDefinitionsList;

    internal Unit GetUnitTypePrefab(Unit unitType)
    {
        return _unitDefinitionsList.Find(x => x.name == unitType.name);
    }

    public void Open()
    {

    }
}
