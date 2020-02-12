using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
[ExecuteAlways]
public class Recruitment_Manager : MonoBehaviour
{
    [SerializeField] List<UnitScript> _unitDefinitionsList = new List<UnitScript>();
    internal List<UnitScript> UnitDefinitionsList => _unitDefinitionsList;

    internal UnitScript GetUnitTypePrefab(UnitScript unitType)
    {
        return _unitDefinitionsList.Find(x => x.name == unitType.name);
    }

    public void Open()
    {

    }
}
