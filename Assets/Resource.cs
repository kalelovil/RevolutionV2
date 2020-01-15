using UnityEngine;
using UnityEngine.UI;

internal abstract class Resource : MonoBehaviour
{
    [SerializeField] Sprite _icon;
    internal Sprite Icon { get => _icon; }

    [SerializeField] int _amount;


    private Resource _instance;

    protected virtual Resource Instance { get { return _instance; } }

    private void Start()
    {
        _instance = this;
    }
}
