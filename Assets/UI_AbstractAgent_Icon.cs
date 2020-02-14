using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class UI_AbstractAgent_Icon : MonoBehaviour
{
    [SerializeField] protected Image _imageIcon;

    [SerializeField] protected TextMeshProUGUI _nameText;
    [SerializeField] protected TextMeshProUGUI _levelText;
    [SerializeField] protected TextMeshProUGUI _speedText;

    [SerializeField] protected UI_AbstractAgentElement_Panel _elementPrefab;
    [SerializeField] protected Transform _elementArea;
    [SerializeField] protected List<UI_AbstractAgentElement_Panel> _elementList;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
