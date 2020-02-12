using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WorldMapStrategyKit;

public class MapClickHandler : MonoBehaviour
{
    WMSK _map => WMSK.instance;

    // Start is called before the first frame update
    void Start()
    {
        _map.OnClick += (float x, float y, int buttonIndex) => 
        {
            if (Unit_Manager.Instance.SelectedUnit) 
            {
                Debug.Log($"Move Unit: {Unit_Manager.Instance.SelectedUnit.gameObject.name} to {x}, {y}");
                MoveUnitWithPathFinding(Unit_Manager.Instance.SelectedUnit, new Vector2(x, y));
            }
            else if (true)
            {

            }
        };


        // Listen to global Vieport GameObject (VGO) events (... better and simple approach)

        _map.OnVGOPointerDown = delegate (GameObjectAnimator obj) {
            Debug.Log("GLOBAL EVENT: Left button pressed on " + obj.name);
            ColorTankMouseDown(obj);
        };
        _map.OnVGOPointerUp = delegate (GameObjectAnimator obj) {
            Debug.Log("GLOBAL EVENT: Left button released on " + obj.name);
            ColorTankMouseUp(obj);
        };

        _map.OnVGOPointerRightDown = delegate (GameObjectAnimator obj) {
            Debug.Log("GLOBAL EVENT: Right button pressed on " + obj.name);
            ColorTankMouseDown(obj);
        };
        _map.OnVGOPointerRightUp = delegate (GameObjectAnimator obj) {
            Debug.Log("GLOBAL EVENT: Right button released on " + obj.name);
            ColorTankMouseUp(obj);
        };

        _map.OnVGOPointerEnter = delegate (GameObjectAnimator obj) {
            Debug.Log("GLOBAL EVENT: Mouse entered " + obj.name);
            ColorTankHover(obj);
        };
        _map.OnVGOPointerExit = delegate (GameObjectAnimator obj) {
            Debug.Log("GLOBAL EVENT: Mouse exited " + obj.name);
            RestoreTankColor(obj);
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    /// <summary>
     /// Moves the unit with path finding.
     /// </summary>
    void MoveUnitWithPathFinding(UnitScript unit, Vector2 destination)
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


    void ColorTankHover(GameObjectAnimator obj)
    {
        // Changes tank color - but first we store original color inside its attribute bag
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        obj.attrib["color"] = renderer.sharedMaterial.color;
        renderer.material.color = Color.yellow; // notice how I use material and not sharedmaterial - this is to prevent affecting all clone instances - we just want to color this one, so we need to make this material unique.
    }

    void ColorTankMouseDown(GameObjectAnimator obj)
    {
        // Changes tank color to white
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        renderer.sharedMaterial.color = Color.white;
    }

    void ColorTankMouseUp(GameObjectAnimator obj)
    {
        // Changes tank color to white
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        renderer.sharedMaterial.color = Color.yellow;
    }

    void RestoreTankColor(GameObjectAnimator obj)
    {
        // Restores original tank color
        Renderer renderer = obj.GetComponentInChildren<Renderer>();
        Color tankColor = obj.attrib["color"];  // get back the original color from attribute bag
        renderer.sharedMaterial.color = tankColor;
    }
}
