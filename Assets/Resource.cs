using UnityEngine;
using UnityEngine.UI;

internal abstract class Resource : MonoBehaviour
{
    [SerializeField] Image _icon;
    [SerializeField] int _amount;
}
