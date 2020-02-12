using TMPro;
using UnityEngine;

public abstract class UI_AbstractInterfacePanel : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] protected GameObject _visualsParentObject;
    [SerializeField] protected TextMeshProUGUI _nameText;

    public virtual void Open()
    {
        _visualsParentObject.SetActive(true);
    }

    public virtual void Close()
    {
        _visualsParentObject.SetActive(false);
    }
}