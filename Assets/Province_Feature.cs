using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Province_Feature : MonoBehaviour
{
    enum Type
    {
        Abstract = 0,
        Swamp = 1,
        Forest = 2,
        Hills = 3,
        Mountains = 4,
    }
    [SerializeField] Type _type;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
