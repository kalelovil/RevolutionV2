using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[DefaultExecutionOrder(-100)]
[ExecuteAlways]
public class Recruitment_Manager : MonoBehaviour
{
    [SerializeField] List<BrigadeElement> _unitDefinitionsList = new List<BrigadeElement>();
    internal List<BrigadeElement> UnitDefinitionsList => _unitDefinitionsList;

    internal BrigadeElement GetUnitTypePrefab(BrigadeElement unitType)
    {
        return _unitDefinitionsList.Find(x => x.name == unitType.name);
    }

    public void Open()
    {

    }
}
