using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UI_Resource_Panel : MonoBehaviour
{
    Resource _resource;

    [SerializeField] Image _iconImage;
    [SerializeField] TextMeshProUGUI _amountText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    internal void Initialise(Unit.ResourceQuantity cost)
    {
        // TODO : Set Resource Icon
        _iconImage.sprite = cost.Resource.Icon;

        _amountText.text = $"{cost.Quantity}";
    }
}
