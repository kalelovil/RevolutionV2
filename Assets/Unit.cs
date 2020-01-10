using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] List<ResourceQuantity> _costList; 

    [SerializeField] float _attack;
    [SerializeField] float _health;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Serializable]
    private class ResourceQuantity
    {
        [SerializeField] Resource _resource;
        [SerializeField] int _quantity;
    }
}