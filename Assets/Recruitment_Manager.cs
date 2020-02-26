using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Kalelovil.Revolution.Units
{
    [DefaultExecutionOrder(-100)]
    [ExecuteAlways]
    public class Recruitment_Manager : MonoBehaviour
    {
        [SerializeField] List<BrigadeElement> _elementDefinitionsList = new List<BrigadeElement>();
        internal List<BrigadeElement> ElementDefinitionsList => _elementDefinitionsList;

        [SerializeField] Brigade _brigadePrefab;
        public Brigade BrigadePrefab => _brigadePrefab;

        internal BrigadeElement GetUnitTypePrefab(BrigadeElement unitType)
        {
            return _elementDefinitionsList.Find(x => x.name == unitType.name);
        }

        public void Open()
        {

        }
    }
}
