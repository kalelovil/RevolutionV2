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

    private void Start()
    {
        BuildUnitDefinitionList();
    }

    private void BuildUnitDefinitionList()
    {
        _unitDefinitionsList = GetComponentsInChildren<Unit>().ToList();
    }

    int _childCount;
    // Update is called once per frame
    void Update()
    {
        CheckChildCount();
    }
    private void OnValidate()
    {
        BuildUnitDefinitionList();
    }

    private void CheckChildCount()
    {
        int newChildCount = transform.childCount;
        if (newChildCount != _childCount)
        {
            _childCount = newChildCount;
            BuildUnitDefinitionList();
        }
    }

    public void Open()
    {

    }
}
