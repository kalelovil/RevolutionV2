using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class MapClickHandler : MonoBehaviour
{
    WMSK _map;

    // Start is called before the first frame update
    void Start()
    {
        _map = WMSK.instance;
        _map.OnClick += (float x, float y, int buttonIndex) => 
        {
            if (Unit.SelectedUnit) 
            {
                Debug.Log($"Move Unit: {Unit.SelectedUnit.gameObject.name} to {x}, {y}");
                MoveUnitWithPathFinding(Unit.SelectedUnit, new Vector2(x, y));
            }
            else if (true)
            {

            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
     /// Moves the unit with path finding.
     /// </summary>
    void MoveUnitWithPathFinding(Unit unit, Vector2 destination)
    {
        if (unit.Speed > float.Epsilon)
        {
            unit.GoAnimator.MoveTo(destination, 1e4f / unit.Speed, DURATION_TYPE.MapLap);
        }
        else
        {
            Debug.LogWarning($"Unit ({unit}) Has 0 Speed: Cannot Give It A Movement Order");
        }
    }
}
