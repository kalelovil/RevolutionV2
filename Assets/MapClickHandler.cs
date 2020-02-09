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
        // Example of durations
        unit.GoAnimator.MoveTo(destination, 0.1f, DURATION_TYPE.Step);
        //												tank.MoveTo (destination, 2f, DURATION_TYPE.Route);
        //												tank.MoveTo (destination, 100f, DURATION_TYPE.MapLap);
    }
}
