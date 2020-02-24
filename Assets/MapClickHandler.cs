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

        // Move Selected Unit To Map Position Clicked
        _map.OnClick += (float x, float y, int buttonIndex) =>
        {
            if (Unit_Manager.Instance.SelectedUnit)
            {
                Debug.Log($"Move Unit: {Unit_Manager.Instance.SelectedUnit.gameObject.name} to {x}, {y}");
                Unit_Manager.Instance.SelectedUnit.MoveWithPathFinding(new Vector2(x, y));
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
